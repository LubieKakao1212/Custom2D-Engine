
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

#include "../Include/SimpleWS.fxh"
#include "Include/LightNormals.fxh"

float Intensity;
float4 Tint;

float2 Direction;
float InnerRadiusRatio;
float2 OutInAngleRatio;

int LinearSmoothing;
int RadialSmoothing;

float2 ObjWorldPos;

float slope(float x, float cutoff)
{
	float slope = 1 / (1 - cutoff);
    float value = slope - abs(slope * x);
    return clamp(value, 0, 1);
}

float doLinearFalloff(float2 UV) 
{
	float2 fragPosHalf = UV - 0.5f;
    float fragDist = length(fragPosHalf) * 2.0f;

    return slope(fragDist, InnerRadiusRatio);
}

float doRadialFalloff(float2 dir) 
{
	float cosTheta = dot(normalize(Direction), dir);
	float theta = acos(cosTheta);

	float OutRatio = OutInAngleRatio.x;
	float InRatio = OutInAngleRatio.y;

	return slope(theta / (OutRatio * 3.1415f), InRatio);
}

float4 MainPS(PSInput input) : COLOR
{
    float2 UV = input.UV;
    float2 screenPos = input.ScreenPos;
    float2 screenPosUV = (screenPos + 1.0f) / 2.0f;
	float2 worldPos = input.WorldPos;
	
	float2 localPos = worldPos - ObjWorldPos;
    float2 lightDir = normalize(localPos);

	float linFall = doLinearFalloff(UV);
	float radFall = doRadialFalloff(lightDir);

	float tmp = linFall * radFall * dotNormal(screenPosUV, lightDir);

    return float4(tmp,tmp,tmp,1);//linFall * radFall * float4(lightDir, 0, 1);//(tmp.xxx, 0.0f);//float4((Tint * light).xyz, 0.0f);
}

technique PointLight
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
