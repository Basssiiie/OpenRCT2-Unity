using System;
using System.Collections;
using System.Collections.Generic;
using OpenRCT2.Behaviours.Generators;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours
{
    public class MapScript : MonoBehaviour
    {
        [SerializeField] TileElementGeneratorScript? _surfaceGenerator;
        [SerializeField] TileElementGeneratorScript? _pathGenerator;
        [SerializeField] TileElementGeneratorScript? _trackGenerator;
        [SerializeField] TileElementGeneratorScript? _smallSceneryGenerator;
        [SerializeField] TileElementGeneratorScript? _entranceGenerator;
        [SerializeField] TileElementGeneratorScript? _wallGenerator;
        [SerializeField] TileElementGeneratorScript? _largeSceneryGenerator;
        [SerializeField] TileElementGeneratorScript? _bannerGenerator;

        [SerializeField] GeneratorTypes _enabledGenerators;
        [SerializeField] LoaderScript? _loader;
        [SerializeField] Behaviour[] _controllers = Array.Empty<Behaviour>();


        public void Generate()
        {
            var generators = GetGenerators();
            var map = new MapGenerator(generators);
            var enumerator = LoadMap(map);

            if (_loader != null)
            {
                _loader.RunCoroutine(enumerator);
            }
            else
            {
                StartCoroutine(enumerator);
            }
        }


        IEnumerator LoadMap(MapGenerator map)
        {
            yield return new WaitForFixedUpdate();

            var enumerable = map.GenerateMap(transform);

            foreach (var status in enumerable)
            {
                if (_loader != null)
                {
                    _loader.SetLoader(status.text, status.progress, status.maximum);
                }
                yield return null;
            }

            foreach (var controller in _controllers)
            {
                controller.enabled = true;
            }
        }


        Dictionary<TileElementType, ITileElementGenerator> GetGenerators()
        {
            var generators = new Dictionary<TileElementType, ITileElementGenerator>();

            if (_surfaceGenerator != null && (_enabledGenerators & GeneratorTypes.Surface) != 0)
            {
                generators.Add(TileElementType.Surface, _surfaceGenerator.CreateGenerator());
            }
            if (_pathGenerator != null && (_enabledGenerators & GeneratorTypes.Path) != 0)
            {
                generators.Add(TileElementType.Path, _pathGenerator.CreateGenerator());
            }
            if (_trackGenerator != null && (_enabledGenerators & GeneratorTypes.Track) != 0)
            {
                generators.Add(TileElementType.Track, _trackGenerator.CreateGenerator());
            }
            if (_smallSceneryGenerator != null && (_enabledGenerators & GeneratorTypes.SmallScenery) != 0)
            {
                generators.Add(TileElementType.SmallScenery, _smallSceneryGenerator.CreateGenerator());
            }
            if (_entranceGenerator != null && (_enabledGenerators & GeneratorTypes.Entrance) != 0)
            {
                generators.Add(TileElementType.Entrance, _entranceGenerator.CreateGenerator());
            }
            if (_wallGenerator != null && (_enabledGenerators & GeneratorTypes.Wall) != 0)
            {
                generators.Add(TileElementType.Wall, _wallGenerator.CreateGenerator());
            }
            if (_largeSceneryGenerator != null && (_enabledGenerators & GeneratorTypes.LargeScenery) != 0)
            {
                generators.Add(TileElementType.LargeScenery, _largeSceneryGenerator.CreateGenerator());
            }
            if (_bannerGenerator != null && (_enabledGenerators & GeneratorTypes.Banner) != 0)
            {
                generators.Add(TileElementType.Banner, _bannerGenerator.CreateGenerator());
            }

            return generators;
        }

        [Flags]
        enum GeneratorTypes : byte
        {
            Surface = (1 << 0),
            Path = (1 << 1),
            Track = (1 << 2),
            SmallScenery = (1 << 3),
            Entrance = (1 << 4),
            Wall = (1 << 5),
            LargeScenery = (1 << 6),
            Banner = (1 << 7),
        }
    }
}
