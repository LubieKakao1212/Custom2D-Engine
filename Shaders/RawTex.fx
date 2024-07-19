#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

#include "Include/SimpleTex.fxh"

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(TexSampler, input.UV);
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};