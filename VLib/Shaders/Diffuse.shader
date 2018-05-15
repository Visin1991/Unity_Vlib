Shader "V/Diffuse" {
	Properties{

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

			struct VertexData
			{
				float4 vertex	: POSITION;
				half3  normal   : NORMAL;
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				half3 normal : TEXCOORD1;

			};

			Interpolators vert(VertexData v)
			{
				Interpolators o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			half4 frag(Interpolators i) : SV_TARGET
			{
				i.normal = normalize(i.normal);      //for non-unit-Length Vector,after Interpolation, will cause non-unit normal
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 lightColor = _LightColor0.rgb;
				half3 diffuse = saturate(dot(lightDir, i.normal)) * lightColor;
				return half4(diffuse,1.0);
			}

			ENDCG
		}
	}
}
