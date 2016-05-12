﻿namespace SEToolbox.Interop
{
    using Support;
    using System;
    using System.Reflection;
    using VRage.Utils;

    public class SpaceEngineersConsts
    {
        /// <summary>
        /// Thumbnail image of last position in save.
        /// </summary>
        public const string ThumbnailImageFilename = "thumb.jpg";

        /// <summary>
        /// Contains summary of save content.
        /// </summary>
        public const string SandBoxCheckpointFilename = "Sandbox.sbc";

        /// <summary>
        /// Contains main content.
        /// </summary>
        public const string SandBoxSectorFilename = "SANDBOX_0_0_0_.sbs";

        /// <summary>
        /// Contains list of save 'worlds'.
        /// </summary>
        public const string LoadLoadedFilename = "LastLoaded.sbl";

        // Current set max speed m/s for Ships.
        public const float MaxShipVelocity = 104.375f;

        // Current set max speed m/s for Players - as of update 01.023.
        public const float MaxPlayerVelocity = 111.531f;

        // Estimated max speed m/s for Meteors - as of update 01.024.
        public const float MaxMeteorVelocity = 202.812f;

        public const float PlayerMass = 100f;

        /// <summary>
        /// The base path of the save files, minus the userid.
        /// </summary>
        public static readonly UserDataPath BaseLocalPath;

        public static readonly UserDataPath BaseDedicatedServerHostPath;

        public static readonly UserDataPath BaseDedicatedServerServicePath;

        static SpaceEngineersConsts()
        {
            // Don't access the ObjectBuilders from the static ctor, as it will cause issues with the Serializer type loader.

            var basePath = "SpaceEngineers";
            //if (GlobalSettings.Default.SEBinPath.Contains("MedievalEngineers", StringComparison.InvariantCulture))
            //    basePath = "MedievalEngineers";

            BaseLocalPath = new UserDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), basePath + @"\Saves", basePath + @"\Mods"); // Followed by .\%SteamuserId%\LastLoaded.sbl
            BaseDedicatedServerHostPath = new UserDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), basePath + @"Dedicated\Saves", basePath + @"Dedicated\Mods"); // Followed by .\LastLoaded.sbl
            BaseDedicatedServerServicePath = new UserDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), basePath + @"Dedicated", ""); // Followed by .\%instancename%\Saves\LastLoaded.sbl  (.\%instancename%\Mods)
        }

        public static Version GetSEVersion()
        {
            try
            {
                return new Version(new MyVersion(GetSEVersionInt()).FormattedText.ToString().Replace("_", "."));
            }
            catch
            {
                return new Version();
            }
        }

        public static int GetSEVersionInt()
        {
            try
            {
                // SE_VERSION is a private constant. Need to use reflection to get it. 
                FieldInfo field = typeof(SpaceEngineers.Game.SpaceEngineersGame).GetField("SE_VERSION", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                return (int)field.GetValue(null);
            }
            catch
            {
                return 0;
            }
        }
    }
}
