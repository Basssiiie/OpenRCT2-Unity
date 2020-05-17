namespace OpenRCT
{
    /// <summary>
    /// Static helper class for pointer information.
    /// </summary>
    public static class Ptr
    {
        /// <summary>
        /// A constant of the current pointer size; 8 bytes on 64-bit, or 4 bytes on 32-bit.
        /// </summary>
        public const byte Size =
#if (UNITY_64 || UNITY_EDITOR_64)
            8;
#else
            4;
#endif
    }
}
