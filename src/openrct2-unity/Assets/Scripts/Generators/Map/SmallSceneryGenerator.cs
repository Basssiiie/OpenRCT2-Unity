using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.Map.Providers;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    public class SmallSceneryGenerator : ITileElementGenerator
    {
        readonly IObjectProvider<SmallSceneryInfo> _defaultProvider;
        readonly IReadOnlyDictionary<string, IObjectProvider<SmallSceneryInfo>> _providers;

        public SmallSceneryGenerator(IObjectProvider<SmallSceneryInfo> defaultProvider, IReadOnlyDictionary<string, IObjectProvider<SmallSceneryInfo>> providers)
        {
            _defaultProvider = defaultProvider;
            _providers = providers;
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            return map.ForEach("Creating small scenery...", (Tile tile, int index, in TileElementInfo element, in SmallSceneryInfo scenery) =>
            {
                CreateElement(transform, tile.x, tile.y, index, element, scenery);
            });
        }

        void CreateElement(Transform parent, int x, int y, int index, in TileElementInfo element, in SmallSceneryInfo scenery)
        {
            float pos_x = x;
            float pos_y = y;
            float height = element.baseHeight;

            // If not a full tile, move small scenery to the correct quadrant.
            if (!scenery.fullTile)
            {
                const float distanceToQuadrant = World.TileCoordsXYMultiplier / 4;
                byte quadrant = scenery.quadrant;

                switch (quadrant)
                {
                    case 0: pos_x -= distanceToQuadrant; pos_y -= distanceToQuadrant; break;
                    case 1: pos_x -= distanceToQuadrant; pos_y += distanceToQuadrant; break;
                    case 2: pos_x += distanceToQuadrant; pos_y += distanceToQuadrant; break;
                    case 3: pos_x += distanceToQuadrant; pos_y -= distanceToQuadrant; break;
                }
            }

            // Instantiate the element.
            string identifier = scenery.identifier.Trim();
            if (!_providers.TryGetValue(identifier, out var provider))
            {
                provider = _defaultProvider;
            }

            var obj = provider.CreateObject(x, y, index, in element, in scenery);
            obj.isStatic = true;

            var position = World.TileCoordsToUnity(pos_x, pos_y, height);
            var rotation = Quaternion.Euler(0, 90 * element.rotation + 90, 0);

            var transform = obj.transform;
            transform.parent = parent;
            transform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}
