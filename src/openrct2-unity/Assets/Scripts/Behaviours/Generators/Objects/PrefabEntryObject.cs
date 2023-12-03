using System;
using OpenRCT2.Generators.Map.Retro.Data;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Generators.Objects
{
    /// <summary>
    /// Object that makes it possible to assign meshes to object entries, like
    /// scenery or banner entries.
    /// </summary>
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(PrefabEntryObject)))]
    public class PrefabEntryObject : ScriptableObject
    {
        [SerializeField, Required] GameObject? _prefab;
        [SerializeField] ObjectScaleMode _scaleMode;

        [ContextMenuItem("Sort Alphabetically ", nameof(SortObjectIds))]
        [SerializeField] string[] _objectIds = Array.Empty<string>();


        public ObjectEntry GetObjectEntry()
        {
            return new ObjectEntry(_prefab, _scaleMode, _objectIds);
        }


        /// <summary>
        /// Sorts all object ids alphabetically.
        /// </summary>
        void SortObjectIds()
        {
            Array.Sort(_objectIds);
        }
    }
}
