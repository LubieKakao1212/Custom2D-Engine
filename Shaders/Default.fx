#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

//#include "Transforms.fxh"
#include "Include/Camera.fxh"
#include "Include/StructsPS.fxh"
#include "Include/StructsVS.fxh"
#include "Include/UnlitPS.fxh"
#include "Include/LitPS.fxh"

PSInput MainVS(in VertexShaderInput input, in InstanceData instance)
{
	PSInput output;

	float3x3 LtV = LocalToView(instance.RotScale, instance.Pos);

	float4 screenPos = float4(mul(LtV, float3(input.Position.xy, 1.0f)).xy, 0.0f, 1.0f);
	output.Position = screenPos;
	output.ScreenPos = screenPos.xy;

	output.Tangents = instance.RotScale; //LtV._m00_m01_m10_m11;

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