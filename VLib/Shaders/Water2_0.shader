Shader "V/Water2_0" {
	Properties {
		//WTF
		_ShallowWater("浅水颜色",Color) = (1,1,1,1)
		_DeepWater("深水颜色",Color) = (1,1,1,1)
		_Transparency("透明度",Range(0,1)) = 0.12
		_DeepthFadeOut("可见深度",Range(0.01,100)) = 3
		_ReflectionIntensity("反射强度",Range(0.01,1)) = 0.8

		[NoScaleOffset]_Normal1("Normal 1",2D) = "bump"{}
		_TileScale1("Tile 比例 1",Range(0.001,20)) = 1
		_NormalScale1("Normal 强度1",Range(-2.0000001,2.0000001)) = 1
		_SpeedX_1("X 水流速度 1",Range(-5,5)) = 1
		_SpeedZ_1("Z 水流速度 1",Range(-5,5)) = 1

		[NoScaleOffset]_Normal2("Normal 2",2D) = "bump"{}
		_TileScale2("Tile 比例 2",Range(0.001,20)) = 1
		_NormalScale2("Normal 强度 2",Range(-2.0000001,2.0000001)) = 1
		_SpeedX_2("X 水流速度 1",Range(-5,5)) = 1
		_SpeedZ_2("Z 水流速度 1",Range(-5,5)) = 1

		_Smoothness("Smoothness", Range(0, 1)) = 0.1
		
		[Gamma] _Metallic("Metallic",Range(0,1)) = 0
		[NoScaleOffset]_CubeMap("Cube Map", CUBE) = ""
	}

Category{

	Tags{
		"Queue" = "AlphaTest+10"
		"RenderType" = "Transparent"
	}

	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off
	Cull Off

	SubShader{
		Pass
		{
			

			CGPROGRAM
			//---------------------------------Pre-Process----------------------------------------
			#pragma multi_compile_fog
			#pragma multi_compile _ LINEAR_SPACE_GAMMA_OUTPUT
			#pragma vertex vert
			#pragma fragment frag


			//#undef SHADER_TARGET
			//#define SHADER_TARGET 30


			#include "UnityPBSLighting.cginc"
			#include "../ShaderLib/VCamera.cginc"
			#include "../ShaderLib/VUtil.cginc"
			#include "../ShaderLib/VBRDF.cginc"



			
			
			//----------------------------------Variables------------------------------------------
			half3 _ShallowWater;
			half3 _DeepWater;
			half _Transparency;
			half _DeepthFadeOut;
			half _ReflectionIntensity;
			
			sampler2D _Normal1;
			float _TileScale1;
			float _NormalScale1;
			float _SpeedX_1;
			float _SpeedZ_1;

			sampler2D _Normal2;
			float _TileScale2;
			float _NormalScale2;
			float _SpeedX_2;
			float _SpeedZ_2;

			half _Smoothness;
			samplerCUBE _CubeMap;
			half _Metallic;
			//-----------------------------------Structures-----------------------------------------
			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD5;
				UNITY_FOG_COORDS(6)
			};

			//-----------------------------------Functions-----------------------------------------
			UnityLight CreateLight(Interpolators i)
			{
				UnityLight light;
				light.dir = _WorldSpaceLightPos0.xyz;
				//UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
				light.color = _LightColor0.rgb;// *attenuation;
				light.ndotl = DotClamped(i.normal, light.dir);
				return light;
			}
			UnityIndirect CreateIndirectLight(Interpolators i, half3 reflectionColor)
			{
				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;
				
				#if defined(FORWARD_BASE_PASS)
					indirectLight.diffuse += max(0, ShadeSH9(half4(i.normal, 1)));
					indirectLight.specular = reflectionColor * _LightColor0.rgb * _ReflectionIntensity;
				#endif

				return indirectLight;
			}

			void ComputeWaterNormal(inout Interpolators i)
			{  
				float2 norUv = float2(0,0);
				float x1 = _SpeedX_1 * _Time;
				float z1 = _SpeedZ_1 * _Time;
				norUv.x = i.worldPos.x * (_TileScale1 / 10.0000001f) + x1;
				norUv.y = i.worldPos.z * (_TileScale1 / 10.0000001f) + z1;

				float2 detalUV = float2(0,0);
				float x2 = _SpeedX_2 * _Time;
				float z2 = _SpeedZ_2 * _Time;
				detalUV.x = i.worldPos.x * (_TileScale2 / 10.0000001f) + x2;
				detalUV.y = i.worldPos.z * (_TileScale2 / 10.0000001f) + z2;

				float3 normal1 = V_UnpackScaleNormal(tex2D(_Normal1, norUv), _NormalScale1);
				float3 normal2 = V_UnpackScaleNormal(tex2D(_Normal2, detalUV), _NormalScale2);

				i.normal = BlendNormals(normal1, normal2);
				i.normal = i.normal.xzy;
				//i.normal = normal1.xzy;	
			}

			void LinearSpaceGammaOutput(inout half4 color) {
				#if !defined(UNITY_COLORSPACE_GAMMA) && defined(LINEAR_SPACE_GAMMA_OUTPUT)
					color.xyz = pow(color.xyz, 0.454545);
				#endif
			}

			//-----------------------------------VS & PS-----------------------------------------
			Interpolators vert(VertexData v)
			{
				Interpolators i;
				UNITY_INITIALIZE_OUTPUT(Interpolators, i);
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				i.pos = mul(UNITY_MATRIX_VP, half4(i.worldPos, 1.0f));
				i.screenPos = V_ComputeScreenPos(i.pos);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = v.uv;
				UNITY_TRANSFER_FOG(i, i.pos);
				return i;
			}



			half4 frag(Interpolators i) : SV_TARGET
			{
				
				i.screenPos.xy /= i.screenPos.w;
				ComputeWaterNormal(i);

				half3 w2cVec = V_World_To_Camera_Vector(i.worldPos.xyz);
				half3 w2cDir = normalize(w2cVec);
				half3 reflectViewDir = V_Reflect_Inverse_World_To_Camera_Dir(w2cDir, i.normal);
				half reflectAngleFactor = saturate(dot(w2cDir, normalize(i.normal)));

				half3 reflectCol = texCUBE(_CubeMap, reflectViewDir) * _ReflectionIntensity;

				half distance = length(w2cVec);
				half depthFactor = saturate(distance / _DeepthFadeOut);
				half3 albedo = lerp(_ShallowWater, _DeepWater, depthFactor);

				albedo = lerp(reflectCol,albedo, reflectAngleFactor);

				half3 specularTint;
				half oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

				half4 color = BRDF2_Unity_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, w2cDir,
					CreateLight(i),
					CreateIndirectLight(i, reflectCol)
				);
				
				depthFactor = max(0.2f, depthFactor);
				color.a = min(depthFactor,1 - _Transparency);
				UNITY_APPLY_FOG(i.fogCoord, color);

				LinearSpaceGammaOutput(color);
				return color;
			}

			ENDCG
		}
	}

}//End Category

}//End Shader
