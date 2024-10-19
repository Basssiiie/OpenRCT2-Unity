using System.IO;
using OpenRCT2.Generators.Sprites;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace EditorExtensions
{
    /// <summary>
    /// Editor window that can browse the sprites in OpenRCT2.
    /// Tip: sprites.h in the original source code has some example indices.
    /// </summary>
    public class SpriteViewerWindow : EditorWindow
    {
        int _imageIndexOffset = 0;
        int _horizontalCount = 10;
        int _verticalCount = 10;
        Vector3Int _colours;
        Vector2 _scrollPosition;


        [MenuItem("OpenRCT2/Sprite Viewer")]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            SpriteViewerWindow window = GetWindow<SpriteViewerWindow>("OpenRCT2 Sprite Viewer");
            window.Show();
        }


        void OnGUI()
        {
            _imageIndexOffset = Mathf.Clamp(EditorGUILayout.IntField("Image index offset", _imageIndexOffset), 0, 0x7FFFE);
            _colours = EditorGUILayout.Vector3IntField("Colours", _colours);
            _colours.Clamp(Vector3Int.zero, new Vector3Int(255, 255, 255));
            _horizontalCount = EditorGUILayout.IntSlider("Horizontal sprite count", _horizontalCount, 1, 20);
            _verticalCount = EditorGUILayout.IntSlider("Vertical sprite count", _verticalCount, 1, 20);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous page"))
                _imageIndexOffset -= (_horizontalCount * _verticalCount);
            if (GUILayout.Button("Next page"))
                _imageIndexOffset += (_horizontalCount * _verticalCount);

            GUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            uint offset = (uint)_imageIndexOffset;

            for (int y = 0; y < _verticalCount; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < _horizontalCount; x++)
                {
                    SpriteTexture graphic = SpriteFactory.GetOrCreate(offset, (byte)_colours.x, (byte)_colours.y, (byte)_colours.z);
                    
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));

                    if (graphic.pixelCount == 0)
                    {
                        GUILayout.Box("Empty sprite");
                    }
                    else
                    {
                        Texture2D texture = TextureFactory.CreateFullColour(graphic, makeTextureReadable: true);
                        GUIContent icon = EditorGUIUtility.IconContent("SavePassive", "Save sprite");

                        GUILayout.BeginHorizontal();
                        GUILayout.Box(texture);
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                            DownloadSprite(texture, $"sprite{offset}({graphic.offsetX},{graphic.offsetY})");

                        GUILayout.EndHorizontal();
                    }

                    string information = $"Index:\t{offset}\nSize:\t{graphic.width}x{graphic.height} px\nOffset:\t({graphic.offsetX}, {graphic.offsetY})";
                    GUILayout.Label(information, new GUIStyle { alignment = TextAnchor.LowerLeft }, GUILayout.ExpandHeight(true));

                    GUILayout.EndVertical();

                    offset++;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Saves the texture file to disk.
        /// </summary>
        static void DownloadSprite(Texture2D texture, string name)
        {
            string filename = $"{name}.png";
            string path = EditorUtility.SaveFilePanel("Save sprite", "", filename, "png");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Debug.Log($"Saving texture '{filename}' to path: {path}");

            byte[] pngBytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngBytes);
        }
    }
}
