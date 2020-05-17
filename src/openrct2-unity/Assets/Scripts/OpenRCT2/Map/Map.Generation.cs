using System;
using Generation;
using Generation.Retro;
using UnityEngine;

namespace OpenRCT
{
    public partial class Map
    {
        /// <summary>
        /// Flags to select which elements to generate and which to ignore.
        /// </summary>
        [Flags]
        enum TileElementFlags
        {
            None = 0,
            Path = (1 << 0),
            Track = (1 << 1),
            SmallScenery = (1 << 2),
            Entrance = (1 << 3),
            Wall = (1 << 4),
            LargeScenery = (1 << 5),
            Banner = (1 << 6),
            All = ~0
        };

        [SerializeField] TileElementFlags generationFlags = TileElementFlags.All;

        [Header("Generators")]
        [SerializeReference] IElementGenerator surfaceGenerator = new SurfaceGenerator();
        [SerializeReference] IElementGenerator pathGenerator = new PrefabGenerator();
        [SerializeReference] IElementGenerator trackGenerator = new PrefabGenerator();
        [SerializeReference] IElementGenerator smallSceneryGenerator = new SmallSceneryGenerator();
        [SerializeReference] IElementGenerator entranceGenerator = new PrefabGenerator();
        [SerializeReference] IElementGenerator wallGenerator = new WallGenerator();
        [SerializeReference] IElementGenerator largeSceneryGenerator = new PrefabGenerator();
        [SerializeReference] IElementGenerator bannerGenerator = new PrefabGenerator();


        /// <summary>
        /// Returns all generators currently selected.
        /// </summary>
        IElementGenerator[] GetGenerators()
        { 
           return new IElementGenerator[]
           {
                surfaceGenerator,
                pathGenerator,
                trackGenerator,
                smallSceneryGenerator,
                entranceGenerator,
                wallGenerator,
                largeSceneryGenerator,
                bannerGenerator
           };
        }


        /// <summary>
        /// Generates the surface of the map.public 
        /// </summary>
        void GenerateMap()
        {
            IElementGenerator[] generators = GetGenerators();

            foreach (var generator in generators)
                generator.StartGenerator(this);

            int end = (Size - 1);
            for (int x = 1; x < end; x++)
            {
                for (int y = 1; y < end; y++)
                {
                    Tile tile = Tiles[x, y];
                    for (int e = 0; e < tile.Count; e++)
                    {
                        GenerateTileElement(x, y, ref tile.Elements[e]);
                    }
                }
            }

            foreach (var generator in generators)
                generator.FinishGenerator();
        }


        /// <summary>
        /// Generates a tile element based on the type of the given tile.
        /// </summary>
        void GenerateTileElement(int x, int y, ref TileElement tile)
        {
            switch (tile.Type)
            {
                case TileElementType.Surface:
                    surfaceGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.Path:
                    if ((generationFlags & TileElementFlags.Path) != 0)
                        pathGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.Track:
                    if ((generationFlags & TileElementFlags.Track) != 0)
                        trackGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.SmallScenery:
                    if ((generationFlags & TileElementFlags.SmallScenery) != 0)
                        smallSceneryGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.Entrance:
                    if ((generationFlags & TileElementFlags.Entrance) != 0)
                        entranceGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.Wall:
                    if ((generationFlags & TileElementFlags.Wall) != 0)
                        wallGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.LargeScenery:
                    if ((generationFlags & TileElementFlags.LargeScenery) != 0)
                        largeSceneryGenerator.CreateElement(x, y, ref tile);
                    break;

                case TileElementType.Banner:
                    if ((generationFlags & TileElementFlags.Banner) != 0)
                        bannerGenerator.CreateElement(x, y, ref tile);
                    break;
            }
        }
    }
}
