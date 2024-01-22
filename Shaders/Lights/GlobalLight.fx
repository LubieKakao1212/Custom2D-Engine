
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

sampler2D SceneNormalsSampler = sampler_state
{
	Texture = <SceneNormals>;
};

float Intensity;
float4 Tint;
float2 Direction;
float Height;

struct VSInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct PSInput
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
};

PSInput MainVS(in VSInput input)
{
	PSInput output;

	output.Position = float4(input.Position.xy, 0.0f, 1.0f);
	output.UV = input.UV;

	return output;
}

float4 MainPS(PSInput input) : COLOR
{
    float3 sceneNormal = normalize(tex2D(SceneNormalsSampler, input.UV).xyz);
    sceneNormal = (sceneNormal * 2.0f) - 1.0f;
    float3 lightDir = normalize(float3(Direction, -Height));
    float light = dot(sceneNormal, -lightDir) * Intensity;
	return float4((Tint * light).xyz, 0.0f);//float4(sceneNormal, 1.0f);//float4((Tint * light).xyz, 0.0f);
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
