#ifndef SPRITES
#define SPRITES

#include "SpritesCommon.fxh"

Texture3D ColorAtlas : register(t0);
SamplerState AtlasSampler : register(s0);
// sampler3D ColorAtlasSampler = sampler_state
// {
//     texture = <ColorAtlas>;
// };

float4 color(float3 spritePos)
{
    return ColorAtlas.SampleLevel(AtlasSampler, spritePos, 0);// tex3D(ColorAtlasSampler, spritePos);//ColorAtlas.SampleLevel(ColorAtlasSampler, spritePos, 0);
}
#endif