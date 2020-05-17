namespace OpenRCT
{
    public enum PeepSubState : byte
    {
        AtEntrance = 0,
        InEntrance = 1,
        FreeVehicleCheck = 2,
        LeaveEntrance = 3,
        ApproachVehicle = 4,
        EnterVehicle = 5,
        OnRide = 6,
        LeaveVehicle = 7,
        ApproachExit = 8,
        RideinExit = 9,
        ApproachVehicleWaypoints = 12,
        ApproachExitWaypoints = 13,
        ApproachSpiralSlide = 14,
        OnSpiralSlide = 15,
        LeaveSpiralSlide = 16,
        MazePathfinding = 17,
        LeaveExit = 18,
        ShopApproach = 19,
        ShopInteract = 20,
        ShopLeave = 21,
    };
}
