using System;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Utilities
{
    /// <summary>
    /// Put this attribute on properties alongside the <see cref="SerializeReference"/>-attribute
    /// to draw a script selector box and property fields for all its values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ScriptSelectorAttribute : PropertyAttribute
    {
    }
}
