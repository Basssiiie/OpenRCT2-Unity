#ifndef PathTransformer
#define PathTransformer


static const float2 aspect_ratio = float2(64, 31); // max pixel width+height of a tile.
static const float scale = 1 / 1.41421356237; // diagonal tile distance.
static const float radians = 0.785398163; // 45 degrees in radians.


static const float3x3 rotation_matrix = float3x3 // this is correct now for rotating in 3d
(
    0.707107, -0.3535535, -0.6123725,
    0.707107,  0.3535535,  0.6123725,
    0.,       -0.8660253,  0.5
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
float2 sprite_scale_to_fit(float2 uv)
{
    return (uv - 0.5f) * scale + 0.5f;
}


void sprite_scale_to_fit_float(float2 uv, out float2 Out)
{
    Out = sprite_scale_to_fit(uv);
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
 * Rotates the sprite 45 degrees with the matrix.
 * 
 * uv:       the input uvs
 * size:     size in pixels, of the sprite.
 * offset:   the sprite offset, as supplied by the .dat file.
 * returns:  the output uvs.
 */
float2 sprite_rotate(float2 uv)
{
    return mul(rotation_matrix, float3(uv, 0));
    //return mul(rot_matrix, uv);;
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
    //float2 rct_offset = sprite_add_offset(uv, size, offset);
    //float2 scaled = sprite_scale_to_fit(rct_offset);

    float3x3 a = float3x3
    (
        (uv.x - 0.5f) * size.x, 0, 0,
        0, (uv.y - 0.5f) * size.y, 0,
        0, 0, 1
    );    
    float3x3 b = float3x3
    (
        (size.x * aspect_ratio.x) + 0.5f, 0, 0,
        0, (size.y * aspect_ratio.y) + 0.5f, 0,
        0, 0, 1
    );
    
    //float2 pixels = uv_to_pixel(uv, size);
    //float2 rotated = sprite_rotate(pixels);
    //float2 result = pixel_to_uv(rotated, size);

    float3x3 m = mul(rotation_matrix, mul(a, b));
    float3 result = float3(uv, 1);
    
    Out = mul(m, result);
    return;
    
    float width_multiplier = (size.x / aspect_ratio.x);
    float height_multiplier = (size.y / aspect_ratio.y);
    float3 resized_uv = float3(uv.x / width_multiplier, uv.y / height_multiplier, 1);

    float offset_x = (-offset.x / size.x);
    float offset_y = (-offset.y / size.y);

    float angle_cos = cos(radians) * scale;
    float angle_sin = sin(radians) * scale;
    
    float3x3 trs_matrix = float3x3
    (
        angle_cos, angle_sin, 0,
        -angle_sin, angle_cos, 0,
        0, 0, 1
    );

    Out = mul(trs_matrix, resized_uv);
}

#endif
