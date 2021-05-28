using System;
using UnityEngine;

#nullable enable

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
		public static SurfaceElement AsSurface(this in TileElement tileElement)
			=> new SurfaceElement(tileElement);


        /// <summary>
        /// Wraps this tile element in a path element wrapper.
        /// </summary>
        public static PathElement AsPath(this in TileElement tileElement)
			=> new PathElement(tileElement);


        /// <summary>
        /// Wraps this tile element in a track element wrapper.
        /// </summary>
        public static TrackElement AsTrack(this in TileElement tileElement)
			=> new TrackElement(tileElement);


        /// <summary>
        /// Wraps this tile element in a small scenery element wrapper.
        /// </summary>
        public static SmallSceneryElement AsSmallScenery(this in TileElement tileElement)
			=> new SmallSceneryElement(tileElement);


        /// <summary>
        /// Wraps this tile element in a wall element wrapper.
        /// </summary>
        public static WallElement AsWall(this in TileElement tileElement)
            => new WallElement(tileElement);


        /// <summary>
        /// Dumps all values on this tile element to the console log.
        /// </summary>
		public static void DumpToConsole(this in TileElement tile)
            => Console.WriteLine(DumpToString(tile));


        /// <summary>
        /// Dumps all values on this tile element to the debug log.
        /// </summary>
        public static void DumpToDebug(this in TileElement tile)
            => Debug.Log(DumpToString(tile));


        /// <summary>
        /// Dumps the byte content of the tile element to a readable string.
        /// </summary>
        static string DumpToString(in TileElement tile)
        {
            return
$@"(me) TileElement:
(me)   -> {tile.type} \t(type)
(me)   -> {tile.flags} \t(flags)
(me)   -> {tile.baseHeight} \t(base_height)
(me)   -> {tile.clearanceHeight} \t(clearance_height)
(me)   -> {tile.owner} \t(owner)
(me)   -> {tile.slot0x1} \t(pad 0x1)
(me)   -> {tile.slot0x2} \t(pad 0x2)
(me)   -> {tile.slot0x3} \t(pad 0x3)
(me)   -> {tile.slot0x4} \t(pad 0x4)
(me)   -> {tile.slot0x5} \t(pad 0x5)
(me)   -> {tile.slot0x6} \t(pad 0x6)
(me)   -> {tile.slot0x7} \t(pad 0x7)
(me)   -> {tile.slot0x8} \t(pad 0x8)
(me)   -> {tile.slot0x9} \t(pad 0x9)
(me)   -> {tile.slot0xA} \t(pad 0xA)
(me)   -> {tile.slot0xB} \t(pad 0xB)";
        }
	}
}
