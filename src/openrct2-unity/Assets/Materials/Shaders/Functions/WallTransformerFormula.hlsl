#ifndef Transformer
#define Transformer

float Slope_float(float u, float height, float gap)
{
    return (u * (gap / height));
}


void Transform_float(float u, float v, float width, float height, out float Out)
{
    float gap = (width / 2);

    Out = (v * ((height - gap) / height) - Slope_float(u, height, gap)) + (gap / height);
}

#endif
