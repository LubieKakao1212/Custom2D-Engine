#ifndef SIMPLE
#define SIMPLE

struct VSInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct PSInput
{
	float4 Position : SV_POSITION;
	float4 UVPos : TEXCOORD0;
};

PSInput MainVS(in VSInput input)
{
	PSInput output;

	float4 screenPos = float4(input.Position.xy, 0.0f, 1.0f);
	output.Position = screenPos;
	output.UVPos = float4(input.UV, screenPos.xy);

	return output;
}
#endif