namespace Lib
{
    public enum PeepAction : byte
    {
        CheckTime = 0,
        // If no food then check watch
        EatFood = 1,
        ShakeHead = 2,
        EmptyPockets = 3,
        SittingEatFood = 4,
        SittingCheckWatch = 4,
        SittingLookAroundLeft = 5,
        SittingLookAroundRight = 6,
        Wow = 7,
        ThrowUp = 8,
        Jump = 9,
        StaffSweep = 10,
        Drowning = 11,
        StaffAnswerCall = 12,
        StaffAnswerCall2 = 13,
        StaffCheckboard = 14,
        StaffFix = 15,
        StaffFix_2 = 16,
        StaffFixGround = 17,
        StaffFix3 = 18,
        StaffWatering = 19,
        Joy = 20,
        ReadMap = 21,
        Wave = 22,
        StaffEmptyBin = 23,
        Wave2 = 24,
        TakePhoto = 25,
        Clap = 26,
        Disgust = 27,
        DrawPicture = 28,
        BeingWatched = 29,
        WithdrawMoney = 30,

        None1 = 254,
        None2 = 255
    };
}
