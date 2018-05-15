// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/MehsProjection2" {
	Properties
	{
		[KeywordEnum(Off,On)]_Debug("Debug 模式",Float) = 0
		[Space(30)]

		[NoScaleOffset]_MainTex("Base (RGB)",2D) = "white"{}
		_TintColor("Tint 颜色",Color) = (1,1,1,1)
		_ColorSclaer("亮度",Float) = 1.0
		_RotateSpeed("旋转速度",Range(-200,200)) = 1

		[Space(30)]
		_Size("内圈大小",Range(0.1,1)) = 0.5
		[NoScaleOffset]_MainTex2("Base (RGB)",2D) = "white"{}
		_TintColor2("Tint 颜色",Color) = (1,1,1,1)
		_ColorSclaer2("亮度",Float) = 1.0
		_RotateSpeed2("旋转速度",Range(-200,200)) = 1

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

				#pragma multi_compile _DEBUG_OFF _DEBUG_ON
				#pragma vertex vert
				#pragma fragment frag

				UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
				float4x4 _ClipToLocal;

				sampler2D _MainTex;
				float _ColorSclaer;
				half4 _TintColor;
				float _RotateSpeed;

				float _Size;
				sampler2D _MainTex2;
				float _ColorSclaer2;
				half4 _TintColor2;
				float _RotateSpeed2;

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
				half2 uv;
				//The Cube mesh-----Edge---Length is 1. so (-x,-y,-z) is (-0.5,-0.5,-0.5)
				//and the (x,y,z) is (0.5,0.5,0.5)--------convert it to (0,1)---> add 0.5 
				uv.x = (localPos.x  + 0.5);
				uv.y = (localPos.z  + 0.5);
				uv = RotatePositionAround(uv, float2(0.5f, 0.5f), _Time * _RotateSpeed);	
				color = tex2D(_MainTex, uv);
				color.rgb *= (_TintColor * _ColorSclaer);

				half4 color2 = half4(0,0,0,0);
				uv.x = (localPos.x / _Size + 0.5);
				uv.y = (localPos.z / _Size + 0.5);
				uv = RotatePositionAround(uv, float2(0.5f, 0.5f), _Time * _RotateSpeed2);
				color2 = tex2D(_MainTex2, uv);
				color2.rgb *= (_TintColor2 * _ColorSclaer2);

				color += color2;

#if defined(_DEBUG_ON)
				if (abs(localPos.x) > 0.45f || abs(localPos.y) > 0.45f || abs(localPos.z) > 0.45f)
				{
					color = half4(1, 0, 0, 1);
				}
#else
				color.a = (color.r + color.g + color.b) / 3.0f;
#endif
				return color;
			}

			ENDCG

		}
	}
}
