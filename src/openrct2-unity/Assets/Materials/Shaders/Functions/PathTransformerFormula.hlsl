#ifndef PathTransformer
#define PathTransformer


static const float2 aspect_ratio = float2(64, 31); // max pixel width+height of a tile.
static const float scale = 1 / 1.41421356237; // diagonal tile distance.
static const float radians = 0.785398163; // 45 degrees in radians.


static const float3x3 rotation_matrix = float3x3 // this is correct now for rotating in 3d
(
    0.707107, -0.3535535, -0.6123725,
    0.707107, 0.3535535, 0.6123725,
    0., -0.8660253, 0.5
);




float2 uv_to_pixel(float2 uv, float2 size)
{
    return (1. / size) * (uv - 0.5) + 0.5;
}



float2 pixel_to_uv(float2 pixel, float2 size)
{
    return (pixel - 0.5) * aspect_ratio + 0.5;

    //float2
    //(
    //    (pixel.x * horizontal) + 0.5f,
    //    (pixel.y * vertical) + 0.5f
    
    //    //((pixel.x / size.x) / (size.x / horizontal)) + 0.5f,
    //    //((pixel.y / size.y) / (size.y / vertical)) + 0.5f
    //);  
}



void uv_to_pixel_float(float2 uv, float2 size, out float2 Out)
{
    Out = uv_to_pixel(uv, size);
}
void pixel_to_uv_float(float2 uv, float2 size, out float2 Out)
{
    Out = pixel_to_uv(uv, size);
}



/*
 * Adds the sprite offsets to the uvs, as supplied by the .dat file.
 * 
 * uv:       the input uvs
 * size:     size in pixels, of the sprite.
 * offset:   the sprite offset, as supplied by the .dat file.
 * returns:  the output uvs.
 */
float2 sprite_add_offset(float2 uv, float2 size, float2 offset)
{
    // Convert to rct coords and add offset.
    float rct_x = (uv.x * size.x) + offset.x;
    float rct_y = (uv.y * size.y) + offset.y;

    // Convert back to uvs + scale by pixel size.
    float x = (rct_x / (size.x)) / (size.x / aspect_ratio.x);
    float y = (rct_y / (size.y)) / (size.y / aspect_ratio.y);
    
    return float2(x, y);
}


void sprite_add_offset_float(float2 uv, float2 size, float2 offset, out float2 Out)
{
    Out = sprite_add_offset(uv, size, offset);
}


/*
 * Scales the sprite to fit the whole texture.
 * 
 * uv:       the input uvs
 * returns:  the output uvs.
 */
float2 sprite_scale_at_center(float2 uv)
{
    return (uv - 0.5f) * scale + 0.5f;
}


void sprite_scale_to_fit_float(float2 uv, out float2 Out)
{
    Out = sprite_scale_at_center(uv);
}



/*
 * Clamps the texture in a way that does not create stretching outside the bounds.
 * 
 * uv:       the input uvs
 * alpha:    the alpha for this uv position.
 * returns:  the output uvs.
 */
float1 clamp_no_stretch(float2 uv, float1 alpha)
{
    float2 s = step(float2(0, 0), uv) - step(float2(1, 1), uv);
    return s.x * s.y * alpha;
}


void clamp_no_stretch_float(float2 uv, float1 alpha, out float1 Out)
{
    Out = clamp_no_stretch(uv, alpha);
}


/*
 * Rotates the sprite 45 degrees, 30 degrees down with the matrix.
 * 
 * uv:       the input uvs
 * size:     size in pixels, of the sprite.
 * offset:   the sprite offset, as supplied by the .dat file.
 * returns:  the output uvs.
 */
float2 sprite_rotate(float2 uv)
{
    return mul(rotation_matrix, float3(uv, 0));
}


void sprite_rotate_float(float2 uv, out float2 Out)
{
    Out = sprite_rotate(uv);
}


/*
 * Translates the center of the sprite.
 * 
 * uv:       the input uvs
 * size:     size in pixels, of the sprite.
 * offset:   the sprite offset, as supplied by the .dat file.
 * returns:  the output uvs.
 */
float2 sprite_translate_center(float2 uv, float2 size)
{
    float diff_x = (0.5f * (size.x / aspect_ratio.x));
    float diff_y = (0.5f * (size.y / aspect_ratio.y));

    
    return float2
    (
        uv.x + diff_x,
        uv.y + diff_y
    );
}


void sprite_translate_center_float(float2 uv, float2 size, float2 offset, out float2 Out)
{
    Out = sprite_translate_center(uv, size);
}




float angle_triangle(float x1, float y1, float z1,
                     float x2, float y2, float z2,
                     float x3, float y3, float z3)
{
    float number = (x2 - x1) * (x3 - x1) + (y2 - y1) * (y3 - y1) + (z2 - z1) * (z3 - z1);
    
    float den = sqrt(pow(x2 - x1, 2) + pow(y2 - y1, 2) + pow(z2 - z1, 2))
              * sqrt(pow(x3 - x1, 2) + pow(y3 - y1, 2) + pow(z3 - z1, 2));

    float angle = acos(number / den) * (180.0 / 3.14159265);
    return angle;
}




/*
 * Converts the uvs of the path sprite so that the corners of the path are
 * in the corners of the texture.
 *
 * uv:      the input uvs
 * size:    size in pixels, of the sprite.
 * offset:  the sprite offset, as supplied by the .dat file.
 * Out:     the output uvs.
 */
void Rct_path_matrix_float(float2 uv, float2 size, float2 offset, out float2 Out)
{
    float3x3 scale_matrix = float3x3
    (
        (1. / size.x), 0, 0,
        0, (1. / size.y), 0,
        0, 0, 1
    );
    float3x3 scale_down_matrix = float3x3
    (
        size.x, 0, 0,
        0, size.y, 0,
        0, 0, 1
    );
    float3x3 mat_rs = mul(rotation_matrix, scale_matrix);
    

    
    // find center
    float2 center = float2(-offset.x, (size.y + offset.y) - 15.5);
    float3 retval = float3(uv, 0);
    
    if (round(center.x * 3) == round(size.x * uv.x * 3) || round(center.y * 3) == round(size.y * uv.y * 3))
    {
        //retval = float3(0, 0, 0);
    }
    
    //float3 upscale = float3(64, 31, 0);
    //upscale = mul(rotation_matrix, upscale);

    float u = uv.x;
    float v = uv.y;

    //float tilingX = ((1.0 / size.x));
    //float tilingY = ((1.0 / size.y));
    
    float offsetX = -(offset.x + 32) / 64;
    float offsetY = -offset.y / 31;

    float scaledX = (u * 64/** tilingX * 37.2*/) + offsetX;
    float scaledY = (v * 31/** tilingY * 59.6*/) + offsetY;

    float3 scaledVector = float3(scaledX, scaledY, 0); // vector of 3 values
    
    float3 rotatedVector = mul(rotation_matrix, scaledVector);
    rotatedVector.x += 0.5;
    
    Out = rotatedVector;

    //rotatedVector *= float3(1.21, 0.79, 1);
    //rotatedVector += float3(0.53, 0.13, 0);

    /*
    float
        x1 = 1, y1 = 0, z1 = 0,
        x2 = 0, y2 = 1, z2 = 0,
        x3 = 0, y3 = 0, z3 = -1;

    

    float3x3 matr = float3x3(
        x1, y1, z1,
        x2, y2, z2,
        x3, y3, z3
    );

    float3 scaled = mul(matr, float3(uv, 0));

    Out = mul(rotation_matrix, scaled);
    */
    return;
}

#endif
