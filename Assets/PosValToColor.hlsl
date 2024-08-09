//UNITY_SHADER_NO_UPGRADE
#ifndef POS_VAL_TO_COLOR_INCLUDED
#define POS_VAL_TO_COLOR_INCLUDED

float inverseLerp(float A, float B, float T);

void PosValToColor_float(float3 position, UnityTexture2D colorMap, float height, out float4 Out)
{
    
    float luminance = inverseLerp(0,height,position.y); 
    luminance = saturate(luminance);

    //texelSize.w - height
    //texelSize.z - width 

    if (luminance < colorMap.Load(int3(0,1,0)).x){
        Out = colorMap.Load(int3(0,0,0));
    } else if (luminance < colorMap.Load(int3(1,1,0)).x){
        Out = colorMap.Load(int3(1,0,0));
    } else if (luminance < colorMap.Load(int3(2,1,0)).x){
        Out = colorMap.Load(int3(2,0,0));
    } else if (luminance < colorMap.Load(int3(3,1,0)).x){
        Out = colorMap.Load(int3(3,0,0));
    }

}

float inverseLerp(float A, float B, float T)
{
    return (T - A)/(B - A);
}

#endif //POS_VAL_TO_COLOR_INCLUDED
