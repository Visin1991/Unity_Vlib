// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/MehsProjection1" {
	Properties
	{
		[NoScaleOffset]_MainTex("Base (RGB)",2D) = "white"{}
		_TintColor("Tint 颜色",Color) = (1,1,1,1)
		_ColorSclaer("亮度",Float) = 1.0
	}

		SubShader{
			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				Tags{ "Queue" = "Geometry+50" }

				CGPROGRAM
				#include "../ShaderLib/VCamera.cginc"
				#include "../ShaderLib/VLinearMath.cginc"

				#pragma vertex vert
				#pragma fragment frag

				UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
				float4x4 _ClipToLocal;

				sampler2D _MainTex;
				float _ColorSclaer;
				float4 _TintColor;

				struct VertexData
				{
					float4 vertex : POSITION;
				};
			
				struct Interpolators
				{
					float4 pos : SV_POSITION;
					float4 clip : TEXCOORD0;
				};
			
			Interpolators vert(VertexData v)
			{
				Interpolators o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.clip = o.pos;
				//o.pos /= o.pos.w;
				//o.pos = mul(_ClipToLocal, o.pos);
				//o.pos /= o.pos.w;
				//o.pos = UnityObjectToClipPos(o.pos);
				return o;
			}

			half4 frag(Interpolators i) : COLOR
			{
				float2 screenUV = V_ComputeScreenPos_Ps(i.clip);
				float depth = tex2D(_CameraDepthTexture, screenUV);

				float4 localPos = mul(_ClipToLocal, float4(i.clip.xy, depth, 1));
				localPos /= localPos.w;

				half4 color = half4(0, 0, 0, 0);
				float2 uv;
				//The Cube mesh-----Edge---Length is 1. so (-x,-y,-z) is (-0.5,-0.5,-0.5)
				//and the (x,y,z) is (0.5,0.5,0.5)--------convert it to (0,1)---> add 0.5 
				uv.x = (localPos.x  + 0.5);
				uv.y = (localPos.z  + 0.5);	
				color = tex2D(_MainTex, uv);
				color.rgb *= (_TintColor * _ColorSclaer);
				return color;
			}

			ENDCG

		}
	}
}
