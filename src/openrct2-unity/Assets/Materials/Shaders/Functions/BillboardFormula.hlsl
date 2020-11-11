#ifndef Billboard
#define Billboard


void Billboard_float(float3 position, float scale, out float3 Out)
{
    Out = mul(UNITY_MATRIX_P,
        mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
        + float4(position.x, position.y, 0.0, 0.0)
        * float4(scale, scale, 1.0, 1.0));
}


void Billboard2_float(float3 position, float2 uvs, float scale, out float3 Out)
{
    float3 upVector = float3(0, 1, 0);
    
    // Direction we are viewing the billboard from
    float3 viewDirection = UNITY_MATRIX_V._m02_m12_m22;
    float3 rightVector = normalize(cross(viewDirection, upVector));

	// Transform billboard normal for lighting support
	// Comment out this line to stop light changing as billboards rotate
    //v.normal = mul((float3x3) UNITY_MATRIX_V, v.normal);

	// Offset vertices based on corners scaled by size
    position += rightVector * (uvs.x - 0.5) * 1;
    position += upVector * (uvs.y - 0.5) * 1;

    Out = position;
}


void Billboard3_float()
{
    float3 _Camera_Direction = -1 * mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V))[2].xyz);

}

#endif
