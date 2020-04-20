namespace OpenRCT2.Unity
{
    public enum PeepSubState : byte
    {
        At_Entrance = 0,
        In_Entrance = 1,
        Free_Vehicle_Check = 2,
        Leave_Entrance = 3,
        Approach_Vehicle = 4,
        Enter_Vehicle = 5,
        On_Ride = 6,
        Leave_Vehicle = 7,
        Approach_Exit = 8,
        Ride_in_Exit = 9,
        Approach_Vehicle_Waypoints = 12,
        Approach_Exit_Waypoints = 13,
        Approach_Spiral_Slide = 14,
        On_Spiral_Slide = 15,
        Leave_Spiral_Slide = 16,
        Maze_Pathfinding = 17,
        Leave_Exit = 18,
        Shop_Approach = 19,
        Shop_Interact = 20,
        Shop_Leave = 21,
    };
}
