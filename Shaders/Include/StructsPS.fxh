#ifndef STRUCTS_PS
#define STRUCTS_PS

struct PSInput
{
	float4 Position : SV_POSITION;
	float4 Tangents : TEXCOORD3;

	float2 ScreenPos : TEXCOORD4;
	//float2 WorldPosition : POSITION2;

	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
	float3 AtlasPos : TEXCOORD1;
};

#endif