#ifndef SIMPLE_WS
#define SIMPLE_WS

#include "Camera.fxh"

float4 RotScale;
float2 Pos;

struct VSInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct PSInput
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
	float2 ScreenPos : TEXCOORD1;
	float2 WorldPos : TEXCOORD2;
};

PSInput MainVS(in VSInput input)
{
	PSInput output;

    float3x3 LtW = ComposeTransform(RotScale, Pos);
    float3x3 WtV = Projection();

    float3 WPos = mul(LtW, float3(input.Position.xy, 1.0f));
    output.WorldPos = WPos.xy;

	float4 screenPos = float4(mul(WtV, WPos).xy, 0.0f, 1.0f);
	output.Position = screenPos;
	output.ScreenPos = screenPos.xy;

	output.UV = input.UV;

	return output;
}
#endif