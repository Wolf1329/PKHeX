﻿using System;
using static PKHeX.Core.Gem8Version;

namespace PKHeX.Core
{
    /// <summary>
    /// Indicates various <see cref="SAV8BS"/> revision values which change on each major patch update, updating the structure of the stored player save data.
    /// </summary>
    public enum Gem8Version
    {
        /// <summary>
        /// Initial cartridge version shipped.
        /// </summary>
        /// <remarks><see cref="SaveUtil.SIZE_G8BDSP"/></remarks>
        V1_0 = 0x25, // 37

        /// <summary>
        /// Pre-release patch.
        /// </summary>
        /// <remarks><see cref="SaveUtil.SIZE_G8BDSP_1"/></remarks>
        V1_1 = 0x2C, // 44
    }

    public static class Gem8VersionExtensions
    {
        /// <summary>
        /// Returns a string to append to the savedata type info, indicating the revision of the player save data.
        /// </summary>
        /// <param name="version">Stored version value in the save data.</param>
        public static string GetSuffixString(this Gem8Version version) => version switch
        {
            V1_0 => "-1.0.0", // Launch Revision
            V1_1 => "-1.1.0", // 1.1.0
            _ => throw new ArgumentOutOfRangeException(nameof(version)),
        };
    }
}
