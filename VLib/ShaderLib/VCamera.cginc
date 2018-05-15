// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_CAMERA_CG_)
#define _V_CAMERA_CG_

#include "UnityShaderVariables.cginc"

#define Y_FILIPPED_FACTOR	     _ProjectionParams.x
#define NEAR					 _ProjectionParams.y
#define FAR						 _ProjectionParams.z
#define ONE_OVER_FAR			 _ProjectionParams.w

#define SCREEN_WIDTH			 _ScreenParams.x
#define SCRREN_HEIGHT			 _ScreenParams.y



/*-------------------------------------------------------------
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
----------------------------------------------------------------*/

inline float2 V_ComputeScreenPos_Ps(inout float4 nearClip)
{
	nearClip.xyz /= nearClip.w;
	return float2(nearClip.x, nearClip.y *  _ProjectionParams.x) *0.5f + 0.5f;
}


//Compute the Screen Pos at Vertex Shader.
//In Pixesl Shader We need to Project on the ClipSpace with Homogenous coordinates 1.
inline float4 V_ComputeScreenPos(float4 nearClipPos)
{
	float4 o = nearClipPos;
	o.xy = float2(o.x, o.y * _ProjectionParams.x) * 0.5f + o.w * 0.5f;
	return o;
	/*-----------------------Detail----------------------------------------
	When Homogenous coordinates of nearClipPos equal to 1, nearClipPos.xyz will be projected
	Clip space; which x,y,z =>[-1,1]

	Here the nearClipPos.w (Homogenous coordinates) is not equal to 1, it is equal to some whatever number....

	So if we want to project the nearClipPos to [-1,1], then we need to devide the nearClipPos.xyz 
		by the Homogenous coordinates nearClipPos.w.
	
	It lead the equation to be:
		o.xy = float2(o.x, o.y * _ProjectionParams.x) / o.w  * 0.5  + 0.5f;

	When we pass the o.xy through Vetex shader to Pixel shader, and Interplate the value, it can lead the 
		o.xy value not accurated, especially when the Mesh has very few vertices.

	So we times o.w  with  o.xy which lead the equation to be:
		In Vertex Shader:
			o.xy = float2(o.x, o.y * _ProjectionParams.x)  * 0.5  + 0.5f * o.w;
		
		In Pixel Shader:
			uv = screenUV.xy / screenUV.w;


	-------------------------Unity Version----------------------------------

	line 1 :       float4 o = pos * 0.5f;
    line 2 :       o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
    line 3 :       o.zw = pos.zw;
    line 4 :       return o;

	------------------------Derivation Progress-----------------------------
	if we skip the line one, and times 0.5f in line2, we get the same Equation of Unity has

	o.xy = float2(o.x, o.y*_ProjectionParams.x) * 0.5f + o.w * 0.5f;

	-------------------------------------------------------------------------*/
}

inline float3 V_World_To_Camera_Dir(float3 worldPos)
{
	return normalize(_WorldSpaceCameraPos - worldPos);
}

inline float3 V_World_To_Camera_Vector(float3 woldPos)
{
	return _WorldSpaceCameraPos - woldPos;
}

inline float3 V_Reflect_Camera_To_World_Dir(float3 normal, float3 worldPos)
{
	return reflect(normalize(worldPos - _WorldSpaceCameraPos), normal);
}

inline float3 V_Reflect_Inverse_World_To_Camera_Dir(float3 world2CameraDir, float3 normal)
{
	return reflect(-world2CameraDir, normal);
}

inline float V_ToCamera_Normal_Angle_01(float3 worldToCamera, float3 normal)
{
	return saturate(dot(worldToCamera, normal));
}

/*---------------------------------------------------------------------

//   float4 _ZBufferParams;
//   x = 1-far/near
//   y = far/near
//   z = x/far
//   w = y/far
//   or in case of a reversed depth buffer (UNITY_REVERSED_Z is 1)
//   x = -1+far/near
//   y = 1
//   z = x/far
//   w = 1/far


n = near
f = far
z = depth buffer Z-value----------------------------------> clip space Z value
EZ  = eye Z value
LZ  = depth buffer Z-value remapped to a linear [0..1] range (near plane to far plane)
LZ2 = depth buffer Z-value remapped to a linear [0..1] range (eye to far plane)


DX:
EZ  = (n * f) / (f - z * (f - n))
LZ  = (eyeZ - n) / (f - n) = z / (f - z * (f - n))
LZ2 = eyeZ / f = n / (f - z * (f - n))


GL:
EZ  = (2 * n * f) / (f + n - z * (f - n))
LZ  = (eyeZ - n) / (f - n) = n * (z + 1.0) / (f + n - z * (f - n))
LZ2 = eyeZ / f = (2 * n) / (f + n - z * (f - n))



LZ2 in two instructions:
LZ2 = 1.0 / (c0 * z + c1)

DX:
c1 = f / n
c0 = 1.0 - c1

GL:
c0 = (1 - f / n) / 2
c1 = (1 + f / n) / 2
----------------------------------------------------------------------------------------*/

//------------------------------------------------------------------------------------------
// The Depth Buffer Value is -------------------- (clip space Z value)----Homogenous coordinates with 1
//------------------------------------------------------------------------------------------
//Z buffer to linear 0..1 depth------> Pos_cam.Z/far; 
/*---------*/ 
inline float V_Linear01Depth(float z)
{
	return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
	// _ZBufferParams.x  = 1-far/near
	// _ZBufferParams.y = far/near
	//.................
}

//Z buffer to linear depth-------> Pos_cam.Z 
inline float V_LinearEyeDepth(float z)
{
	return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}
//---------------------------------------------------------------------------------------------------

#endif