#ifndef SCENE_NORMALS
#define SCENE_NORMALS

sampler2D SceneNormalsSampler = sampler_state
{
	Texture = <SceneNormals>;
};

float3 sceneNormal(float2 screenPos)
{
    float3 sceneNormal = tex2D(SceneNormalsSampler, screenPos).xyz;
    sceneNormal.y = 1 - sceneNormal.y;
    return normalize((sceneNormal * 2.0f) - 1.0f);
}

#endif