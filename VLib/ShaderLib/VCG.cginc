// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_CG_)
#define _V_CG_



struct VertexData_base
{
	float4 vertex  : POSITION;
	half3 normal   : NORMAL;
	half4 texcoord : TEXCOORD0;
};

struct VertexData_tan
{
	float4 vertex  : POSITION;
	half3 normal   : NORMAL;
	half4 tangent  : TANGENT;
	half4 texcoord : TEXCOORD0;
};

//struct Interpolators
//{
//	float4 pos : SV_POSITION;
//	float2 uv : TEXCOORD0;
//	float3 normal : TEXCOORD1;
//	float3 worldPos : TEXCOORD2;
//	float4 screenPos : TEXCOORD5;
//	UNITY_FOG_COORDS(6)
//};


#define V_PI			3.14159265359f
#define V_TWO_PI		6.28318530718f
#define V_FOUR_PI		12.56637061436f
#define V_INV_PI		0.31830988618f
#define V_INV_TWO_PI    0.15915494309f
#define V_INV_FOUR_PI   0.07957747155f
#define V_HALF_PI		1.57079632679f
#define V_INV_HALF_PI   0.636619772367f

#ifdef UNITY_COLORSPACE_GAMMA
#define V_ColorSpaceGrey fixed4(0.5, 0.5, 0.5, 0.5)
#define V_ColorSpaceDouble fixed4(2.0, 2.0, 2.0, 2.0)
#define V_ColorSpaceDielectricSpec half4(0.220916301, 0.220916301, 0.220916301, 1.0 - 0.220916301)
#define V_ColorSpaceLuminance half4(0.22, 0.707, 0.071, 0.0) // Legacy: alpha is set to 0.0 to specify gamma mode
#else // Linear values
#define V_ColorSpaceGrey fixed4(0.214041144, 0.214041144, 0.214041144, 0.5)
#define V_ColorSpaceDouble fixed4(4.59479380, 4.59479380, 4.59479380, 2.0)
#define V_ColorSpaceDielectricSpec half4(0.04, 0.04, 0.04, 1.0 - 0.04) // standard dielectric reflectivity coef at incident angle (= 4%)
#define V_ColorSpaceLuminance half4(0.0396819152, 0.458021790, 0.00609653955, 1.0) // Legacy: alpha is set to 1.0 to specify linear mode
#endif


#endif