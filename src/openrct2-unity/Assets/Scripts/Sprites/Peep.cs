using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenRCT2.Unity
{
    [StructLayout(LayoutKind.Sequential, Size = 264)]
    public struct Peep
    {
        public SpriteBase sprite;
        public IntPtr namePtr; // The real name
        public int nextLocX;
        public int nextLocY;
        public int nextLocZ;
        public byte nextFlags;
        public byte outsideOfPark;
        public PeepState state;
        public PeepSubState substate;
        public PeepSpriteType spriteType;
        public PeepType type;
        public byte staffTypeOrNoOfRides; // union of staff type or no. of rides.
        public byte tshirtColour;
        public byte trousersColour;
        public ushort destinationX; // Location that the peep is trying to get to
        public ushort destinationY;
        public byte destinationTolerance; // How close to destination before next action/state 0 = exact
        public byte var37;
        public byte energy;
        public byte energyTarget;
        public byte happiness;
        public byte happinessTarget;
        public byte nausea;
        public byte nauseaTarget;
        public byte hunger;
        public byte thirst;
        public byte toilet;
        public byte mass;
        public byte timeToConsume;
        public byte intensity; // The max intensity is stored in the first 4 bits, and the min intensity in the second 4 bits
        public byte nauseaTolerance;
        public byte windowInvalidateFlags;
        public short paidOnDrink;
        public long rideTypesBeenOn1; // This is 16x a byte for a ride;
        public long rideTypesBeenOn2;
        public uint itemExtraFlags;
        public byte photo2RideRef;
        public byte photo3RideRef;
        public byte photo4RideRef;
        public byte currentRide;
        public byte stationIndex;
        public byte currentTrain;
        public ushort currentCarAndSeatOrTimeToSitDownOrTimeToStandAndStandingFlags;
        public byte specialSprite;
        public PeepActionSprite actionSpriteType;
        public PeepActionSprite nextActionSpriteType;
        public byte actionSpriteImageOffset;
        public PeepAction action;
        public byte actionFrame;
        public byte stepProgress;
        public ushort MechanicTimeSinceCallOrNextInQueue; // time getting to ride to fix
        public byte MazeLastEdgeOrDirection;
        public byte interactionRideIndex;
        public ushort timeInQueue;
        public long ridesBeenOn1;
        public long ridesBeenOn2;
        public long ridesBeenOn3;
        public long ridesBeenOn4;
        public uint Id2;
        public int cashInPocket;
        public int cashSpent;
        public int timeInPark;
        public byte rejoinQueueTimeOut;
        public byte previousRide;
        public ushort previousRideTimeOut;
        public uint peepThoughts;
        public byte pathCheckOptimisation;
        public byte staffIdOrGuestHeadingToRideId;
        public byte staffOrdersOrPeepIsLostCountdown;
        public byte photo1RideRef;
        public uint peepFlags;



        public ushort Id
            => sprite.spriteIndex;


        public string Name
            => Marshal.PtrToStringAuto(namePtr);


        public Vector3 Position
            => new Vector3(sprite.x, sprite.z, sprite.y);
    }
}
