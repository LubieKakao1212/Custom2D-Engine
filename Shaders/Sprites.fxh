#ifndef SPRITES
#define SPRITES

#include "SpritesCommon.fxh"

// Texture3D ColorAtlas : register(t0);
// SamplerState ColorAtlasSampler : register(s0)
// {
// 	Texture = <ColorAtlas>;
// };
 
sampler3D ColorAtlasSampler : register(s0) = sampler_state
{
	Texture = <ColorAtlas>;
};

float4 color(float3 spritePos)
{
    return tex3D(ColorAtlasSampler, spritePos);//ColorAtlas.SampleLevel(ColorAtlasSampler, spritePos, 0);
}
#endif