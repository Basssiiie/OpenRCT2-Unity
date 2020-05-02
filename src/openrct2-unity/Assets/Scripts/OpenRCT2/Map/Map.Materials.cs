using System.Collections.Generic;
using UnityEngine;

namespace OpenRCT2.Unity
{
    public partial class Map
    {
        [SerializeField] Shader surfaceShader;
        [SerializeField] string surfaceTextureField;
        [SerializeField] Shader edgeShader;
        [SerializeField] string edgeTextureField;


        const byte TypeSurface = 1;
        const byte TypeEdge = 2;
        const byte TypeWater = 3;


        readonly List<RequestedImage> images = new List<RequestedImage>();


        /// <summary>
        /// Pushes a image index to the materials stack and returns its mesh index.
        /// </summary>
        int PushImageIndex(uint imageIndex, byte type)
        {
            int position = images.FindIndex(i => i.ImageIndex == imageIndex);

            if (position != -1)
                return position;

            position = images.Count;
            images.Add(new RequestedImage(imageIndex, type));

            return position;
        }


        /// <summary>
        /// Generates the required materials for the surface mesh.
        /// </summary>
        /// <returns></returns>
        Material[] GenerateSurfaceMaterials()
        {
            int count = images.Count;
            Material[] materials = new Material[count];

            for (int i = 0; i < count; i++)
            {
                RequestedImage image = images[i];

                Texture2D texture = GraphicsFactory
                    .ForImageIndex(image.ImageIndex)
                    .ToTexture2D(TextureWrapMode.Repeat);

                Material material;

                switch (image.Type)
                {
                    case TypeSurface:
                    case TypeWater:
                        material = new Material(surfaceShader);
                        material.SetTexture(surfaceTextureField, texture);
                        break;

                    case TypeEdge:
                        material = new Material(edgeShader);
                        material.SetTexture(edgeTextureField, texture);
                        break;

                    default:
                        Debug.LogWarning("Could not parse this image type: " + image.Type);
                        continue;
                }

                materials[i] = material;
            }

            return materials;
        }


        void ResetImages()
        {
            images.Clear();
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
