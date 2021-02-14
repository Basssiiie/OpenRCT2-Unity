using System;
using UnityEngine;

#nullable enable

namespace Utilities
{
    /// <summary>
    /// Marks a property or field as required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RequiredAttribute : PropertyAttribute
    {
    }
}
