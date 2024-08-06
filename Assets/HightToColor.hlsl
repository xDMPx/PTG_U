//UNITY_SHADER_NO_UPGRADE
#ifndef HIGHT_TO_COLOR_INCLUDED
#define HIGHT_TO_COLOR_INCLUDED

void HightToColor_float(float4 color, UnityTexture2D colorMap, out float4 Out)
{
    float luminance = (color.r + color.g + color.b) / 3.0; 
    luminance = saturate(luminance);


    //texelSize.w - height
    //texelSize.z - width 

    if (luminance < 0.1){
        Out = colorMap.Load(int3(0,0,0));
    } else if (luminance < 0.2){
        Out = colorMap.Load(int3(0,1,0));
    } else if (luminance < 0.5){
        Out = colorMap.Load(int3(0,2,0));
    } else {
        Out = colorMap.Load(int3(0,3,0));
    }

}

#endif //HIGHT_TO_COLOR_INCLUDED
