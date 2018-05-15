// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_LINEARMATH_CG_)
#define _V_LINEARMATH_CG_

#include "VConst.cginc"

///https://www.desmos.com/calculator

inline float Smooth_Transition(float a, float b, float value)
{
	return 1 / (1 + pow(_E, (a - b * value)));
}

inline float Smooth_Transition01(float a, float value)
{
	return 1 / (1 + pow(_E, a - 2*a * value));
}

inline float Smooth_Transition01_B(float a,float b, float value)
{
	return pow(value,a*(value+b));
}

inline float2 CartesianToPolar(float2 xy)
{
	float r = sqrt(xy.x * xy.x + xy.y * xy.y);
	float theta = atan2(xy.y,xy.x);
	return float2(r,theta);
}

inline float2 PolarToCartesian(float2 rt)
{
	return float2(rt.x * cos(rt.y),rt.x * sin(rt.y));
}

inline float2 RotatePositionAround(float2 xy, float2 center,float radius)
{
	xy -= center;
	float2 polar = CartesianToPolar(xy);
	polar.y += radius;
	return PolarToCartesian(polar) + center;
}

float3 V_reflect(float3 i, float3 n)
{
	return i - 2.0 * n * dot(n, i);
}

inline float3 V_ReflectRayDir(float3 ray, float3 normal)
{
	return reflect(ray, normal);
}

#endif