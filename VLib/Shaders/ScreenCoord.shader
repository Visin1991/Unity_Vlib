Shader "V/ScreenCoord" {
	Properties {

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "../ShaderLib/VCamera.cginc"
			
			struct VertexData
			{
				float4 vertex : POSITION;
			};	

			struct Interpolaters
			{
				float4 pos : SV_POSITION;
				float4 clip : TEXCOORD3;
			};
			
			Interpolaters vert(VertexData v)
			{
				Interpolaters o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.clip = o.pos;
				return o;
			}

			half4 frag(Interpolaters i) : SV_TARGET
			{
				half2 screenPos =  V_ComputeScreenPos_Ps(i.clip);
				return half4(screenPos.x,screenPos.y,i.pos.z ,1);
			}

			ENDCG
		}
	}
}
