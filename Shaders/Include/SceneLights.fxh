#ifndef SCENE_LIGHTS
#define SCENE_LIGHTS

//Texture2D SceneLights;

sampler2D SceneLightsSampler = sampler_state
{
	Texture = <SceneLights>;
};

float3 lightMap(float2 screenPos)
{
    return tex2D(SceneLightsSampler, screenPos).rgb;
}
#endif