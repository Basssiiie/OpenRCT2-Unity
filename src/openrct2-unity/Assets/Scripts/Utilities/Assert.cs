using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Assertions;

#nullable enable

namespace OpenRCT2.Utilities
{
    /// <summary>
    /// Small class to check if something is true or false
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the condition is not true.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool condition)
        {
            UnityEngine.Assertions.Assert.IsTrue(condition);
        }


        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the condition is not false.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool condition)
        {
            UnityEngine.Assertions.Assert.IsFalse(condition);
        }


        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the value is null.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull([NotNull] object? value)
        {
            UnityEngine.Assertions.Assert.IsNotNull(value);
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
        }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.


        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the value is null.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull([NotNull] object? value, string valueName)
        {
            UnityEngine.Assertions.Assert.IsNotNull(value, $"Value name: {valueName}");
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
        }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.


        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the value is null.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull([MaybeNull] object? value)
        {
            UnityEngine.Assertions.Assert.IsNotNull(value);
        }


        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the value is null.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull([MaybeNull] object? value, string valueName)
        {
            UnityEngine.Assertions.Assert.IsNotNull(value, $"Value name: {valueName}");
        }
    }
}
