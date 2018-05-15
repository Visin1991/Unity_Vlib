Shader "V/Uber Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AutoExposure ("", 2D) = "" {}
        _BloomTex ("", 2D) = "" {}
        _Bloom_DirtTex ("", 2D) = "" {}
        _GrainTex ("", 2D) = "" {}
        _LogLut ("", 2D) = "" {}
        _UserLut ("", 2D) = "" {}
        _Vignette_Mask ("", 2D) = "" {}
        _ChromaticAberration_Spectrum ("", 2D) = "" {}
        _DitheringTex ("", 2D) = "" {}
    }

    CGINCLUDE

        //#pragma target 3.0

        #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
        #pragma multi_compile __ CHROMATIC_ABERRATION


        #include "UnityCG.cginc"

        // Chromatic aberration
        half _ChromaticAberration_Amount;
        sampler2D _ChromaticAberration_Spectrum;
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		float4 _MainTex_ST;

		// -----------------------------------------------------------------------------
		// Vertex shaders

		struct AttributesDefault
		{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};



        struct VaryingsFlipped
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        VaryingsFlipped VertUber(AttributesDefault v)
        {
            VaryingsFlipped o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord.xy;
            return o;
        }

        half4 FragUber(VaryingsFlipped i) : SV_Target
        {
			float2 uv = i.uv;
            half3 color = (0.0).xxx;
            //
            // HDR effects
            // ---------------------------------------------------------

            // Chromatic Aberration
            // Inspired by the method described in "Rendering Inside" [Playdead 2016]
            // https://twitter.com/pixelmager/status/717019757766123520
            #if CHROMATIC_ABERRATION
            {
				
                float2 coords = 2.0 * uv - 1.0;
				float2 end = uv - coords * dot(coords, coords) * _ChromaticAberration_Amount;

                float2 diff = end - uv;
                int samples = clamp(int(length(_MainTex_TexelSize.zw * diff / 2.0)), 3, 6);
                float2 delta = diff / samples;
                float2 pos = uv;
                half3 sum = (0.0).xxx, filterSum = (0.0).xxx;

                for (int i = 0; i < samples; i++)
                {
                    half t = (i + 0.5) / samples;
                    half3 s = tex2D(_MainTex, float4(pos, 0, 0)).rgb;
                    half3 filter = tex2D(_ChromaticAberration_Spectrum, float4(t, 0, 0, 0)).rgb;

					/*half x = 0.5;
					half gbTerm = step(x, t);
					half3 gb =  gbTerm * lerp(half3(0,1,0),half3(0,0,1), smoothstep(x, 1, t));
					
					half rgTerm = step(0, gbTerm);
					half3 rg =  rgTerm * lerp(half3(1, 0, 0), half3(0, 1, 0), smoothstep(0, 0.5, t));
					half3 filter = rg + gb;*/

                    sum += s * filter;
                    filterSum += filter;
                    pos += delta;
                }

                color = sum / filterSum;
            }
            #endif

            color = saturate(color);

            // Back to gamma space if needed
            #if UNITY_COLORSPACE_GAMMA
            {
                color = LinearToGammaSpace(color);
            }
            #endif

            // Done !
            return half4(color, 1.0);
        }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        // (0)
        Pass
        {
            CGPROGRAM

                #pragma vertex VertUber
                #pragma fragment FragUber

            ENDCG
        }
    }
}
