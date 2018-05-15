Shader "V/WorldNormal" {
	Properties {

	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
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
				return half4(i.normal * 0.5 + 0.5,1);
			}

			ENDCG
		}
	}
}
