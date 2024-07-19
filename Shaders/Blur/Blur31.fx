#define KERNEL_RADIUS 31
#include "BlurBase.fxh"

technique Simple
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL VerticalPS();
	}
	pass Pass1 
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL HorizontalPS();
	}
};

technique General
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL GeneralPS();
	}
}