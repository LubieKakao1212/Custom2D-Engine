#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "Include/Transforms.fxh"
#include "Include/Camera.fxh"
#include "Include/StructsPS.fxh"
#include "Include/StructsVS.fxh"
#include "Include/UnlitPS.fxh"
#include "Include/LitPS.fxh"

float4 ChunkRS;
float2 ChunkT;
float2 Spacing;

PSInput MainVS(in VertexShaderInput input, in InstanceData instance)
{
	PSInput output;

	float3x3 projection = Projection();
    float3x3 chunkToWorld = ComposeTransform(ChunkRS, ChunkT);
    float3x3 tileToChunk = ComposeTransform(instance.RotScale, instance.Pos * Spacing);

    float3x3 TtW = mul(chunkToWorld, tileToChunk);
	float3x3 TtV = mul(projection, TtW);

	float4 screenPos = float4(mul(TtV, float3(input.Position.xy, 1.0f)).xy, 0.0f, 1.0f);
	output.Position = screenPos;
	output.ScreenPos = screenPos.xy;

	output.Tangents = TtW._m00_m01_m10_m11;//mul(float2x2(GridRS), float2x2(instance.RotScale));

	output.Color = instance.Color;

	output.AtlasPos = ProcessSpritePos(instance.AtlasPos, input.UV);

	output.UV = input.UV;


	return output;
}

technique Unlit
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL UnlitPS();
	}
};

technique Lit
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL LitNormalPS();
	}

	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL LitFinalPS();
	}
}