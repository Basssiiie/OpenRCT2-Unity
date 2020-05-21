using System;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// Extensions for a tile element struct.
    /// </summary>
	public static class TileElementExtensions
	{
        /// <summary>
        /// Wraps this tile element in a surface element wrapper.
        /// </summary>
		public static SurfaceElement AsSurface(this ref TileElement tileElement)
			=> new SurfaceElement(ref tileElement);


        /// <summary>
        /// Wraps this tile element in a path element wrapper.
        /// </summary>
        public static PathElement AsPath(this ref TileElement tileElement)
			=> new PathElement(ref tileElement);


        /// <summary>
        /// Wraps this tile element in a track element wrapper.
        /// </summary>
        public static TrackElement AsTrack(this ref TileElement tileElement)
			=> new TrackElement(ref tileElement);


        /// <summary>
        /// Wraps this tile element in a small scenery element wrapper.
        /// </summary>
        public static SmallSceneryElement AsSmallScenery(this ref TileElement tileElement)
			=> new SmallSceneryElement(ref tileElement);


        /// <summary>
        /// Wraps this tile element in a wall element wrapper.
        /// </summary>
        public static WallElement AsWall(this ref TileElement tileElement)
            => new WallElement(ref tileElement);


        /// <summary>
        /// Dumps all values on this tile element to the console log.
        /// </summary>
		public static void DumpToConsole(this ref TileElement tile)
            => Console.WriteLine(DumpToString(ref tile));


        /// <summary>
        /// Dumps all values on this tile element to the debug log.
        /// </summary>
        public static void DumpToDebug(this ref TileElement tile)
            => Debug.Log(DumpToString(ref tile));


        /// <summary>
        /// Dumps the byte content of the tile element to a readable string.
        /// </summary>
        static string DumpToString(ref TileElement tile)
        {
            return
$@"(me) TileElement:
(me)   -> {tile.type}   (type)
(me)   -> {tile.flags}  (flags)
(me)   -> {tile.baseHeight}     (base_height)
(me)   -> {tile.clearanceHeight}    (clearance_height)
(me)   -> {tile.slot0x1}    (pad 0x1)
(me)   -> {tile.slot0x2}    (pad 0x2)
(me)   -> {tile.slot0x3}    (pad 0x3)
(me)   -> {tile.slot0x4}    (pad 0x4)
(me)   -> {tile.slot0x5}    (pad 0x5)
(me)   -> {tile.slot0x6}    (pad 0x6)
(me)   -> {tile.slot0x7}    (pad 0x7)
(me)   -> {tile.slot0x8}    (pad 0x8)
(me)   -> {tile.slot0x9}    (pad 0x9)
(me)   -> {tile.slot0xA}    (pad 0xA)
(me)   -> {tile.slot0xB}    (pad 0xB)
(me)   -> {tile.slot0xC}    (pad 0xC)";
        }
	}
}
