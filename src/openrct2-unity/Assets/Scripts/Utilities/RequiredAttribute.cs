using System;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Utilities
{
    /// <summary>
    /// Marks a property or field as required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RequiredAttribute : PropertyAttribute
    {
    }
}
