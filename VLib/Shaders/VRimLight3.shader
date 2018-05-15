// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/VRimLight3" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Outline("Outline width1",Range(0,1)) = 0.05
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	half4 _Color;
	half _Outline;

	struct VertexData1
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct VertexData2
	{
		float4 vertex   : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct Interpolators
	{
		float4 pos : SV_POSITION;
		float4 color : COLOR;
	};

	struct Interpolators2
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	Interpolators vert(VertexData1 i)
	{
		Interpolators o;
		o.pos = UnityObjectToClipPos(i.vertex);
		float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, i.normal);
		float2 projNormal = mul((float2x2)UNITY_MATRIX_P, viewNormal.xy);
		o.pos.xy += projNormal * o.pos.z *_Outline;
		o.color = _Color;
		return o;
	}

	half4 frag(Interpolators i) : COLOR
	{
		return i.color;
	}


	Interpolators2 vert2(VertexData2 i)
	{
		Interpolators2 o;
		o.pos = UnityObjectToClipPos(i.vertex);
		o.uv = i.texcoord;
		return o;
	}

	half4 frag2(Interpolators2 i) : COLOR
	{
		float4 color = tex2D(_MainTex,i.uv);
		return color;
	}

	ENDCG

	SubShader{

		Pass
		{
			Name "OutLine"
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG

		}

		Pass
		{
			Name "Base"
			CGPROGRAM
			#pragma vertex vert2
			#pragma fragment frag2
			ENDCG
		}

		
	}
}
