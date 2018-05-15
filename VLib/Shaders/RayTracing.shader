// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/RayTracing" {
	Properties
	{
		[KeywordEnum(Off,On)]_Debug("Debug 模式",Float) = 0
		_MainTex("Base (RGB)",2D) = "white"{}
		_ColorSclaer("亮度",Float) = 1.0
	}

		SubShader{
			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				Tags{ "Queue" = "Geometry+50" }

				CGPROGRAM
				#pragma multi_compile _DEBUG_OFF _DEBUG_ON
				#pragma vertex vert
				#pragma fragment frag
				#include "../ShaderLib/VCamera.cginc"
				#include "../ShaderLib/VImageUtil.cginc"
				#include "../ShaderLib/VLinearMath.cginc"


				sampler2D _MainTex;

			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			float4x4 _ClipToWorld;
			float3 _Center;
			float2 _ScaleXZ;
			float _RotationSpeed;
			float _ColorSclaer;

			struct VertexData
			{
				float4 vertex : POSITION;
			};
			
			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float4 clip : TEXCOORD3;
			};
			
			Interpolators vert(VertexData v)
			{
				Interpolators o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.clip = o.pos;
				return o;

			}

			half4 frag(Interpolators i) : COLOR
			{
				float2  screenUV = V_ComputeScreenPos_Ps(i.clip);
				float depth = V_Linear01Depth(tex2D(_CameraDepthTexture, screenUV));
				float3 cam2FarClipProj_World = Get_CameraToFarClipProj_World(i.clip, _ClipToWorld);
				float3 worldPos = Get_WorldPos_From_CameraToFarClipProj_World(cam2FarClipProj_World, depth);
				float2 worldXZ = float2(worldPos.x, worldPos.z);

				half2 uv;
					
				float2 centerXZ = float2(_Center.x, _Center.z);

				float2 bottonLeft = float2(centerXZ.x - _ScaleXZ.x * 0.5f, centerXZ.y- _ScaleXZ.y * 0.5f);
				worldXZ = RotatePositionAround(worldXZ, centerXZ, _RotationSpeed);

				half2 offset = worldXZ - bottonLeft;
				uv.x = offset.x / _ScaleXZ.x;
				uv.y = offset.y / _ScaleXZ.y;

				half4 color;
				if ( 0.0f < uv.x && uv.x <1.0f &&  0.0f < uv.y && uv.y <1.0f)
				{
					color = tex2D(_MainTex, uv);
					#if defined(_DEBUG_ON)
						//Do nothing
					#else
						color.a = (color.r + color.g + color.b) / 3.0f;
						color.rgb *= _ColorSclaer;
					#endif
				}
				else
				{
					#if defined(_DEBUG_ON)
						color = half4(0,1,0,1);
					#else
						color = half4(0,0,0,0);
					#endif
				}

				return color;
			}

			ENDCG

		}
	}
}
