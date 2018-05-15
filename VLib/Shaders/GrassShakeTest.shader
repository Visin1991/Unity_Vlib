Shader "V/Grass Test" {

Properties {
    _Cutoff("Cutoff", Range(0.000000, 1.000000)) = 1.000000
    _EmissionColor("EmissionColor", Color) = (0.000000, 0.000000, 0.000000, 0.000000)
    _GrassRatio("GrassRatio", Range(0.000000, 1.000000)) = 1.000000
    _Lightmap("Lightmap", Vector) = (0.000000, 0.000000, 0.000000, 0.000000)
    _MainTex("MainTex", 2D) = "white" {}
    _Metallic("Metallic", Range(0.000000, 1.000000)) = 0.000000
    _Smoothness("Smoothness", Range(0.000000, 1.000000)) = 0.199951
    [HideInInspector] _SnowSizeMultiply("SnowSizeMultiply", Range(0.005001, 1.000000)) = 0.250000
    _SnowThreshold("SnowThreshold", Range(0.000000, 1.000000)) = 0.099976
    _Translucency("Translucency", Range(0.000000, 1.000000)) = 0.199951
    _WrappedNdotLScale("WrappedNdotLScale", Range(0.000000, 1.000000)) = 0.399902

	_WindDirection("WindDirection",Vector) = (1,1,1,1)
	_CollisionRange("Collision Range",Range(0,2)) = 1
	_ShakeIntensity("Shake Intensity",Range(0.01,2)) = 1
	//_PlayerPosition("_PlayerPosition",Vector) = (0,0,0,0)
}

SubShader {
Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
LOD 100

Cull Off

Pass {
Tags { "LightMode" = "ForwardBase" }

CGPROGRAM

#pragma multi_compile _ DEPTH_GENERATION_MRT
#pragma multi_compile _ LINEAR_SPACE_GAMMA_OUTPUT
#pragma multi_compile WEATHER_DEFAULT WEATHER_RAIN WEATHER_SNOW
#pragma multi_compile_instancing
#pragma vertex vert
#pragma fragment frag

#define UNITY_PASS_FORWARDBASE

#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

//#define STAR_GAMMA_TEXTURE
//#define LINEAR_SPACE_GAMMA_OUTPUT

void UnityBRDF1(half lh, half nh, half nl, half nv, half perceptualRoughness, half perceptualSmoothness, half roughness, half3 specularity, out half3 specular) {
#if UNITY_BRDF_GGX
    half V = SmithJointGGXVisibilityTerm(nl, nv, roughness);
    half D = GGXTerm(nh, roughness);
#else
    half V = SmithBeckmannVisibilityTerm(nl, nv, roughness);
    half D = NDFBlinnPhongNormalizedTerm(nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
#endif

    half specularTerm = V * D * UNITY_PI;

#if defined(UNITY_COLORSPACE_GAMMA) && !defined(STAR_GAMMA_TEXTURE)
    specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

    specularTerm = max(0, specularTerm * nl);
    // specularTerm *= any(specularity) ? 1.0 : 0.0;

    specular = specularTerm * FresnelTerm (specularity, lh);
}

void UnityBRDF2(half lh, half nh, half nl, half nv, half perceptualRoughness, half perceptualSmoothness, half roughness, half3 specularity, out half3 specular) {
#if UNITY_BRDF_GGX
    half a = roughness;
    half a2 = a*a;

    half d = nh * nh * (a2 - 1.h) + 1.00001h;
#if defined(UNITY_COLORSPACE_GAMMA) && !defined(STAR_GAMMA_TEXTURE)
    half specularTerm = a / (max(0.32h, lh) * (1.5h + roughness) * d);
#else
    half specularTerm = a2 / (max(0.1h, lh*lh) * (roughness + 0.5h) * (d * d) * 4);
#endif

#if defined (SHADER_API_MOBILE)
    specularTerm = specularTerm - 1e-4h;
#endif

#else
    half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
    half invV = lh * lh * perceptualSmoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
    half invF = lh;
    half specularTerm = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);

#if defined(UNITY_COLORSPACE_GAMMA) && !defined(STAR_GAMMA_TEXTURE)
    specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

    specular = specularTerm * specularity * nl;
}


#ifdef WEATHER_DEFAULT

struct v2f {
    float4 pos : SV_Position;
    float2 uv : TEXCOORD0;
    half3 worldNormal : TEXCOORD1;
    half3 worldViewDir : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(half4, _Lightmap)
UNITY_INSTANCING_BUFFER_END(Props)

half4 _WindDirection;
float4 _PlayerPosition;
half _CollisionRange;
half _ShakeIntensity;



void GrassShakeSimple(float2 uv, out float4 clipPos, inout float3 worldPos) {
	
	_WindDirection.w += 0.01;
	//Get Obstical position.......
	//1. detect if the grass in side the collision range
	half3 distance = worldPos - _PlayerPosition;
	half2 playerForce = half2(distance.x,distance.z);

	half hLength = length(playerForce);
	float mask = step(distance.y, 3);
	mask *= step(hLength, 3 * _CollisionRange);

	worldPos.x += playerForce.x * mask * uv.y * _CollisionRange  * (3 - hLength) / 3 ;
	worldPos.z += playerForce.y * mask * uv.y * _CollisionRange  * (3 - hLength) / 3;
	worldPos.y -= mask * uv.y * _CollisionRange * (3 - hLength) / 3 ;
	//-------------------------------------------------------------


	//风力周期变化
	float intensityRand = sin(_Time.y + worldPos.x) + sin(1.4 * _Time.y + worldPos.x) + sin(2.1 * _Time.y + worldPos.x) + sin(3.24 * _Time.y + worldPos.x);
	intensityRand /= 5;
	intensityRand += 0.8;
	intensityRand /= (15523* _WindDirection.w + 0.001);
	_WindDirection.w += intensityRand;

	//float partA = _MainWindAmptitude / ((0.14f) * sqrt(3.141592654f));
	//float powerValue = -(sin(_MainWindFrequency*(_Time.y + worldPos.x)) / 0.14f) * (sin(_MainWindFrequency*(_Time.y + worldPos.x)) / 0.14f);
	//float WindSinValue = partA * pow(2.718281828f, powerValue);
	//_WindDirection.w += WindSinValue;

	_WindDirection.xyz = normalize(_WindDirection.xyz);
	float sinValue =  sin(_Time.y *(_WindDirection.w * _WindDirection.w + 0.5) + worldPos.z) + sin(_Time.y*(_WindDirection.w * _WindDirection.w + 0.5) + worldPos.x);
	sinValue /= 2;

	//风向周期小幅度变化
	half windDirection_X = _WindDirection.x + (sinValue * 0.1);
	half windDirection_Z = _WindDirection.z + (cos(sinValue) *0.1);
	half2 windDirection2D = normalize(half2(windDirection_X, windDirection_Z));

	//随风小幅晃动....
	half ridEffect = (-(0.5*_WindDirection.w - 1) * (0.5*_WindDirection.w - 1) + 1);
	ridEffect = min(0.3, ridEffect);

	half x_offset = windDirection2D.x * ridEffect  * uv.y * _ShakeIntensity;
	half z_offset = windDirection2D.y * ridEffect  * uv.y * _ShakeIntensity;

	

	half xzOffset = x_offset + z_offset;
	xzOffset /= 2;
	xzOffset *= 0.2f;

	half3 ridDir = half3(x_offset, xzOffset, z_offset);
	worldPos += ridDir * uv.y * (sinValue);

	worldPos.x += _WindDirection.x * _WindDirection.w * uv.y * 0.5 * _ShakeIntensity;
	worldPos.z += _WindDirection.z * _WindDirection.w * uv.y * 0.5 * _ShakeIntensity;
	worldPos.y -= abs(_WindDirection.w) * uv.y * 0.2 * _ShakeIntensity;
	
    clipPos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
}

v2f vert (appdata_full v) {
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);

    UNITY_SETUP_INSTANCE_ID(v);
    float4 vertex = v.vertex;
    float3 normal = v.normal;
    float2 uv = v.texcoord;

    half3 worldNormal;
    worldNormal = UnityObjectToWorldNormal(normal);

    float3 worldPos;
    worldPos = mul(unity_ObjectToWorld, vertex).xyz;

    float4 clipPos;
    GrassShakeSimple(uv, clipPos, worldPos);

    half3 worldViewDir;
#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    worldViewDir = normalize(_WorldSpaceCameraPos - worldPos);
#else
    worldViewDir = _WorldSpaceCameraPos - worldPos;
#endif

    o.pos = clipPos;
    o.uv = uv;
    o.worldNormal = worldNormal;
    o.worldViewDir = worldViewDir;
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    return o;
}

half _Cutoff;
fixed4 _EmissionColor;
half _GrassRatio;
sampler2D _MainTex;
half _Translucency;
half _WrappedNdotLScale;

void AlphaTest(half transparency) {
    clip (transparency - _Cutoff);
}

void GammaCompression(inout half4 color) {
#ifdef UNITY_COLORSPACE_GAMMA
#ifdef STAR_GAMMA_TEXTURE
    color.xyz = pow(color.xyz, 0.454545);
#endif 
#endif
}

void GrassTranslucency(half3 lightDir, half3 worldNormal, half3 worldViewDir, half wrappedNdotL, out half3 lightScattering) {
    half3 transLightDir = lightDir + worldNormal * 0.01;
    half transDot = dot(-transLightDir, worldViewDir); // sign(minus) comes from eyeVec
    transDot = exp2(saturate(transDot) * 2 - 2);
    lightScattering = transDot * lerp(1.0 - wrappedNdotL, 1.0, _GrassRatio);
    lightScattering = 4.0 * _Translucency * lightScattering;
}

void LinearSpaceGammaOutput(inout half4 color) {
#if !defined(UNITY_COLORSPACE_GAMMA) && defined(LINEAR_SPACE_GAMMA_OUTPUT)
    color.xyz = pow(color.xyz, 0.454545);
#endif
}

void UnpackAlbedo(half4 albedo_transparency, out half3 albedo) {
#if defined(UNITY_COLORSPACE_GAMMA) && defined(STAR_GAMMA_TEXTURE)
    albedo = pow(albedo_transparency.xyz, 2.2);
#else
    albedo = albedo_transparency.xyz;
#endif
}

#if defined(DEPTH_GENERATION_MRT)
void frag(v2f IN, out half4 color: SV_Target0, out float depthBuffer: SV_Target1) { 
#else
void frag(v2f IN, out half4 color: SV_Target0) { 
#endif
    half3 worldNormal = IN.worldNormal;
    float4 clipPos = IN.pos;
    float2 uv = IN.uv;
    half3 worldViewDir = IN.worldViewDir;
    UNITY_SETUP_INSTANCE_ID(IN);

    half4 albedo_transparency;
    albedo_transparency = tex2D(_MainTex, uv);

    half transparency;
    transparency = albedo_transparency.w;

    AlphaTest(transparency);

    half3 lightDir;
    lightDir = _WorldSpaceLightPos0.xyz;

    half3 albedo;
    UnpackAlbedo(albedo_transparency, albedo);

    half ndotl;
    ndotl = dot(worldNormal, lightDir);

#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    // do nothing, already normalized in vertex shader
#else
    worldViewDir = normalize(worldViewDir);
#endif

    half wrappedNdotL;
    wrappedNdotL = saturate((ndotl + _WrappedNdotLScale) /
        ((1 + _WrappedNdotLScale) * (1 + _WrappedNdotLScale)));

    half3 lightScattering;
    GrassTranslucency(lightDir, worldNormal, worldViewDir, wrappedNdotL, lightScattering);

    half3 diffuse;
    diffuse = wrappedNdotL;

    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
    gi.indirect.diffuse = 1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).xyz;
    gi.indirect.specular = 0;
    gi.light.color = _LightColor0.rgb * (1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).w);
    gi.light.dir = lightDir;

    color.xyz = albedo * (_EmissionColor.xyz + gi.indirect.diffuse + gi.light.color * (diffuse + lightScattering));
    color.w = 1.0f;

    GammaCompression(color);

    LinearSpaceGammaOutput(color);

    float depthCompressed;
    depthCompressed = clipPos.z;


#if defined(DEPTH_GENERATION_MRT)
    depthBuffer = depthCompressed;
#endif
}

#endif // WEATHER_DEFAULT

#ifdef WEATHER_RAIN

struct v2f {
    float4 pos : SV_Position;
    float2 uv : TEXCOORD0;
    half3 worldNormal : TEXCOORD1;
    half3 worldViewDir : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(half4, _Lightmap)
UNITY_INSTANCING_BUFFER_END(Props)
half3 _ForceShakeVector;
half3 _NormalShakeVector;
half _ShakeTime;

void GrassShakeSimple(float2 uv, out float4 clipPos, inout float3 worldPos) {
    worldPos += _NormalShakeVector.xyz * uv.y * _SinTime.w * _ShakeTime +
    _ForceShakeVector.xyz * uv.y * 0.3f;
    clipPos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
}

v2f vert (appdata_full v) {
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);

    UNITY_SETUP_INSTANCE_ID(v);
    float4 vertex = v.vertex;
    float3 normal = v.normal;
    float2 uv = v.texcoord;

    half3 worldNormal;
    worldNormal = UnityObjectToWorldNormal(normal);

    float3 worldPos;
    worldPos = mul(unity_ObjectToWorld, vertex).xyz;

    float4 clipPos;
    GrassShakeSimple(uv, clipPos, worldPos);

    half3 worldViewDir;
#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    worldViewDir = normalize(_WorldSpaceCameraPos - worldPos);
#else
    worldViewDir = _WorldSpaceCameraPos - worldPos;
#endif

    o.pos = clipPos;
    o.uv = uv;
    o.worldNormal = worldNormal;
    o.worldViewDir = worldViewDir;
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    return o;
}

half _Cutoff;
fixed4 _EmissionColor;
half _GrassRatio;
sampler2D _MainTex;
half _Metallic;
half _Smoothness;
half _Translucency;
half _WeatherControl;
half _WetScale;
half _WrappedNdotLScale;

void AlphaTest(half transparency) {
    clip (transparency - _Cutoff);
}

void GammaCompression(inout half4 color) {
#ifdef UNITY_COLORSPACE_GAMMA
#ifdef STAR_GAMMA_TEXTURE
    color.xyz = pow(color.xyz, 0.454545);
#endif 
#endif
}

void GrassTranslucency(half3 lightDir, half3 worldNormal, half3 worldViewDir, half wrappedNdotL, out half3 lightScattering) {
    half3 transLightDir = lightDir + worldNormal * 0.01;
    half transDot = dot(-transLightDir, worldViewDir); // sign(minus) comes from eyeVec
    transDot = exp2(saturate(transDot) * 2 - 2);
    lightScattering = transDot * lerp(1.0 - wrappedNdotL, 1.0, _GrassRatio);
    lightScattering = 4.0 * _Translucency * lightScattering;
}

void LinearSpaceGammaOutput(inout half4 color) {
#if !defined(UNITY_COLORSPACE_GAMMA) && defined(LINEAR_SPACE_GAMMA_OUTPUT)
    color.xyz = pow(color.xyz, 0.454545);
#endif
}

void UnpackAlbedo(half4 albedo_transparency, out half3 albedo) {
#if defined(UNITY_COLORSPACE_GAMMA) && defined(STAR_GAMMA_TEXTURE)
    albedo = pow(albedo_transparency.xyz, 2.2);
#else
    albedo = albedo_transparency.xyz;
#endif
}

void WetMaterial(half metallic, float2 uv, inout half3 albedo, inout half perceptualSmoothness) {
    //half rain_mask = tex2D(WetMask, uv);
    //half rain_mask = occlusion;
    half rain_mask = _WeatherControl;
    perceptualSmoothness += _WetScale * rain_mask;
    perceptualSmoothness = min(1.0f, perceptualSmoothness);
    half porosity = saturate(((1 - perceptualSmoothness) - 0.5) * 10.f );
    float factor = lerp(0.15, 1, metallic * porosity);
    albedo *= saturate(lerp(1, factor * 5, rain_mask));
}

#if defined(DEPTH_GENERATION_MRT)
void frag(v2f IN, out half4 color: SV_Target0, out float depthBuffer: SV_Target1) { 
#else
void frag(v2f IN, out half4 color: SV_Target0) { 
#endif
    half3 worldNormal = IN.worldNormal;
    float4 clipPos = IN.pos;
    float2 uv = IN.uv;
    half3 worldViewDir = IN.worldViewDir;
    UNITY_SETUP_INSTANCE_ID(IN);

    half4 albedo_transparency;
    albedo_transparency = tex2D(_MainTex, uv);

    half transparency;
    transparency = albedo_transparency.w;

    AlphaTest(transparency);

    half perceptualSmoothness;
    perceptualSmoothness = _Smoothness;

    half metallic;
    metallic = _Metallic;

    half3 albedo;
    UnpackAlbedo(albedo_transparency, albedo);

    half3 lightDir;
    lightDir = _WorldSpaceLightPos0.xyz;

    WetMaterial(metallic, uv, albedo, perceptualSmoothness);

    half ndotl;
    ndotl = dot(worldNormal, lightDir);

#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    // do nothing, already normalized in vertex shader
#else
    worldViewDir = normalize(worldViewDir);
#endif

    half wrappedNdotL;
    wrappedNdotL = saturate((ndotl + _WrappedNdotLScale) /
        ((1 + _WrappedNdotLScale) * (1 + _WrappedNdotLScale)));

    half3 lightScattering;
    GrassTranslucency(lightDir, worldNormal, worldViewDir, wrappedNdotL, lightScattering);

    half3 diffuse;
    diffuse = wrappedNdotL;

    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
    gi.indirect.diffuse = 1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).xyz;
    gi.indirect.specular = 0;
    gi.light.color = _LightColor0.rgb * (1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).w);
    gi.light.dir = lightDir;

    color.xyz = albedo * (_EmissionColor.xyz + gi.indirect.diffuse + gi.light.color * (diffuse + lightScattering));
    color.w = 1.0f;

    GammaCompression(color);

    LinearSpaceGammaOutput(color);

    float depthCompressed;
    depthCompressed = clipPos.z;


#if defined(DEPTH_GENERATION_MRT)
    depthBuffer = depthCompressed;
#endif
}

#endif // WEATHER_RAIN

#ifdef WEATHER_SNOW

struct v2f {
    float4 pos : SV_Position;
    half4 tspace0 : TEXCOORD0;
    half4 tspace1 : TEXCOORD1;
    half4 tspace2 : TEXCOORD2;
    float2 uv : TEXCOORD3;
    float3 worldPos : TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_DEFINE_INSTANCED_PROP(half4, _Lightmap)
UNITY_INSTANCING_BUFFER_END(Props)
half3 _ForceShakeVector;
half3 _NormalShakeVector;
half _ShakeTime;

void GrassShakeSimple(float2 uv, out float4 clipPos, inout float3 worldPos) {
    worldPos += _NormalShakeVector.xyz * uv.y * _SinTime.w * _ShakeTime +
    _ForceShakeVector.xyz * uv.y * 0.3f;
    clipPos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
}

void WorldBitangent(float4 tangent, half3 worldNormal, half3 worldTangent, out half3 worldBitangent) {
    half tangentSign = tangent.w * unity_WorldTransformParams.w;
    worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
}

void WorldTangentSpaceWithViewDir(half3 worldBitangent, half3 worldNormal, half3 worldTangent, half3 worldViewDir, out half4 tspace0, out half4 tspace1, out half4 tspace2) {
    tspace0 = float4(worldTangent.x, worldBitangent.x, worldNormal.x, worldViewDir.x);
    tspace1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldViewDir.y);
    tspace2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldViewDir.z);
}

v2f vert (appdata_full v) {
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);

    UNITY_SETUP_INSTANCE_ID(v);
    float4 vertex = v.vertex;
    float4 tangent = v.tangent;
    float3 normal = v.normal;
    float2 uv = v.texcoord;

    half3 worldTangent;
    worldTangent = UnityObjectToWorldDir(tangent.xyz);

    half3 worldNormal;
    worldNormal = UnityObjectToWorldNormal(normal);

    half3 worldBitangent;
    WorldBitangent(tangent, worldNormal, worldTangent, worldBitangent);

    float3 worldPos;
    worldPos = mul(unity_ObjectToWorld, vertex).xyz;

    half3 worldViewDir;
#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    worldViewDir = normalize(_WorldSpaceCameraPos - worldPos);
#else
    worldViewDir = _WorldSpaceCameraPos - worldPos;
#endif

    half4 tspace0;
    half4 tspace1;
    half4 tspace2;
    WorldTangentSpaceWithViewDir(worldBitangent, worldNormal, worldTangent, worldViewDir, tspace0, tspace1, tspace2);

    float4 clipPos;
    GrassShakeSimple(uv, clipPos, worldPos);

    o.pos = clipPos;
    o.tspace0 = tspace0;
    o.tspace1 = tspace1;
    o.tspace2 = tspace2;
    o.uv = uv;
    o.worldPos = worldPos;
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    return o;
}

half _Cutoff;
fixed4 _EmissionColor;
half _GrassRatio;
sampler2D _MainTex;
half _SnowSizeMultiply;
half _SnowThreshold;
half _Translucency;
half _WeatherControl;
half _WrappedNdotLScale;

void AlphaTest(half transparency) {
    clip (transparency - _Cutoff);
}

void GammaCompression(inout half4 color) {
#ifdef UNITY_COLORSPACE_GAMMA
#ifdef STAR_GAMMA_TEXTURE
    color.xyz = pow(color.xyz, 0.454545);
#endif 
#endif
}

void GrassTranslucency(half3 lightDir, half3 worldNormal, half3 worldViewDir, half wrappedNdotL, out half3 lightScattering) {
    half3 transLightDir = lightDir + worldNormal * 0.01;
    half transDot = dot(-transLightDir, worldViewDir); // sign(minus) comes from eyeVec
    transDot = exp2(saturate(transDot) * 2 - 2);
    lightScattering = transDot * lerp(1.0 - wrappedNdotL, 1.0, _GrassRatio);
    lightScattering = 4.0 * _Translucency * lightScattering;
}

void LinearSpaceGammaOutput(inout half4 color) {
#if !defined(UNITY_COLORSPACE_GAMMA) && defined(LINEAR_SPACE_GAMMA_OUTPUT)
    color.xyz = pow(color.xyz, 0.454545);
#endif
}

void SnowEffect(half3 snowNormal, half3 tsNormal, inout half3 localNormal) {
    half snowCoeff = clamp((10.0 * tsNormal.y * tsNormal.y), 0, 1);
    localNormal = lerp(localNormal,
        lerp(snowNormal, localNormal, 0.4),
        lerp(0.f, snowCoeff, _WeatherControl)
    );
}

void SnowLightingSSS(half3 lightDir, half3 snowAlbedo, half snowHint, half snowPerceptualSmoothness, half3 specularity, half transparency, half3 worldNormal, half3 worldRefl, half3 worldViewDir, inout half3 albedo, inout half occlusion, out half snowCoverage) {
    half offset = dot(worldViewDir, worldNormal);
    snowCoverage = 1.0 - clamp(pow(/*SnowVerticalBlending*/1.25 * (1.0 - worldNormal.y), 10.0), 0, 1);

    snowCoverage *= (transparency - _Cutoff + _WeatherControl * 0.5f) / (1.0f - _Cutoff);

    half snowOffset = _WeatherControl * _SnowThreshold;
    half SnowBand = 0.05f;
    half SnowLow = max(0, snowOffset - SnowBand);
    half SnowHigh= min(1, snowOffset + SnowBand);

    snowHint = clamp(snowHint, SnowLow, SnowHigh);

    half snowMask = (SnowHigh - snowHint) / (SnowHigh - SnowLow);

    snowCoverage *= snowMask;
    snowCoverage = min(snowCoverage, 1.0f);

    albedo = lerp(albedo, (albedo * (1.0f - offset) + (1.0f + offset) * snowAlbedo) * 0.5f, snowCoverage);
    occlusion = lerp(occlusion, 1.0f, snowCoverage);
}

void SnowMaterial(float2 uv, out half3 snowAlbedo, out half3 snowNormal, out half snowPerceptualSmoothness) {
    float2 scale = float2(1.0, 1.0) / float2(_SnowSizeMultiply, _SnowSizeMultiply);
    float2 snowUV = uv * scale;
    snowAlbedo = 1.0f; // SnowIntensity;
    snowNormal = half3(0, 0, 1); //UnpackNormal(tex2D(SnowNormal, snowUV));
    snowPerceptualSmoothness = 1.0f;
}

void UnpackAlbedo(half4 albedo_transparency, out half3 albedo) {
#if defined(UNITY_COLORSPACE_GAMMA) && defined(STAR_GAMMA_TEXTURE)
    albedo = pow(albedo_transparency.xyz, 2.2);
#else
    albedo = albedo_transparency.xyz;
#endif
}

void WorldRelf(half3 worldNormal, half3 worldViewDir, out half3 worldRefl) {
    worldRefl = reflect(-worldViewDir, worldNormal);
}

void WorldTangentNormal(half3 localNormal, half4 tspace0, half4 tspace1, half4 tspace2, out half3 worldNormal) {
    worldNormal.x = dot(half3(tspace0.xyz), localNormal);
    worldNormal.y = dot(half3(tspace1.xyz), localNormal);
    worldNormal.z = dot(half3(tspace2.xyz), localNormal);
    worldNormal = normalize(worldNormal);
}

#if defined(DEPTH_GENERATION_MRT)
void frag(v2f IN, out half4 color: SV_Target0, out float depthBuffer: SV_Target1) { 
#else
void frag(v2f IN, out half4 color: SV_Target0) { 
#endif
    half4 tspace0 = IN.tspace0;
    half4 tspace1 = IN.tspace1;
    half4 tspace2 = IN.tspace2;
    float4 clipPos = IN.pos;
    float2 uv = IN.uv;
    UNITY_SETUP_INSTANCE_ID(IN);

    half4 albedo_transparency;
    albedo_transparency = tex2D(_MainTex, uv);

    half transparency;
    transparency = albedo_transparency.w;

    AlphaTest(transparency);

    half3 tsNormal;
    tsNormal = half3(tspace0.z, tspace1.z, tspace2.z);

    half3 localNormal;
    localNormal = half3(0.f, 0.f, 1.f);

    half3 worldViewDir;
    worldViewDir = float3(tspace0.w, tspace1.w, tspace2.w);
#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
    // do nothing, already normalized in vertex shader
#else
    worldViewDir = normalize(worldViewDir);
#endif

    half3 snowAlbedo;
    half3 snowNormal;
    half snowPerceptualSmoothness;
    SnowMaterial(uv, snowAlbedo, snowNormal, snowPerceptualSmoothness);

    half occlusion;
    occlusion = 1.0;

    half3 albedo;
    UnpackAlbedo(albedo_transparency, albedo);

    half3 lightDir;
    lightDir = _WorldSpaceLightPos0.xyz;

    SnowEffect(snowNormal, tsNormal, localNormal);

    half snowHint;
    snowHint = albedo.g; // dot(albedo, half3(0.333, 0.333, 0.333));

    half3 worldNormal;
    WorldTangentNormal(localNormal, tspace0, tspace1, tspace2, worldNormal);

    half ndotl;
    ndotl = dot(worldNormal, lightDir);

    half3 worldRefl;
    WorldRelf(worldNormal, worldViewDir, worldRefl);

    half wrappedNdotL;
    wrappedNdotL = saturate((ndotl + _WrappedNdotLScale) /
        ((1 + _WrappedNdotLScale) * (1 + _WrappedNdotLScale)));

    half3 lightScattering;
    GrassTranslucency(lightDir, worldNormal, worldViewDir, wrappedNdotL, lightScattering);

    half3 diffuse;
    diffuse = wrappedNdotL;

    half3 specularity;
    specularity = half3(0.0f, 0.0f, 0.0f);

    half snowCoverage;
    SnowLightingSSS(lightDir, snowAlbedo, snowHint, snowPerceptualSmoothness, specularity, transparency, worldNormal, worldRefl, worldViewDir, albedo, occlusion, snowCoverage);

    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
    gi.indirect.diffuse = 1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).xyz;
    gi.indirect.specular = 0;
    gi.light.color = _LightColor0.rgb * (1.0 - UNITY_ACCESS_INSTANCED_PROP(Props, _Lightmap).w);
    gi.light.dir = lightDir;

    color.xyz = albedo * ((1.0f - snowCoverage) * _EmissionColor.xyz + gi.indirect.diffuse + gi.light.color * (diffuse + lightScattering));
    color.w = 1.0f;

    GammaCompression(color);

    LinearSpaceGammaOutput(color);

    float depthCompressed;
    depthCompressed = clipPos.z;


#if defined(DEPTH_GENERATION_MRT)
    depthBuffer = depthCompressed;
#endif
}

#endif // WEATHER_SNOW
ENDCG

} // Pass end

Pass {
Tags { "LightMode" = "ShadowCaster" }

ZWrite On

CGPROGRAM

#pragma multi_compile_shadowcaster

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
half _Cutoff;

struct v2f { 
    V2F_SHADOW_CASTER;
    float2 uv : TEXCOORD1;
};

v2f vert(appdata_base v)
{
    v2f o;
    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
    o.uv = v.texcoord;
    return o;
}

float4 frag(v2f i) : SV_Target
{
    half alpha = tex2D(_MainTex, i.uv).a;
    clip(alpha - _Cutoff);

    SHADOW_CASTER_FRAGMENT(i)
}
ENDCG

} // Pass end

} // SubShader end

} // Shader end
