#ifndef UNLIT_PS
#define UNLIT_PS

#include "PBRSprites.fxh"
#include "SceneLights.fxh"
#include "StructsPS.fxh"

float4 UnlitPS(PSInput input) : COLOR
{
	return color(input.AtlasPos) * input.Color;
}

#endif