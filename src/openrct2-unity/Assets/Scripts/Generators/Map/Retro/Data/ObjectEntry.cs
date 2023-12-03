using System;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro.Data
{
    /// <summary>
    /// Object that makes it possible to assign meshes to object entries, like
    /// scenery or banner entries.
    /// </summary>
    public readonly struct ObjectEntry
    {
        public readonly GameObject? prefab;
        public readonly ObjectScaleMode scaleMode;
        public readonly string[] objectIds;


        public ObjectEntry(GameObject? prefab, ObjectScaleMode scaleMode, string[] objectIds)
        {
            this.prefab = prefab;
            this.scaleMode = scaleMode;
            this.objectIds = objectIds;
        }


        /// <summary>
        /// Checks whether the specified entry name matches any of the valid entries,
        /// and returns the prefab if it has found one.
        /// </summary>
        public bool IsMatch(string entryName)
        {
            for (int i = 0; i < objectIds.Length; i++)
            {
                if (string.Equals(entryName, objectIds[i], StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
