Shader "V/Specular" {
	Properties{
		[KeywordEnum(On,Off)] _VertexLight("定点灯光",Float) = 0

		_MainTex("Albedo",2D) = "white"{}
		_Tint("Tint Color",Color) = (1,1,1,1)
		_LightIntensity("Light Intensity",Range(0,2)) = 1
		_Smoothness("Smoothness",Range(0,1)) = 0.001
		_SpecularIntensity("SpecularIntensity",Range(0,1)) = 1.0
		_MinDiffuse("Min Diffuse",Range(0.001,0.2)) = 0.03
	}

	SubShader
	{
		Tags{
			"RenderType" = "Opaque"
		}

		LOD 200

		Pass
		{
			Tags{
				"LightMode" = "ForwardBase"   // forward rendering path
			}

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#pragma multi_compile_fwdadd
			#pragma multi_compile _VERTEXLIGHT_ON _VERTEXLIGHT_OFF
			#pragma multi_compile _SH_ON _SH_OFF

			#define FORWARD_BASE_PASS

			#pragma vertex vert
			#pragma fragment frag
			


			sampler2D _MainTex;
			half4 _MainTex_ST;
			half3 _Tint;
			half _Smoothness;
			half _MinDiffuse;
			half _SpecularIntensity;
			half _LightIntensity;


			struct VertexData
			{
				float4 vertex	: POSITION;
				half3  normal   : NORMAL;
				half2  texcoord : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				half3 normal    : TEXCOORD0;
				half2 uv        : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				#if defined(_VERTEXLIGHT_ON)
					float3 vertexLightColor : TEXCOORD3;
				#endif
			};

			void ComputeVertexLightColor(VertexData v,inout Interpolators i)
			{
				#if defined(_VERTEXLIGHT_ON)
					i.vertexLightColor = Shade4PointLights(
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb,
						unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, i.worldPos, i.normal
					);
				#endif

			}

			Interpolators vert(VertexData v)
			{
				Interpolators o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				ComputeVertexLightColor(v, o);
				return o;
			}

			half4 frag(Interpolators i) : SV_TARGET
			{
				i.normal = normalize(i.normal);      //for non-unit-Length Vector,after Interpolation, will cause non-unit normal
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				
				half3 lightColor = _LightColor0.rgb * _LightIntensity;

				half3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint;
				half diffuseTerm = saturate(dot(lightDir, i.normal)); 
				diffuseTerm = max(_MinDiffuse, diffuseTerm);
				half3 diffuse = albedo * diffuseTerm * lightColor;

				float3 lightRay = -lightDir;
				float3 reflectRay = reflect(lightRay, i.normal);

				half reflectIntensity = saturate(dot(reflectRay, viewDir)) * _SpecularIntensity;
				reflectIntensity = pow(reflectIntensity, _Smoothness * 100);
				half3 specular = lightColor * reflectIntensity;

				#if defined(_VERTEXLIGHT_ON)
					diffuse += i.vertexLightColor;
				#endif



				diffuse *= (1 - reflectIntensity);
				

				half3 color = diffuse + specular; 

				return half4(color,1.0);
			}

			ENDCG
		}	
	}
}
