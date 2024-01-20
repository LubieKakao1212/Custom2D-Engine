#ifndef SPRITES_PBR
#define SPRITES_PBR

#include "Sprites.fxh"

// Texture3D NormalAtlas : register(t1);
// SamplerState NormalAtlasSampler : register(s1);

// Texture3D EmissionAtlas : register(t2);
// SamplerState EmissionAtlasSampler : register(s2);

sampler3D NormalAtlasSampler : register(s1) = sampler_state
{
	Texture = <NormalAtlas>;
};

sampler3D EmissionAtlasSampler : register(s2) = sampler_state
{
	Texture = <EmissionAtlas>;
};

float3 normal(float3 spritePos)
{
    return tex3D(NormalAtlasSampler, spritePos).rgb;
}

float3 emission(float3 spritePos)
{
    return tex3D(EmissionAtlasSampler, spritePos).rgb;
    //return EmissionAtlas.SampleLevel(EmissionAtlasSampler, spritePos, 0).rgb;
}

#endif