using System.IO;
using Graphics;
using Lib;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    /// <summary>
    /// Editor window that can browse the sprites in OpenRCT2.
    /// Tip: sprites.h in the original source code has some example indices.
    /// </summary>
    public class SpriteViewerWindow : EditorWindow
    {
        int imageIndexOffset = 0;
        int horizontalCount = 10;
        int verticalCount = 10;
        Vector2 scrollPosition;


        [MenuItem("OpenRCT2/Sprite Viewer")]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            SpriteViewerWindow window = GetWindow<SpriteViewerWindow>("OpenRCT2 Sprite Viewer");
            window.Show();
        }


        void OnGUI()
        {

            imageIndexOffset = Mathf.Clamp(EditorGUILayout.IntField("Image index offset", imageIndexOffset), 0, 0x7FFFE);
            horizontalCount = EditorGUILayout.IntSlider("Horizontal sprite count", horizontalCount, 1, 20);
            verticalCount = EditorGUILayout.IntSlider("Vertical sprite count", verticalCount, 1, 20);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous page"))
                imageIndexOffset -= (horizontalCount * verticalCount);
            if (GUILayout.Button("Next page"))
                imageIndexOffset += (horizontalCount * verticalCount);

            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            uint offset = (uint)imageIndexOffset;

            for (int y = 0; y < verticalCount; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < horizontalCount; x++)
                {
                    Graphic graphic = GraphicsFactory.ForImageIndex(offset);

                    SpriteData data = OpenRCT2.GetTextureData(offset);
                    string information = $"Index:\t{offset}\nSize:\t{data.width}x{data.height} px\nOffset:\t({data.offsetX}, {data.offsetY})";

                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));

                    if (graphic == null)
                    {
                        GUILayout.Box("Not found");
                    }
                    else
                    {
                        Texture2D texture = graphic.GetTexture();
                        GUIContent icon = EditorGUIUtility.IconContent("SavePassive", "Save sprite");

                        GUILayout.BeginHorizontal();
                        GUILayout.Box(texture);
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                            DownloadSprite(texture, $"sprite{offset}({data.offsetX},{data.offsetY})");

                        GUILayout.EndHorizontal();
                    }

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


        void Awake()
        {
            // Only start if currently not already running OpenRCT2.
            if (!EditorApplication.isPlaying)
                OpenRCT2.StartGame();
        }


        void OnDestroy()
        {
            // Only stop if currently not already running OpenRCT2.
            if (!EditorApplication.isPlaying)
                OpenRCT2.StopGame();
        }
    }
}
