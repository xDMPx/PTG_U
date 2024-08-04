//UNITY_SHADER_NO_UPGRADE
#ifndef HIGHT_TO_COLOR_INCLUDED
#define HIGHT_TO_COLOR_INCLUDED

void HightToColor_float(float4 color, float4 c1, float4 c2, float4 c3, float4 c4, out float4 Out)
{
    float luminance = (color.r + color.g + color.b) / 3.0; 
    luminance = saturate(luminance);

    if (luminance < 0.1){
        Out = c1;
    } else if (luminance < 0.2){
        Out = c2;
    } else if (luminance < 0.5){
        Out = c3;
    } else {
        Out = c4;
    }

}

#endif //HIGHT_TO_COLOR_INCLUDED
