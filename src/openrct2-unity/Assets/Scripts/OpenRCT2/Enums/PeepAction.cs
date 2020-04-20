namespace OpenRCT2.Unity
{
    public enum PeepAction : byte
    {
        Check_Time = 0,
        // If no food then check watch
        Eat_Food = 1,
        Shake_Head = 2,
        Empty_Pockets = 3,
        Sitting_Eat_Food = 4,
        Sitting_Check_Watch = 4,
        Sitting_Look_Around_Left = 5,
        Sitting_Look_Around_Right = 6,
        Wow = 7,
        Throw_Up = 8,
        Jump = 9,
        Staff_Sweep = 10,
        Drowning = 11,
        Staff_Answer_Call = 12,
        Staff_Answer_Call_2 = 13,
        Staff_Checkboard = 14,
        Staff_Fix = 15,
        Staff_Fix_2 = 16,
        Staff_Fix_Ground = 17,
        Staff_Fix_3 = 18,
        Staff_Watering = 19,
        Joy = 20,
        Read_Map = 21,
        Wave = 22,
        Staff_Empty_Bin = 23,
        Wave_2 = 24,
        Take_Photo = 25,
        Clap = 26,
        Disgust = 27,
        Draw_Picture = 28,
        Being_Watched = 29,
        Withdraw_Money = 30,

        None_1 = 254,
        None_2 = 255
    };
}
