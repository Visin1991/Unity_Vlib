// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_RANDOM_CG_)
#define _V_RANDOM_CG_

float V_Rand(float2 co) {
	return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);

}

#endif