#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

#ifndef KERNEL_RADIUS
	#define KERNEL_RADIUS 3
#endif

#include "../Include/SimpleTex.fxh"

float2 delta;
float resInv;
float kernel[KERNEL_RADIUS];

float4 doSample(float2 uv, int kernelIndex) {
	return tex2D(TexSampler, uv) * kernel[kernelIndex];
}

float4 doBlur(float2 uv, float2 delta) {
	float4 col = doSample(uv, 0);
	[unroll(KERNEL_RADIUS - 1)]
	for(int i=1; true; i++)
	{
		col += doSample(uv + delta * i, i);
		col += doSample(uv - delta * i, i);
	}
	return col;// / (KERNEL_RADIUS * 2 + 1)));
}

float4 HorizontalPS(VertexShaderOutput input) : COLOR
{
	return doBlur(input.UV, float2(resInv, 0));
}

float4 VerticalPS(VertexShaderOutput input) : COLOR
{
	return doBlur(input.UV, float2(0, resInv));
}

float4 GeneralPS(VertexShaderOutput input) : COLOR
{
	return doBlur(input.UV, delta);
}