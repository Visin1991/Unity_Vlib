// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_UTIL_CG_)
#define _V_UTIL_CG_

#include "UnityShaderVariables.cginc"

//Note : smoothstep(lower,heigher,input)------> this function return a value between [0,1]
//			when the input smaller than or equal to lower return 0. If it heigher than or equal to
//          heigher it return 1.  if lower < input < heigher------>  return the percentage of the length 
//          between lower and heigher.


//        o.pos = UnityObjectToClipPos(v.vertex);

//--------------unity_ObjectToWorld---------------M Matrix
inline float3 Object_2_WorldNormal(float3 normal)
{
	return mul((float3x3)unity_WorldToObject, normal);
}

inline float3 Object_2_ViewNormal(float3 normal)
{
	return mul((float3x3)UNITY_MATRIX_IT_MV, normal);
}

inline float2 View_2_Projection(float2 i)
{
	return mul((float2x2)UNITY_MATRIX_P, i);
}

inline float3 View_2_Projection(float3 i)
{
	return mul((float3x3)UNITY_MATRIX_P, i);
}

half3 V_UnpackScaleNormal(half4 packednormal, half bumpScale)
{
	half3 normal;
#if defined(UNITY_NO_DXT5nm)
	normal = packednormal.xyz * 2 - 1;
	normal.xy *= bumpScale;
	return normal;
#else
	normal.xy = (packednormal.wy * 2 - 1);
	normal.xy *= bumpScale;
	normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
	return normal;
#endif
}

#endif