//UNITY_SHADER_NO_UPGRADE
#ifndef NOISE_VAL_TO_COLOR_INCLUDED
#define NOISE_VAL_TO_COLOR_INCLUDED

void NoiseValToColor_float(float4 color, UnityTexture2D colorMap, out float4 Out)
{
    float luminance = (color.r + color.g + color.b) / 3.0; 
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

#endif //NOISE_VAL_TO_COLOR_INCLUDED
