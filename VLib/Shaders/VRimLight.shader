// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/VRimLight" {
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
				float2 texcoord : TEXCOORD0;

			};

			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 intensity : COLOR;
			};

			Interpolators vert(VertexData i)
			{
				Interpolators o;
				float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
				o.pos = mul(UNITY_MATRIX_VP, worldPos);
				o.uv = i.texcoord;

				float3 toCameraDir = normalize(_WorldSpaceCameraPos - worldPos);
				float viewNormalAngle01 = dot(i.normal, toCameraDir);
				o.intensity = smoothstep(_MinViewAngle01, 1.0f,1 - viewNormalAngle01);

				return o;
			}

			half4 frag(Interpolators i) : COLOR
			{
				float3 albedo = tex2D(_MainTex,i.uv).rgb;
				albedo += _Color *  i.intensity;

				return float4(albedo,1.0f);
			}

			ENDCG
		}
	}
}
