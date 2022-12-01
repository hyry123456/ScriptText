#ifndef DEFFER_CARTOON_INPUT
#define DEFFER_CARTOON_INPUT


TEXTURE2D(_BrighnessMap);
// TEXTURE2D(_ShadowMap);
SAMPLER(sampler_BrighnessMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)

	UNITY_DEFINE_INSTANCED_PROP(float4, _BrighnessMap_ST)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BrighnessCol)
	// UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowCol)
	UNITY_DEFINE_INSTANCED_PROP(float4, _OutLineCol)
    UNITY_DEFINE_INSTANCED_PROP(float, _ShadowAttWeight)
    UNITY_DEFINE_INSTANCED_PROP(float, _OutlineWidth)
    UNITY_DEFINE_INSTANCED_PROP(float, _DividLineDeepDark)
    UNITY_DEFINE_INSTANCED_PROP(float, _DividLineM)
    UNITY_DEFINE_INSTANCED_PROP(float, _DividLineD)
    UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)
    UNITY_DEFINE_INSTANCED_PROP(float, _FresnelEff)
    UNITY_DEFINE_INSTANCED_PROP(float, _DividLineSpec)
    

UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

#define INPUT_PROP(name) UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, name)

half LinearRgbToLuminance(half3 linearRgb)
{
    return dot(linearRgb, half3(0.2126729f,  0.7151522f, 0.0721750f));
}

float GetOutline(float2 uv){
    float3 map = SAMPLE_TEXTURE2D_LOD(_BrighnessMap, sampler_BrighnessMap, uv, 0).xyz;
    half width = LinearRgbToLuminance(map);
    return INPUT_PROP(_OutlineWidth) * width;
}

float2 TransformBaseUV (float2 uv) {
	float4 baseST = INPUT_PROP(_BrighnessMap_ST);
	return uv * baseST.xy + baseST.zw;
}

float3 GetDivid3Level(){
    return float3(INPUT_PROP(_DividLineDeepDark), INPUT_PROP(_DividLineM), INPUT_PROP(_DividLineD));
}

float4 GetOutlineColor(){
    return INPUT_PROP(_OutLineCol);
}

float4 GetBrighnessColor(float2 baseUV){
    float4 brighness = SAMPLE_TEXTURE2D(_BrighnessMap, sampler_BrighnessMap, baseUV);
    float4 brighnessCol = INPUT_PROP(_BrighnessCol);
    return brighness * brighnessCol;
}

float GetLightAttenionWeight(){
    return INPUT_PROP(_ShadowAttWeight);
}



float GetRoughness(){
    return INPUT_PROP(_Roughness);
}

float GetFresnelIntensity(){
    return INPUT_PROP(_FresnelEff);
}

float GetDividLineSpec(){
    return INPUT_PROP(_DividLineSpec);
}


float ndc2Normal(float x) {
     return x * 0.5 + 0.5;
}

float halfDot(float3 dir1, float3 dir2){
    return dot(dir1, dir2) * 0.5 + 0.5;
}

float Pow2(float x){
    return x * x;
}

float GetBoundSharp(){
    return 9.5 * Pow2(INPUT_PROP(_Roughness) - 1) + 0.5;
}

float sigmoid(float x, float center, float sharp) {
    float s;
    s = 1 / (1 + pow(100000, (-3 * sharp * (x - center))));
    return s;
}

// a2 is the roughness^2
float D_GGX(float a2, float NoH) {
    float d = (NoH * a2 - NoH) * NoH + 1;
    return a2 / (3.14159 * d * d);
}

float3 Fresnel_extend(float VoN, float3 rF0) {
    return rF0 + (1 - rF0) * pow(1 - VoN, 3);
}


#endif