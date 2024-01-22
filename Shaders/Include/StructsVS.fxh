#ifndef STRUCTS_VS
#define STRUCTS_VS

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct InstanceData
{
	float4 RotScale : POSITION1;
	float2 Pos : POSITION2;
	float4 Color : COLOR0;
	float4 AtlasPos : TEXCOORD1;
};

#endif