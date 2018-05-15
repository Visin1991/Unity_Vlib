// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_BRDF_CG_)
#define _V_BRDF_CG_


#include "UnityStandardConfig.cginc"
#include "UnityLightingCommon.cginc"


half V_PerceptualRoughnessToRoughness(half perceptualRoughness)
{
	return perceptualRoughness * perceptualRoughness;
}

half V_RoughnessToPerceptualRoughness(half roughness)
{
	return sqrt(roughness);
}

half V_SmoothnessToRoughness(half smoothness)
{
	return (1 - smoothness) * (1 - smoothness);
}

half V_SmoothnessToPerceptualRoughness(half smoothness)
{
	return (1 - smoothness);
}

//------------------------------------------------------------
inline half V_Pow4(half x)
{
	return x * x*x*x;
}

inline half2 V_Pow4(half2 x)
{
	return x * x*x*x;
}

inline half3 V_Pow4(half3 x)
{
	return x * x*x*x;
}

inline half4 V_Pow4(half4 x)
{
	return x * x*x*x;
}

// Pow5 uses the same amount of instructions as generic pow(), but has 2 advantages:
// 1) better instruction pipelining
// 2) no need to worry about NaNs
inline half V_Pow5(half x)
{
	return x * x * x*x * x;
}

inline half2 V_Pow5(half2 x)
{
	return x * x * x*x * x;
}

inline half3 V_Pow5(half3 x)
{
	return x * x * x*x * x;
}

inline half4 V_Pow5(half4 x)
{
	return x * x * x*x * x;
}

//-----------------------------------------------------------------------------
inline half3 V_Unity_SafeNormalize(half3 inVec)
{
	half dp3 = max(0.001f, dot(inVec, inVec));
	return inVec * rsqrt(dp3);//反平方根
}

inline float V_SchlickFresnel_Safe(float u)
{
	float m = clamp(1 - u, 0, 1);
	return V_Pow5(m);
}

inline float V_SchlickFresnel(float u)
{
	return V_Pow5(u);
}

inline half V_DisneyDiffuse(half NdotV, half NdotL, half LdotH, half perceptualRoughness)
{
	half FD90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
	half FL = V_SchlickFresnel(NdotL);
	half FV = V_SchlickFresnel(NdotV);
	half lightScatter = (1 + (FD90 - 1) * FL);
	half viewScatter  = (1 + (FD90 - 1) * FV);
	return lightScatter * viewScatter;
}

inline half V_perceptualRoughnessToSpecPower(half perceptualRoughness)
{
	half m = V_PerceptualRoughnessToRoughness(perceptualRoughness);   // m is the true academic roughness.
	half sq = max(1e-4f, m*m);
	half n = (2.0 / sq) - 2.0;                          // https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
	n = max(n, 1e-4f);                                  // prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero
	return n;
}

inline half V_RoughnessToSpecPower(half roughness)
{
	half sq = max(1e-4f, roughness * roughness);
	half n = (2.0 / sq) - 2.0;                          // https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
	n = max(n, 1e-4f);                                  // prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero
	return n;
}

half V_NDFBlinnPhongNormalizedTerm(half NdotH, half n)
{
	//norm = (n+2) / (2 * pi)
	half normTerm = (n + 2.0) * (0.5 / UNITY_PI);
	half specTerm = pow(NdotH, n);
	return specTerm * normTerm;
}

inline half3 V_FresnelLerp(half3 F0, half3 F90, half cosA)
{
	half t = V_Pow5(1 - cosA);   // ala Schlick interpoliation
	return lerp(F0, F90, t);
}

inline half V_GGXTerm(half NdotH, half roughness)
{
	half a2 = roughness * roughness;
	half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
	return UNITY_INV_PI * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
												// therefore epsilon is smaller than what can be represented by half
}

inline half V_SmithJointGGXVisibilityTerm(half NdotL, half NdotV, half roughness)
{
#if 0
	// Original formulation:
	//  lambda_v    = (-1 + sqrt(a2 * (1 - NdotL2) / NdotL2 + 1)) * 0.5f;
	//  lambda_l    = (-1 + sqrt(a2 * (1 - NdotV2) / NdotV2 + 1)) * 0.5f;
	//  G           = 1 / (1 + lambda_v + lambda_l);

	// Reorder code to be more optimal
	half a = roughness;
	half a2 = a * a;

	half lambdaV = NdotL * sqrt((-NdotV * a2 + NdotV) * NdotV + a2);
	half lambdaL = NdotV * sqrt((-NdotL * a2 + NdotL) * NdotL + a2);

	// Simplify visibility term: (2.0f * NdotL * NdotV) /  ((4.0f * NdotL * NdotV) * (lambda_v + lambda_l + 1e-5f));
	return 0.5f / (lambdaV + lambdaL + 1e-5f);  // This function is not intended to be running on Mobile,
												// therefore epsilon is smaller than can be represented by half
#else
	// Approximation of the above formulation (simplify the sqrt, not mathematically correct but close enough)
	half a = roughness;
	half lambdaV = NdotL * (NdotV * (1 - a) + a);
	half lambdaL = NdotV * (NdotL * (1 - a) + a);
	return 0.5f / (lambdaV + lambdaL + 1e-5f);
#endif
}

//尽量减少 ALU 操作
half4 V_BRDF1_Unity_PBS(half3 diffColor, half3 specColor, half oneMinusReflectivity,
						half smoothness, half3 normal, half3 viewDir,
						UnityLight light, UnityIndirect gi)
{
	half perceptualRoughness = V_SmoothnessToPerceptualRoughness(smoothness);
	half3 halfDir = V_Unity_SafeNormalize(light.dir + viewDir);

	// NdotV should not be negative for visible pixels, but it can happen due to perspective projection and normal mapping
	// In this case normal should be modified to become valid (i.e facing camera) and not cause weird artifacts.
	// but this operation adds few ALU and users may not want it. Alternative is to simply take the abs of NdotV (less correct but works too).
	// Following define allow to control this. Set it to 0 if ALU is critical on your platform.
	// This correction is interesting for GGX with SmithJoint visibility function because artifacts are more visible in this case due to highlight edge of rough surface
	// Edit: Disable this code by default for now as it is not compatible with two sided lighting used in SpeedTree.



	half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
	half nl = saturate(dot(normal, light.dir));
	half nh = saturate(dot(normal, halfDir));

	half lv = saturate(dot(light.dir, viewDir));
	half lh = saturate(dot(light.dir, halfDir));

	// Diffuse term
	half diffuseTerm = V_DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;

	// Specular term
	// HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
	// BUT 1) that will make shader look significantly darker than Legacy ones
	// and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
	half roughness = V_PerceptualRoughnessToRoughness(perceptualRoughness);

	// GGX with roughtness to 0 would mean no specular at all, using max(roughness, 0.002) here to match HDrenderloop roughtness remapping.
	roughness = max(roughness, 0.002);
	half V = V_SmithJointGGXVisibilityTerm(nl, nv, roughness);
	half D = V_GGXTerm(nh, roughness);

	//    DVF
	half specularTerm = V * D * UNITY_PI; // Torrance-Sparrow model, Fresnel is applied later

#   ifdef UNITY_COLORSPACE_GAMMA
	specularTerm = sqrt(max(1e-4h, specularTerm));
#   endif

	// specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
	specularTerm = max(0, specularTerm * nl);

	// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
	half surfaceReduction;

#   ifdef UNITY_COLORSPACE_GAMMA
	surfaceReduction = 1.0 - 0.28*roughness*perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
#   else
	surfaceReduction = 1.0 / (roughness*roughness + 1.0);           // fade \in [0.5;1]
#   endif
																// To provide true Lambert lighting, we need to be able to kill specular completely.
	specularTerm *= any(specColor) ? 1.0 : 0.0;

	half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
	half3 color = diffColor * (gi.diffuse + light.color * diffuseTerm)
		+ specularTerm * light.color * FresnelTerm(specColor, lh)
		+ surfaceReduction * gi.specular * V_FresnelLerp(specColor, grazingTerm, nv);

	return half4(color, 1);
}

#endif