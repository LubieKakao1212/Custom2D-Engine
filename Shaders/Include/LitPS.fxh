#ifndef LIT_PS
#define LIT_PS

#include "PBRSprites.fxh"
#include "SceneLights.fxh"
#include "StructsPS.fxh"

float4 LitNormalPS(PSInput input) : COLOR 
{
	float2x2 tangents = float2x2(input.Tangents);
	tangents = tangents / sqrt(determinant(tangents));
	tangents = transpose(tangents);
	float4 rawNormal = normal(input.AtlasPos);

	clip(rawNormal.a - 0.5f);

	float3 surfaceNormal = (rawNormal.xyz * 2.0f) - 1.0f;
	surfaceNormal.y = -surfaceNormal.y;
	surfaceNormal = float3(mul(tangents, surfaceNormal.xy), surfaceNormal.z);
	surfaceNormal = (surfaceNormal + 1.0f) * 0.5f;

	return float4(surfaceNormal, 1.0f);
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