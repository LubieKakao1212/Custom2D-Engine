#ifndef SPRITES_COMMON
#define SPRITES_COMMON

int AtlasSize;

float3 ProcessSpritePos(float4 atlasPos, float2 UV) {
    float2 spriteSize = atlasPos.zw;
	float2 spritePos = float2(frac(atlasPos.x), atlasPos.y);
	float atlasIdx = floor(atlasPos.x);

	return float3(spritePos + (UV * spriteSize), atlasIdx / AtlasSize);
}

#endif