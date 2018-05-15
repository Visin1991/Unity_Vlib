
Shader "V/VRimLight4" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		
		Pass
			{
				Name "OutLine"
				Cull Front
				ZWrite On
				//ZTest Always  ----- no matter what the depth is, always render the pixel
				CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			//float4x4 V_MVP;
			float4 _Color;

			struct VertexData1
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
			};
			Interpolators vert(VertexData1 i)
			{
				Interpolators o;
				//o.pos = mul(V_MVP, i.vertex);
				float3 center = UnityObjectToClipPos(half4(0, 0, 0, 1));
				o.pos = UnityObjectToClipPos(i.vertex * half4(1.05,1.05,1.05,1));
				o.pos.z = center.z;
				o.normal = i.normal;
				return o;
			}
			half4 frag(Interpolators i) : COLOR
			{
				return _Color;
			}
				ENDCG
		}
		Pass
		{
			Name "Base"
			ZWrite On
			//ZTest Always
			CGPROGRAM
			#pragma vertex vert2
			#pragma fragment frag2
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float2 uv  : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

			Interpolators vert2(VertexData v)
			{
				Interpolators o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.normal = mul((float3x3)unity_ObjectToWorld,v.normal);
				return o;
			}

			float4 frag2(Interpolators i) : COLOR
			{
				float4 albedo = tex2D(_MainTex,i.uv);
				return albedo;
			}
			ENDCG
		}


	}
}
