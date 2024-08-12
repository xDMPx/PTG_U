//UNITY_SHADER_NO_UPGRADE
#ifndef NOISE_VAL_TO_COLOR_INCLUDED
#define NOISE_VAL_TO_COLOR_INCLUDED

void NoiseValToColor_float(float4 color, UnityTexture2D colorMap, out float4 Out)
{
    float luminance = (color.r + color.g + color.b) / 3.0; 
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

#endif //NOISE_VAL_TO_COLOR_INCLUDED
