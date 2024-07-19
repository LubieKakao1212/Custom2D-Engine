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
	float3 col = tex2D(TexSampler, input.UV);
    
    float cMx = max(col.r, max(col.g, col.b));
    // float cMn = min(col.r, min(col.g, col.b));
    // float d = cMx - cMn;
    
    return float4(col / max(cMx, 1.0f), 1.0f);
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};