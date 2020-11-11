using System.Runtime.InteropServices;
using Sprites;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// The struct of a peep, which can be either a guest or a staff member.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Peep : ISprite
    {
        public readonly ushort idx;
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public readonly byte direction;
        public readonly uint imageId;

        //public readonly byte tshirtColour;
        //public readonly byte trousersColour;


        /// <summary>
        /// Returns an id of the peep, currently based on the sprite index.
        /// </summary>
        public ushort Id
            => idx;


        /// <summary>
        /// Returns the peep's position in Unity coordinates.
        /// </summary>
        public Vector3 Position
            => Map.CoordsToVector3(x, z, y);
    }
}
