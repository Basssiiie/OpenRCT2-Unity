using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro.Providers
{
    /// <summary>
    /// Unity object that can create a game object provider for a set of OpenRCT2 object identifiers.
    /// </summary>
    public abstract class ProviderObject<T> : ScriptableObject where T : struct
    {
        /// <summary>
        /// Get all identifiers and their object provider.
        /// </summary>
        public abstract (string[] identifiers, IObjectProvider<T> provider) GetEntries();
    }
}
