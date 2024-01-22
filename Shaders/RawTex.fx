#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

Texture2D Tex;

sampler2D TexSampler = sampler_state
{
	Texture = <Tex>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = float4(input.Position.xy, 0.0f, 1.0f);
	output.UV = input.UV;

	return output;
}

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