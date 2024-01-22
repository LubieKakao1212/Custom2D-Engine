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

float4 GridRS;
float2 GridT;

PSInput MainVS(in VertexShaderInput input, in InstanceData instance)
{
	PSInput output;

    float3x3 GridToWorld = ComposeTransform(GridRS, GridT);
    float3x3 LocalToTile = ComposeTransform(instance.RotScale, float2(0.0f, 0.0f));

    float3x3 LtG = LocalToTile;

    float3x3 GtV = mul(Projection(), GridToWorld);

	float3 position = mul(LtG, float3(input.Position.xy, 1.0f));
	position.xy += instance.Pos;

	float4 screenPos = float4(mul(GtV, position).xy, 0.0f, 1.0f);
	output.Position = screenPos;
	output.ScreenPos = screenPos.xy;
	output.Tangents = mul(float2x2(GridRS), float2x2(instance.RotScale));

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