using System;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// Object that makes it possible to assign meshes to object entries, like
    /// scenery or banner entries.
    /// </summary>
    [CreateAssetMenu(menuName = ("OpenRCT2/Objects/" + nameof(ObjectEntry)))]
    public class ObjectEntry : ScriptableObject
    {
        public GameObject prefab;
        public ObjectScaleMode scaleMode;

        [ContextMenuItem("Sort alphabetically ", nameof(SortObjectIds))]
        [SerializeField] string[] _objectIds;


        /// <summary>
        /// Checks whether the specified entry name matches any of the valid entries,
        /// and returns the prefab if it has found one.
        /// </summary>
        public bool IsMatch(string entryName)
        {
            for (int i = 0; i < _objectIds.Length; i++)
            {
                if (string.Equals(entryName, _objectIds[i], StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
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
