using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.Map.Providers;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    public class TrackGenerator : ITileElementGenerator
    {
        readonly IObjectProvider<TrackInfo> _defaultProvider;
        readonly IReadOnlyDictionary<string, IObjectProvider<TrackInfo>> _providers;

        readonly IObjectProvider<TunnelInfo> _defaultTunnelProvider;
        readonly IReadOnlyDictionary<string, IObjectProvider<TunnelInfo>> _tunnelProviders;

        public TrackGenerator(IObjectProvider<TrackInfo> defaultProvider, IReadOnlyDictionary<string, IObjectProvider<TrackInfo>> providers, IObjectProvider<TunnelInfo> defaultTunnelProvider, IReadOnlyDictionary<string, IObjectProvider<TunnelInfo>> tunnelProviders)
        {
            _defaultProvider = defaultProvider;
            _providers = providers;
            _defaultTunnelProvider = defaultTunnelProvider;
            _tunnelProviders = tunnelProviders;
        }



        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            return map.ForEach("Creating tracks...", (in Element<TrackInfo> element) =>
            {
                CreateElement(map, transform, element);
            });
        }

        void CreateElement(Map map, Transform parent, in Element<TrackInfo> element)
        {
            ref TrackInfo track = ref element.GetData();

            if (track.sequenceIndex != 0)
                return;

            float trackOffset = track.trackHeight * World.CoordsZMultiplier;
            if (track.inverted)
            {
                trackOffset *= 2;
            }

            Tile tile = element.tile;
            int x = tile.x;
            int y = tile.y;

            string identifier = "";// track.identifier.Trim(); // todo get track style
            if (!_providers.TryGetValue(identifier, out var provider))
            {
                provider = _defaultProvider;
            }

            var obj = provider.CreateObject(map, in element);
            if (obj == null)
            {
                return;
            }

            obj.isStatic = true;

            int baseHeight = element.info.baseHeight;
            Vector3 position = World.TileCoordsToUnity(x, y, baseHeight);
            Vector3 trackPosition = new Vector3(position.x, position.y + trackOffset, position.z);

            var rotation = Quaternion.Euler(0, 90 * element.info.rotation, 0);
            var transform = obj.transform;
            transform.parent = parent;
            transform.SetLocalPositionAndRotation(trackPosition, rotation);

            var surfaceHeight = GetSurfaceHeight(tile, out var surfaceIndex);
            if (surfaceHeight <= baseHeight)
            {
                return;
            }

            ref var surface = ref tile.surfaces[surfaceIndex];
            var tunnelInfo = new TunnelInfo(0, track.trackType, surface.edgeImageIndex, false, false);
            //var tunnelElement = new Element<TunnelInfo>(tile, element.info, element.index, element.offset,)

            // Generate tunnels..
            //var tunnel = _defaultTunnelProvider.CreateObject(map, in tunnelInfo);
            //if (obj == null)
            //{
            //    return;
            //}

            //obj.isStatic = true;

            //var tunnelTransform = tunnel.transform;
            //tunnelTransform.parent = parent;
            //tunnelTransform.SetLocalPositionAndRotation(position, rotation);
        }

        private int GetSurfaceHeight(Tile tile, out int surfaceIndex)
        {
            var elements = tile.elements;
            var length = elements.Length;
            var surfaceHeight = -1;
            var currentIndex = 0;

            surfaceIndex = -1;

            for (var i = 0; i < length; i++)
            {
                ref var element = ref elements[i];

                if (element.type == TileElementType.Surface)
                {
                    var currentHeight = element.baseHeight;
                    if (currentHeight > surfaceHeight)
                    {
                        surfaceHeight = currentHeight;
                        surfaceIndex = currentIndex;
                    }

                    currentIndex++;
                }
            }

            return surfaceHeight;
        }
    }
}
