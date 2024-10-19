using System.Collections.Generic;
using OpenRCT2.Generators.Map.Providers;

#nullable enable

namespace OpenRCT2.Behaviours.Generators.Objects
{
    public static class ProviderHelper
    {
        public static IReadOnlyDictionary<string, IObjectProvider<T>> CreateLookup<T>(ProviderObject<T>[] providers) where T : struct
        {
            var lookup = new Dictionary<string, IObjectProvider<T>>();

            foreach (var provider in providers)
            {
                var entries = provider.GetEntries();

                foreach (var identifier in entries.identifiers)
                {
                    lookup.Add(identifier, entries.provider);
                }
            }

            return lookup;
        }
    }
}
