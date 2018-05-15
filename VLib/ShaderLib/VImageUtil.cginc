// Upgrade NOTE: replaced 'defined _V_IMAGEUTILI_CG_' with 'defined (_V_IMAGEUTILI_CG_)'

#if !defined (_V_IMAGEUTILI_CG_)
#define _V_IMAGEUTILI_CG


//Use the function in vertex shader...../ or in PS shader
float3 Get_CameraToFarClipProj_World(float4 clip_near, float4x4 clipToWorld)
{
	//Calculate the far clip plane Projection Position XY ------------------Homogenous Coordinates now equal to one
	float2 clip_Homo1 = clip_near.xy / clip_near.w;

	//In clip space for all point on far clip plane have Z value of 1, and Homogenous Coordinates 1. ----------------------------for example the center of the screen will be (0,0,1,1)
	float4 clipXYZ_Homo1 = float4(clip_Homo1, 1, 1);

	//Check if the coordinates flipped
	clipXYZ_Homo1.y *= _ProjectionParams.x;

	//clipXYZ_Homo1------> can be see as a point on a  plane in clip space-------> and the plane has the distance 1 to the camera
	float4 farClipProj_Wrold = mul(clipToWorld, clipXYZ_Homo1);

	//When Homogenous Coordinates equal to one. we get reletively world position
	farClipProj_Wrold.xyz = farClipProj_Wrold.xyz / farClipProj_Wrold.w;

	return  farClipProj_Wrold.xyz - _WorldSpaceCameraPos;
}


float3 Get_WorldPos_From_CameraToFarClipProj_World(float3 cameraToFarClipProj_World, float depth)
{
	float3 worldPos = (cameraToFarClipProj_World * depth) + _WorldSpaceCameraPos;
	worldPos.y += 0.00001; // fixes artifacts when worldPos.y = _WorldSpaceCameraPos.y which is really rare but occurs at y = 0
	return worldPos;
}

/// return -1 means, the ray did not intersect with the plane.
/// return 0 may means the ray almost parallel with the plane
float Ray_Plane_Intersection_Distance(float3 planeNormal, float planeDistance, float3 rayOrigin, float3 rayDir)
{
	float denom = dot(rayDir, planeNormal);
	if (abs(denom) > 0.0000001f)
	{
		float3 worldOriginToPlane = planeNormal * planeDistance;
		float t = dot(float3(worldOriginToPlane - rayOrigin), planeNormal) / denom;
		return t;
	}
	return -1;
}

#endif