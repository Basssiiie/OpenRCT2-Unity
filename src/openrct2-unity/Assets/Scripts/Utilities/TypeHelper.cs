using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace OpenRCT2.Unity.Utilities
{
    public static class TypeHelper
    {
        public static void Describe<T>()
        {
            var type = typeof(T);
            var builder = new StringBuilder();

            builder.Append("[");
            builder.Append(type.Name);
            builder.Append("] Size = ");
            builder.Append(Marshal.SizeOf(type));
            builder.AppendLine();

            foreach (var field in type.GetFields())
            {
                var fieldName = field.Name;
                builder.Append("- ");
                builder.Append(Marshal.OffsetOf<T>(fieldName));
                builder.Append(" \t-> ");
                builder.AppendLine(fieldName);
            }

            Debug.Log(builder.ToString());
        }
    }
}
