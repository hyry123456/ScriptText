#ifndef DEFFER_CARTOON_PASS
#define DEFFER_CARTOON_PASS

#include "../../ShaderLibrary/Surface.hlsl"
#include "../../ShaderLibrary/Shadows.hlsl"
#include "../../ShaderLibrary/Light.hlsl"
// #include "../../ShaderLibrary/Lighting.hlsl"

struct a2v 
{
    float3 vertex : POSITION;
    // float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
};
struct v2f
{
    float4 pos : SV_POSITION;
};

v2f outlineVert (a2v input) 
{
    v2f output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    float4 pos = TransformObjectToHClip(input.vertex);
    //获得视角空间的法线坐标,由于我们将平均后的法线坐标存储在了顶点色中，所以使用这个属性
    float3 viewNormal = mul((float3x3)unity_IT_MatrixMV,  input.tangent.xyz);
    //将法线变换到NDC空间，NDC空间看起来是一个长方形，可以保证在法线在任何情况下看起来都是一样长的
    // float3 ndcNormal = normalize(TransformWViewToHClip(viewNormal).xyz) * pos.w;
    float3 ndcNormal = normalize(TransformWViewToHClip(viewNormal).xyz);
    float3 normal = input.tangent;
    float outlineWidth = GetOutline(input.uv);
    // pos.xy += 0.01 * outlineWidth * ndcNormal.xy;
    pos.xy += ndcNormal * (pos.w * outlineWidth * 0.1);
    output.pos = pos;
    return output;
}

half4 outlineFrag(v2f i) : SV_TARGET 
{
    return GetOutlineColor();
}

struct Attributes{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 baseUV : TEXCOORD0;
};

struct Varyings{
    float4 positionCS : SV_POSITION;
    float3 positionWS : VAR_POSITION;
    float3 normalWS : VAR_NORMAL;
    float2 baseUV : VAR_UV;
};

Varyings CartoonVert(Attributes input){
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    output.positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
    output.baseUV = TransformBaseUV(input.baseUV);
    return output;
}



half3 LightingToon(Surface surface, Light light) {
	half3 nNormal = normalize(surface.normal);
	float3 reflectDir = reflect(-surface.viewDirection, surface.normal);

    // float2 shadowRange = GetColorChangeRange();
    float attenWeight = GetLightAttenionWeight();

	half NoL = dot(nNormal, light.direction) + attenWeight * (light.attenuation - 1);
	half3 HDir = normalize(light.direction + surface.viewDirection);
	half NoH = Pow2(dot(nNormal, HDir)) + attenWeight * (light.attenuation - 1);
	half VoN = dot(nNormal, surface.viewDirection);
	half VoL = dot(surface.viewDirection, light.direction);
	half VoH = dot(surface.viewDirection, HDir) + attenWeight * 2 * (light.attenuation - 1);

    float3 divid3 = GetDivid3Level();
    float boundSharp = GetBoundSharp();

    half MidSig = sigmoid(NoL, divid3.y, boundSharp);
    half DarkSig = sigmoid(NoL, divid3.z, boundSharp);
    half DeepDarkSig = sigmoid(NoL, divid3.x, boundSharp);

    half MidLWin = MidSig;
    half MidDWin = DarkSig - MidSig;
    half DarkWin = DeepDarkSig - DarkSig;
    half DeepDarkWin = 1 - DeepDarkSig;

    half diffuse = MidLWin * (1 + ndc2Normal(divid3.y)) / 2 + MidDWin * (ndc2Normal(divid3.y) + ndc2Normal(divid3.z)) / 2;
    diffuse += DarkWin * (ndc2Normal(divid3.z) + ndc2Normal(divid3.x)) / 2 + DeepDarkWin * ndc2Normal(divid3.x) / 2;

    float roughness = GetRoughness();
    float dividSpec = GetDividLineSpec();
    half NDF0 = D_GGX(roughness * roughness, 1);
    half NDF_HBound = NDF0 * dividSpec;
    half NDF = D_GGX(roughness * roughness, clamp(0, 1, NoH));
    half specularWin = sigmoid(NDF, NDF_HBound, boundSharp);

    half specular = specularWin * (NDF0 + NDF_HBound) / 2;

    //双重高光叠加，叠加更光滑的高光
    float heighRougness = 0.15;
    float NDf0_Add = D_GGX(heighRougness * heighRougness, 1);
    half NDF_HBound_ADD = NDf0_Add * 0.85;
    half NDF_ADD = D_GGX(heighRougness * heighRougness, clamp(0, 1, NoH));

    half specularWin_Add = sigmoid(NDF_ADD, NDF_HBound_ADD, boundSharp);
    specular += specularWin_Add * (NDf0_Add + NDF_HBound_ADD) / 2;
    specular /= 2.0;


    float fresnelInten = GetFresnelIntensity();
    half3 fresnel = Fresnel_extend(VoN, float3(0.1, 0.1, 0.1));
    half3 fresnelResult = fresnelInten * fresnel * (1 - VoL) / 2;

    half3 Intensity = specular.xxx + diffuse.xxx + fresnelResult.xxx + MidLWin * MidDWin * float3(0, 0, 0.3);

    // return MidLWin * MidDWin;

    return surface.color * Intensity.xyz * light.color;

}

float3 GetCartoonLight(Surface surface, float4 clipPos){
	ShadowData shadowData = GetShadowData(surface);

	float3 re = 0;
	for (int i = 0; i < GetDirectionalLightCount(); i++) {
		Light light = GetDirectionalLight(i, surface, shadowData);
        // intensity += dot(light.direction, surface.normal);
        re += LightingToon(surface, light);
	}

#ifdef _USE_CLUSTER
	uint id = Get1DCluster(clipPos.xy / _ScreenParams.xy, clipPos.w);
	int count = min(_ClusterCountBuffer[id], GetOtherLightCount());
	LightArray array = _ClusterArrayBuffer[id];

	for (int j = 0; j < count; j++) {
		Light light = GetOtherLight(array.lightIndex[j], surface, shadowData);
        light.color *= light.attenuation;
        re += LightingToon(surface, light);
	}
#else
	for (int j = 0; j < GetOtherLightCount(); j++) {
		Light light = GetOtherLight(j, surface, shadowData);
        // intensity += dot(light.direction, surface.normal);
        light.color *= light.attenuation;
        re += LightingToon(surface, light);
	}
#endif
	return re;
}



float4 CartoonFrag(Varyings input) : SV_TARGET{
	UNITY_SETUP_INSTANCE_ID(input);
    float4 brighnessCol = GetBrighnessColor(input.baseUV);

    Surface surface = (Surface)0;
    // surface.depth = input.positionCS.w;
    surface.depth = -TransformWorldToView(input.positionWS).z;
    surface.dither = InterleavedGradientNoise(input.positionCS.xy, 0);
    surface.position = input.positionWS;
    surface.normal = input.normalWS;
    surface.color = brighnessCol.xyz;
    surface.alpha = brighnessCol.w;
    surface.viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);

    float3 color = GetCartoonLight(surface, input.positionCS);
    // float4 color = lerp(shadowCol, brighnessCol, smoothstep(colorRange.x, colorRange.y, intensity));

    
    return float4(color, 1);
}

#endif