#ifndef LIGHT_NORMALS
#define LIGHT_NORMALS

#include "../../Include/SceneNormals.fxh"

float Height;

float dotNormal(float2 screenPos, float2 lightDir) 
{
    float3 lightDirNorm = normalize(float3(lightDir, -Height));
    return clamp(dot(sceneNormal(screenPos), -lightDirNorm), 0, 1);
    //return sceneNormal(screenPos).z;
}

#endif