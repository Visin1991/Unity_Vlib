// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/VRimLight2" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MinViewAngle01("Min ViewAngle01",Range(0,0.9)) = 0.5
	}

	SubShader{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	

			sampler2D _MainTex;
			half4 _Color;
			half _MinViewAngle01;

			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord : TEXCOORD0;

			};

			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD3;
			};

			Interpolators vert(VertexData i)
			{
				Interpolators o;
				o.worldPos = mul(unity_ObjectToWorld, i.vertex);
				o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos,1.0f));
				o.normal = mul(i.normal, (float3x3)unity_WorldToObject);
				o.uv = i.texcoord;
				return o;
			}

			half4 frag(Interpolators i) : COLOR
			{
				float3 albedo = tex2D(_MainTex,i.uv).rgb;
				float3 toCamDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float viewNormalAngle10 = saturate(dot(i.normal, toCamDir));
				float intensity = smoothstep(_MinViewAngle01, 1.0, 1-viewNormalAngle10);
				albedo += _Color * intensity;
				return float4(albedo,1.0f);
			}

			ENDCG
		}
	}
}
