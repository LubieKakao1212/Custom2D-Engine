#ifndef LIT_PS
#define LIT_PS

#include "PBRSprites.fxh"
#include "SceneLights.fxh"
#include "StructsPS.fxh"

float4 LitNormal(PSInput input) : COLOR 
{
	return float4(normal(input.AtlasPos), 0.0f);
}

float4 LitFinalPS(PSInput input) : COLOR
{
	float3 emit = emission(input.AtlasPos);
	float4 col = color(input.AtlasPos);

	float2 screenPosUV = (input.ScreenPos + 1.0f) * 0.5f;

	float3 lights = lightMap(screenPosUV);

	return float4(lights * col.rgb + emit, col.a);
}

#endif