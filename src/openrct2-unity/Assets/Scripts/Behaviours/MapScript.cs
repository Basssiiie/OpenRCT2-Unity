using System.Collections;
using System.Collections.Generic;
using OpenRCT2.Behaviours.Generators;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map;
using OpenRCT2.Utilities;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField] LoaderScript? _loader;
        [SerializeField] UnityEvent? _onGenerationComplete;


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
        }


        Dictionary<TileElementType, ITileElementGenerator> GetGenerators()
        {
            var generators = new Dictionary<TileElementType, ITileElementGenerator>();

            if (_surfaceGenerator != null)
            {
                generators.Add(TileElementType.Surface, _surfaceGenerator.CreateGenerator());
            }
            if (_pathGenerator != null)
            {
                generators.Add(TileElementType.Path, _pathGenerator.CreateGenerator());
            }
            if (_trackGenerator != null)
            {
                generators.Add(TileElementType.Track, _trackGenerator.CreateGenerator());
            }
            if (_smallSceneryGenerator != null)
            {
                generators.Add(TileElementType.SmallScenery, _smallSceneryGenerator.CreateGenerator());
            }
            if (_entranceGenerator != null)
            {
                generators.Add(TileElementType.Entrance, _entranceGenerator.CreateGenerator());
            }
            if (_wallGenerator != null)
            {
                generators.Add(TileElementType.Wall, _wallGenerator.CreateGenerator());
            }
            if (_largeSceneryGenerator != null)
            {
                generators.Add(TileElementType.LargeScenery, _largeSceneryGenerator.CreateGenerator());
            }
            if (_bannerGenerator != null)
            {
                generators.Add(TileElementType.Banner, _bannerGenerator.CreateGenerator());
            }

            return generators;
        }
    }
}
