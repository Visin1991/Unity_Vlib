Shader "V/Water2_1" {
	Properties{

		[KeywordEnum(No,Yes)] _Distortion("水底变形处理",Float) = 0
		_DistortionIntencity("水底变形",Range(0.01,1)) = 0.07

		[Space(30)]
		_ShallowWater("浅水颜色",Color) = (1,1,1,1)
		_DeepWater("深水颜色",Color) = (1,1,1,1)
		_DeepthFadeOut("深水深度值",Range(0.0001,100)) = 3
		_SoftEdge("边缘柔和深度",Range(0,5)) = 1
		[Space(20)]
		_Opacity("水质Opacity",Range(0.01,1)) = 1
		[Space]
		_NoiseIntensity("噪音强度",Range(0,0.02)) = 0.02
		_ReflectionIntensity("反射强度",Range(0.01,1)) = 0.8

		[Space(20)]
		[NoScaleOffset]_Normal1("Normal 1",2D) = "bump"{}
		_TileScale1("Tile 比例 1",Range(0.001,20)) = 1
		_NormalScale1("Normal 强度1",Range(-2,2)) = 1
		_SpeedX_1("X 水流速度 1",Range(-5,5)) = 1
		_SpeedZ_1("Z 水流速度 1",Range(-5,5)) = 1
		[Space(20)]
		[NoScaleOffset]_Normal2("Normal 2",2D) = "bump"{}
		_TileScale2("Tile 比例 2",Range(0.001,20)) = 1
		_NormalScale2("Normal 强度 2",Range(-2,2)) = 1
		_SpeedX_2("X 水流速度 1",Range(-5,5)) = 1
		_SpeedZ_2("Z 水流速度 1",Range(-5,5)) = 1

		_Smoothness("Smoothness", Range(0, 1)) = 0.1

		[NoScaleOffset]_CubeMap("Cube Map", CUBE) = ""		
	}

	SubShader{
		
		Tags{
			"RenderType" = "Transparent"
		}

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
		{
			"_BackgroundTexture"
		}

		Pass
		{
			Tags{
				"LightMode" = "ForwardBase"
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			Cull Off

			CGPROGRAM
			//---------------------------------Pre-Process----------------------------------------
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile _ LINEAR_SPACE_GAMMA_OUTPUT
			#pragma multi_compile _DISTORTION_NO _DISTORTION_YES
			#pragma vertex vert
			#pragma fragment frag

			//#undef SHADER_TARGET
			//#define SHADER_TARGET 30

			#include "UnityPBSLighting.cginc"
			#include "../ShaderLib/VCamera.cginc"
			#include "../ShaderLib/VLinearMath.cginc"
			#include "../ShaderLib/VRandom.cginc"
			#include "../ShaderLib/VUtil.cginc"

			//----------------------------------Variables------------------------------------------
			half3 _ShallowWater;
			half3 _DeepWater;
			half _NoiseIntensity;
			half _DeepthFadeOut;
			half _SoftEdge;
			half _Opacity;
			half _ReflectionIntensity;

			sampler2D _Normal1;
			half _TileScale1;
			half _NormalScale1;
			half _SpeedX_1;
			half _SpeedZ_1;

			sampler2D _Normal2;
			half _TileScale2;
			half _NormalScale2;
			half _SpeedX_2;
			half _SpeedZ_2;

			half _Smoothness;
			samplerCUBE _CubeMap;
			half _DistortionIntencity;

			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			sampler2D _BackgroundTexture;

		//-----------------------------------Structures-----------------------------------------
			struct VertexData
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				half3 normal : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_FOG_COORDS(6)
			};

			//-----------------------------------Functions-----------------------------------------
			UnityLight CreateLight(Interpolators i)
			{
				UnityLight light;
				#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
					light.dir = normalize(_WorldSpaceLightPos0 - i.worldPos);
				#else
					light.dir = _WorldSpaceLightPos0.xyz;
				#endif
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

			void ComputeWaterNormal(inout Interpolators i, half scaler)
			{
				//Normal
				half2 norUv = half2(0,0);
				norUv.x = i.worldPos.x * _TileScale1 / 10 + _SpeedX_1 * _Time;
				norUv.y = i.worldPos.z * _TileScale1 / 10 + _SpeedZ_1 * _Time;

				half2 detalUV = half2(0,0);
				detalUV.x = i.worldPos.x * _TileScale2 / 10 + _SpeedX_2 * _Time;
				detalUV.y = i.worldPos.z * _TileScale2 / 10 + _SpeedZ_2 * _Time;


				half3 normal1 = V_UnpackScaleNormal(tex2D(_Normal1, norUv), _NormalScale1 * scaler);
				half3 normal2 = V_UnpackScaleNormal(tex2D(_Normal2, detalUV), _NormalScale2 * scaler);

				i.normal = BlendNormals(normal1, normal2);
				i.normal = i.normal.xzy;	
			}

			void LinearSpaceGammaOutput(inout half4 color) {
				#if !defined(UNITY_COLORSPACE_GAMMA) && defined(LINEAR_SPACE_GAMMA_OUTPUT)
					color.xyz = pow(color.xyz, 0.454545);
				#endif
			}

			half4 Rendering(Interpolators i)
			{
				//Screen Pos
				i.screenPos.xyz /= i.screenPos.w;

				//Ground Eye Depth & Surface Eye Depth
				half groundEyeDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.screenPos.xy));
				half surfEyeDepth = LinearEyeDepth(i.pos.z);

				//Water Ray Length /  Water Ray Factor
				half waterRayLength = groundEyeDepth - surfEyeDepth;
				half waterRayFactor = max(0.001,saturate(waterRayLength / _DeepthFadeOut));
				half transparentFactor = max(0.001, saturate(waterRayLength / _SoftEdge));


				//Compute Normal
				i.normal = normalize(i.normal);
				ComputeWaterNormal(i, waterRayFactor);

				//Reflection Index
				half3 w2cVector = _WorldSpaceCameraPos - i.worldPos;
				half3 w2cDir = normalize(w2cVector);
				half viewAngleO1 = V_ToCamera_Normal_Angle_01(w2cDir, i.normal);
		
				//Infinitesimal calculus
				half random = ((V_Rand(i.worldPos.xz + i.normal.xz) * 2) -1) * _NoiseIntensity;
				_DeepWater += half3(random, random, random);

				//Water Albedo
				half3 albedo;
				albedo = lerp(_ShallowWater, _DeepWater, waterRayFactor);
				
				//Refrac Color
#if defined(_DISTORTION_YES)
				//--------------------------Distortion Version-----------------
				half2 totalDistortion = half2(0, 0);
				half3 distortionNormal = i.normal * _DistortionIntencity;
				totalDistortion.x = distortionNormal.x;
				totalDistortion.y = distortionNormal.z;
				half2 refracCoord = i.screenPos.xy + totalDistortion;
				half3 refracColor = tex2D(_BackgroundTexture, refracCoord);
				//--------------------------Non-Distortion Version-----------------
#else
				half3 refracColor = tex2D(_BackgroundTexture, i.screenPos.xy);
#endif
				//Merge albedo
				albedo = lerp(refracColor, albedo, waterRayFactor);

				//Reflection Color

				half3 reflectViewDir = V_Reflect_Inverse_World_To_Camera_Dir(normalize(w2cVector), i.normal);
				half3 reflectCol = texCUBE(_CubeMap, reflectViewDir) * _ReflectionIntensity;

				//Merge .............
				albedo = lerp(reflectCol, albedo, viewAngleO1);

				half3 specularTint;
				half oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(albedo, 0, specularTint, oneMinusReflectivity);

				half4 color = BRDF1_Unity_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, w2cDir,
					CreateLight(i),
					CreateIndirectLight(i, reflectCol)
				);

				UNITY_APPLY_FOG(i.fogCoord, color);

				//Transparent Factor
				color.a = lerp(0, color.a, transparentFactor);
				color.a *= _Opacity;
				LinearSpaceGammaOutput(color);
				return color;
			}

			//-----------------------------------VS & PS-----------------------------------------
			Interpolators vert(VertexData v)
			{
				Interpolators i;
				UNITY_INITIALIZE_OUTPUT(Interpolators, i);
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				i.pos = mul(UNITY_MATRIX_VP, i.worldPos);
				i.screenPos = V_ComputeScreenPos(i.pos);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = v.uv;
				UNITY_TRANSFER_FOG(i, i.pos);
				return i;
			}

			float4 frag(Interpolators i) : SV_TARGET
			{
				return Rendering(i);
			}

		ENDCG
		}
	}
}
