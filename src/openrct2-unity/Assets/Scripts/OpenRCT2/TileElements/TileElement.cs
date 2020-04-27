using System.Runtime.InteropServices;

namespace OpenRCT2.Unity
{
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct TileElement
    {
        public byte type;
        public byte flags;
        public byte baseHeight;
        public byte clearanceHeight;

        public byte slot0x1;
        public byte slot0x2;
        public byte slot0x3;
        public byte slot0x4;

        public byte slot0x5;
        public byte slot0x6;
        public byte slot0x7;
        public byte slot0x8;

        public byte slot0x9;
        public byte slot0xA;
        public byte slot0xB;
        public byte slot0xC;


        // The mask skim off the extra bits, to retrieve the actual type.
        const byte TypeMask = 0b00111100;


        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public TileElementType Type
            => (TileElementType)(type & TypeMask);
    }
}
