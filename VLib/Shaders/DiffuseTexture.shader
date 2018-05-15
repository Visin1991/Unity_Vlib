Shader "V/DiffuseTexture" {
	Properties{
		_MainTex("Albedo",2D) = "white"{}
	}

		SubShader
	{
		Tags{
			"RenderType" = "Opaque"
			"LightMode" = "ForwardBase"   // forward rendering path
		}
		LOD 200

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			half4 _MainTex_ST;

			struct VertexData
			{
				float4 vertex	: POSITION;
				half3  normal   : NORMAL;
				half2  texcoord : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				half3 normal    : TEXCOORD0;
				half2 uv        : TEXCOORD1;

			};

			Interpolators vert(VertexData v)
			{
				Interpolators o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			half4 frag(Interpolators i) : SV_TARGET
			{
				i.normal = normalize(i.normal);      //for non-unit-Length Vector,after Interpolation, will cause non-unit normal
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 lightColor = _LightColor0.rgb;
				half3 albedo = tex2D(_MainTex, i.uv).rgb;
				half3 diffuse = albedo * saturate(dot(lightDir, i.normal)) * lightColor;
				return half4(diffuse,1.0);
			}

			ENDCG
		}
	}
}
