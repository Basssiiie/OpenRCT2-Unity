using System.Collections.Generic;
using Graphics;
using UnityEngine;

namespace Generation.Retro
{
    public partial class SurfaceGenerator 
    {
        [SerializeField] Shader _surfaceShader;
        [SerializeField] string _surfaceTextureField = "Surface";
        [SerializeField] Shader _edgeShader;
        [SerializeField] string _edgeTextureField = "Edge";
        [SerializeField] Shader _waterShader;
        [SerializeField] string _waterTextureField = "Water";
        [SerializeField] string _waterRefractionField = "WaterRefraction";


        const byte TypeSurface = 1;
        const byte TypeEdge = 2;
        const byte TypeWater = 3;


        List<RequestedImage> _images;


        /// <summary>
        /// Pushes a image index to the materials stack and returns its list index.
        /// </summary>
        int PushImageIndex(uint imageIndex, byte type)
        {
            int position = _images.FindIndex(i => i.ImageIndex == imageIndex);
            if (position == -1)
            {
                position = _images.Count;
                _images.Add(new RequestedImage(imageIndex, type));
            }
            return position;
        }


        /// <summary>
        /// Generates the required materials for the surface mesh.
        /// </summary>
        Material[] GenerateSurfaceMaterials()
        {
            int count = _images.Count;
            Material[] materials = new Material[count];

            for (int i = 0; i < count; i++)
            {
                RequestedImage image = _images[i];

                Graphic graphic = GraphicsFactory.ForImageIndex(image.ImageIndex);

                if (graphic == null)
                {
                    Debug.LogError($"Missing surface sprite image: {image.ImageIndex & 0x7FFFF}");
                    continue;
                }

                Texture2D texture = graphic.GetTexture(TextureWrapMode.Repeat);
                Material material;

                switch (image.Type)
                {
                    case TypeSurface:
                        material = new Material(_surfaceShader);
                        material.SetTexture(_surfaceTextureField, texture);
                        break;

                    case TypeEdge:
                        material = new Material(_edgeShader);
                        material.SetTexture(_edgeTextureField, texture);
                        break;

                    case TypeWater:
                        material = new Material(_waterShader);
                        material.SetTexture(_waterTextureField, texture);

                        // HACK: injection of the refraction sprite shouldnt be here.
                        Graphic refraction = GraphicsFactory.ForImageIndex(WaterRefractionImageIndex);
                        if (refraction == null)
                        {
                            Debug.LogError($"Missing water refraction sprite image: {WaterRefractionImageIndex}");
                            continue;
                        }

                        material.SetTexture(_waterRefractionField, refraction.GetTexture(TextureWrapMode.Repeat));
                        break;

                    default:
                        Debug.LogWarning("Could not parse this image type: " + image.Type);
                        continue;
                }

                materials[i] = material;
            }

            return materials;
        }


        readonly struct RequestedImage
        {
            public readonly uint ImageIndex;
            public readonly byte Type;


            public RequestedImage(uint imageIndex, byte type)
            {
                ImageIndex = imageIndex;
                Type = type;
            }


            public override bool Equals(object obj)
                => (obj is RequestedImage image && ImageIndex == image.ImageIndex);


            public override int GetHashCode()
                => (int)ImageIndex;
        }
    }
}
