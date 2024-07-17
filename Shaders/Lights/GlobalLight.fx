
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif


#include "../Include/Simple.fxh"
#include "Include/LightNormals.fxh"

float Intensity;
float4 Tint;

float2 Direction;

float4 MainPS(PSInput input) : COLOR
{
    float fromNormal = dotNormal(input.UVPos.xy, Direction);
	float light = fromNormal * Intensity;
	return float4((Tint * light).xyz, 1.0f);//float4(sceneNormal, 1.0f);//float4((Tint * light).xyz, 0.0f);
}

technique GlobalLight
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
