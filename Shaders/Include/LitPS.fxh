#ifndef LIT_PS
#define LIT_PS

#include "PBRSprites.fxh"
#include "SceneLights.fxh"
#include "StructsPS.fxh"

float4 LitNormalPS(PSInput input) : COLOR 
{
	float4 rawNormal = normal(input.AtlasPos);
	//clip(rawNormal.a - 0.5f);

	float2x2 tangents = float2x2(input.Tangents);
	tangents = tangents / sqrt(determinant(tangents));
	tangents = transpose(tangents);
	
	float3 surfaceNormal = (rawNormal.xyz * 2.0f) - 1.0f;
	surfaceNormal.y = -surfaceNormal.y;
	surfaceNormal = float3(mul(tangents, surfaceNormal.xy), surfaceNormal.z);
	//surfaceNormal = (surfaceNormal + 1.0f) * 0.5f;

	//Using premultiplied blending
	return float4(surfaceNormal * rawNormal.a, rawNormal.a);
}

float4 LitFinalPS(PSInput input) : COLOR
{
	float4 emit = emission(input.AtlasPos);
	float4 albedo = color(input.AtlasPos);

	float2 screenPosUV = (input.ScreenPos + 1.0f) * 0.5f;

	float3 lights = lightMap(screenPosUV);

	//Premultiplied
	float3 litMul = lights * albedo.rgb;
	float3 unlitMul = emit.rgb;
	
	//Emit over albedo
	float alpha = emit.a + albedo.a * (1.0f - emit.a);
	float3 color = unlitMul + litMul * (1.0f - emit.a);

	return float4(color * input.Color.rgb, alpha * input.Color.a);
}

#endif