// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "V/Fog2" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FogDistance("Distance", float) = 50
		_FogMaxDistance("FogMaxDis",float) = 300
		_FogHeight("Height", Range(0, 100)) = 1
		_FogFadeOut("FadeOut",Range(0,100)) = 1
		_FogBaseHeight("Baseline Height", float) = 0
		_Intensity("Intensity", Range(0, 5)) = 1
		_Color("Fog Color", Color) = (0.9,0.9,0.9,1.0)
		_Min("Intensity Range",Range(0,1.0)) = 0.9
		_SunTint("Sun Tint",Range(0,1.0)) = 0.5
	}

	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma target 3.0
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include"../ShaderLib/VImageUtil.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D_float _CameraDepthTexture;
			float4x4 _ClipToWorld;

			float _FogDistance;
			float _FogFadeOut;
			float _FogMaxDistance;
			float  _FogHeight, _FogBaseHeight;
			half4 _Color;
			half _Intensity;
			half _Min;
			fixed _SunTint;


			const half4 zeros = half4(0, 0, 0, 0);

			struct appdata {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 clip_near : POSITION;
				float2 uv: TEXCOORD0;
				float3 cameraToFarClipProj_World : TEXCOORD2;
			};

			v2f vert(appdata v) {
				v2f o;
				//Get near clip plane projection Position-------------------2D
				o.clip_near = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.cameraToFarClipProj_World = Get_CameraToFarClipProj_World(o.clip_near, _ClipToWorld);
				return o;
			}

			half4 getFogColor(float2 uv, float3 worldPos,float3 viewRayN, float depth01) {



				float volumetricFogRayLength = 0;
				float fadeOutVolumetricFogRayLength = 0.0;

				float3 cameraToWorld = worldPos - _WorldSpaceCameraPos;
				float3 norC2W = normalize(cameraToWorld);
				float distance = length(cameraToWorld); //Distance of the pixel to Camera in world space.

														//-------------------------------------------------------------------------------

				volumetricFogRayLength = distance - _FogDistance.x;             // subtract the fog start distance.
				volumetricFogRayLength *= step(0, volumetricFogRayLength);          // if the distance small or equal to 0, then distance equal to 0

				float distFactor = saturate(volumetricFogRayLength / _FogMaxDistance);

				float3 planeNormalUp = float3(0,1,0);
				float3 planeNormalDown = float3(0,-1,0);

				_FogHeight += _WorldSpaceCameraPos.y;

				float rayLengthUp = Ray_Plane_Intersection_Distance(planeNormalUp, _FogHeight, _WorldSpaceCameraPos, norC2W);   //Compute the fog length........
				float rayLengthDown = Ray_Plane_Intersection_Distance(planeNormalDown, _FogHeight, _WorldSpaceCameraPos, norC2W);   //Compute the fog length........

																																	//--------------------------------------Calculate the Fog Distance-------------------------------------------
																																	//If the ray go through both Plane---> eigther Heigher than the top of the Fog, or Lower then the botton of the fog
				if (rayLengthUp > 0 && rayLengthDown > 0)
				{
					//I am assuming the ray did not go through the whole fog
					volumetricFogRayLength -= min(rayLengthUp, rayLengthDown);
				}
				else
				{
					float length = rayLengthUp > 0 ? rayLengthUp : rayLengthDown;
					volumetricFogRayLength = volumetricFogRayLength > length ? length : volumetricFogRayLength;
				}

				//----------------------------------------------------------------------------------------------------------


				float factor = (saturate(volumetricFogRayLength * _Intensity / _FogMaxDistance));
				factor = min(factor, _Min);

				float3 viewDir = norC2W;
				float lightReciveAngle = (dot(viewDir, _WorldSpaceLightPos0.xyz) + 1) / 2;

				_Color.rgb += _LightColor0 * lightReciveAngle *_SunTint;

				return float4(_Color.rgb * factor, factor);
			}



			// Fragment Shaders
			half4 frag(v2f i) : COLOR
			{
				//Sample screen RT
				half4 rtColor = tex2D(_MainTex, i.uv);

				float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));

				// Reconstruct the world position of the pixel
				_WorldSpaceCameraPos.y -= _FogBaseHeight;

				//Get the world Position of the pixel based on Depth Value
				float3 worldPos = Get_WorldPos_From_CameraToFarClipProj_World(i.cameraToFarClipProj_World, depth);

				float3 viewRayN = normalize(i.cameraToFarClipProj_World);
				half4 fogColor = getFogColor(i.uv, worldPos, viewRayN, depth);

				rtColor = rtColor * (1.0 - fogColor.a) + fogColor;

				return rtColor;
			}

			ENDCG
		}
	}

}