﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Array storing all the Random Seed group data.
    /// </summary>
    /// <remarks>size: 0x630</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class RandomGroup8b : SaveBlock
    {
        public const int COUNT_GROUP = 12;

        public RandomGroup8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

#pragma warning disable CA1819 // Properties should not return arrays
        public RandomSeed8b[] Seeds
        {
            get => GetSeeds();
            set => SetSeeds(value);
        }
#pragma warning restore CA1819 // Properties should not return arrays

        private RandomSeed8b[] GetSeeds()
        {
            var result = new RandomSeed8b[COUNT_GROUP];
            for (int i = 0; i < result.Length; i++)
                result[i] = new RandomSeed8b(Data, Offset + (i * RandomSeed8b.SIZE));
            return result;
        }

        private static void SetSeeds(IReadOnlyList<RandomSeed8b> value)
        {
            if (value.Count != COUNT_GROUP)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }
    }

    /// <summary>
    /// Random Seed data.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class RandomSeed8b
    {
        public const int GROUP_NAME_SIZE = 16; // chars
        public const int PERSON_NAME_SIZE = 32; // chars

        private const int OFS_GROUPNAME = 0; // 0x00
        private const int OFS_PLAYERNAME = OFS_GROUPNAME + (GROUP_NAME_SIZE * 2); // 0x20
        private const int OFS_GENDER = OFS_PLAYERNAME + (PERSON_NAME_SIZE * 2); // 0x60
        private const int OFS_REGION = OFS_GENDER + 4; // 0x68
        private const int OFS_SEED = OFS_REGION + 4; // 0x6C
        private const int OFS_RAND = OFS_SEED + 8; // 0x70
        private const int OFS_TICK = OFS_RAND + 8; // 0x78
        private const int OFS_UID = OFS_TICK + 8; // 0x80
        public const int SIZE = OFS_UID + 4; // 0x84

        private readonly int Offset;
        private readonly byte[] Data;

        public RandomSeed8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public string GroupName
        {
            get => StringConverter.GetString7b(Data, Offset + OFS_GROUPNAME, GROUP_NAME_SIZE * 2);
            set => StringConverter.SetString7b(value, GROUP_NAME_SIZE, GROUP_NAME_SIZE).CopyTo(Data, Offset + OFS_GROUPNAME);
        }

        public string PlayerName
        {
            get => StringConverter.GetString7b(Data, Offset + OFS_PLAYERNAME, PERSON_NAME_SIZE * 2);
            set => StringConverter.SetString7b(value, PERSON_NAME_SIZE, PERSON_NAME_SIZE).CopyTo(Data, Offset + OFS_PLAYERNAME);
        }

        public bool Male { get => Data[Offset + OFS_GENDER] == 1; set => Data[Offset + OFS_GENDER] = (byte)(value ? 1 : 0); }

        public int RegionCode
        {
            get => BitConverter.ToInt32(Data, Offset + OFS_REGION);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_REGION);
        }

        public ulong Seed
        {
            get => BitConverter.ToUInt64(Data, Offset + OFS_SEED);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SEED);
        }

        public ulong Random
        {
            get => BitConverter.ToUInt64(Data, Offset + OFS_RAND);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_RAND);
        }

        public long Ticks
        {
            get => BitConverter.ToInt64(Data, Offset + OFS_TICK);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_TICK);
        }

        public int UserID
        {
            get => BitConverter.ToInt32(Data, Offset + OFS_UID);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_UID);
        }

        public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
        public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }
    }
}
