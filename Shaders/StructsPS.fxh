#ifndef STRUCTS_PS
#define STRUCTS_PS

struct PSInput
{
	float4 Position : SV_POSITION;
	float2 ScreenPos : POSITION1;
	//float2 WorldPosition : POSITION2;
	
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
	float3 AtlasPos : TEXCOORD1;
};

#endif