//UNITY_SHADER_NO_UPGRADE
#ifndef POS_VAL_TO_COLOR_INCLUDED
#define POS_VAL_TO_COLOR_INCLUDED

float inverseLerp(float A, float B, float T);

void PosValToColor_float(float3 position, UnityTexture2D colorMap, float height, out float4 Out)
{
    
    float luminance = inverseLerp(0, height, position.y); 
    luminance = saturate(luminance);

    //texelSize.w - height
    //texelSize.z - width 

    for (int i = 0; i < colorMap.texelSize.z; i++) {
        float limit = colorMap.Load(int3(i, 1, 0)).x;
        if (luminance < limit) {
            Out = colorMap.Load(int3(i, 0, 0));
            break;
        }
    }

}

float inverseLerp(float A, float B, float T)
{
    return (T - A)/(B - A);
}

#endif //POS_VAL_TO_COLOR_INCLUDED
