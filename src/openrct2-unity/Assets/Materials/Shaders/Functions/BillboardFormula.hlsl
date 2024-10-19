#ifndef Billboard
#define Billboard


void BilboardVertex_float(float3 cameraDirection, float3 size, out float3 Out)
{
    float3 up = float3(0, 1, 0);
    float3 horizontalNormalised = normalize(cross(up, cameraDirection));
    float3 verticalNormalised = cross(cameraDirection, horizontalNormalised);

    float3 horizontal = horizontalNormalised * size.x;
    float3 vertical = verticalNormalised * size.y;
    float3 forward = cameraDirection * size.z;

    Out = horizontal + vertical + forward;
}

void BilboardRotation_float(float3 objectPosition, float3 cameraPosition, float rotation, float rotationsCount, out float Out)
{
    float3 directionToObject = normalize(objectPosition - cameraPosition);

    float3 up = float3(0, 1, 0);
    float3 startingNormal = float3(-0.7071, 0, 0.7071); // normal of rotation 0
    
    float crossProd = dot(cross(directionToObject, startingNormal), up);
    float dotProd = dot(directionToObject, startingNormal);

    float index = round(degrees(atan2(dotProd, crossProd)) / (-360 / rotationsCount));
    Out = (index + rotationsCount + rotation) % rotationsCount; // wrap potential negative numbers
}

#endif
