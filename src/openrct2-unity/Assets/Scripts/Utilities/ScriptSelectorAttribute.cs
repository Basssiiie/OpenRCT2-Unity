using System;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Put this on attributes alongside the <see cref="SerializeReference"/>-attribute
    /// to draw a script selector box and property fields for all its values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ScriptSelectorAttribute : PropertyAttribute
    {
    }
}
