Shader "V/VDeadDissoveBurn" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EdgeColor1("EdgeColor",color) = (0,0,0,1)
		_ColorFire("FireColor",color) = (1,0,0,1)
		_DissolveMask("Dissolve Mask",2D) = "white"{}
		_Control("Control",Range(0,1)) = 0
		_FlashIntensity("Flash Intensity",Range(0,1)) = 1
		_FlashSpeed("Flash Speed",Range(0,300)) = 100
		_EdgeLength("EdgeLength",Range(0,1))= 0.3
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout"
			   "Queue" = "AlphaTest"
			}

		Cull Back
		ZTest LEqual

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		sampler2D _MainTex;
		sampler2D _DissolveMask;
		half _Control;
		fixed4    _EdgeColor1;
		fixed4	  _ColorFire;
		half _EdgeLength;
		half _FlashIntensity;
		fixed _FlashSpeed;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DissolveMask;
		};



		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			half h = tex2D(_DissolveMask, IN.uv_DissolveMask).r;

			half dissolveValue = h * _Control;
			half albedoGray =1- ((c.r + c.g + c.b) / 3.0);

			half3 emission;

			half edgeHeight = (1 + _EdgeLength);

			half heightA = _Control * edgeHeight;
			half heightB = _Control * (edgeHeight - 0.4);


			half signalA = h < heightA ? 1 : 0;
			half3 emissionA = signalA * lerp(_ColorFire,_EdgeColor1, smoothstep(heightB,heightA,h));

			half signalB = albedoGray < pow(_Control, 0.1 *(_Control + 2)) ? 1 : 0;
			//emissionA += signalB * lerp(_ColorFire, _EdgeColor1, smoothstep(heightB, heightA, albedoGray));

			o.Albedo = c;
			o.Emission = emissionA + (signalB * _ColorFire) * (sin(_Time.x * _FlashSpeed) +1)/2 * _FlashIntensity;

			clip(h  -  _Control );

		}

		ENDCG
	}
	FallBack "Diffuse"
}
