/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Pediatric
{

    public class PediatricVersion
    {

        public static readonly PediatricVersion LATEST = new PediatricVersion("1.0");
        public static readonly PediatricVersion V1_0 = new PediatricVersion("1.0");

        public static IEnumerable<PediatricVersion> Values
        {
            get
            {
                yield return LATEST;
                yield return V1_0;
            }
        }

        private readonly String _version;

        PediatricVersion(String version)
        {
            this._version = version;
        }

        public String getVersion()
        {
            return _version;
        }
    }


    public sealed class PediatricDataProvider : StagingFileDataProvider
    {
        private static readonly Dictionary<PediatricVersion, PediatricDataProvider> _PROVIDERS = new Dictionary<PediatricVersion, PediatricDataProvider>();

        //========================================================================================================================
        // Construct a Pediatric data provider with the passed version
        // @param version version to initialize
        //========================================================================================================================
        private PediatricDataProvider(PediatricVersion version) : base("pediatric", version.getVersion())
        {
        }

        //========================================================================================================================
        // Return the Pediatric provider for the latest version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static PediatricDataProvider getInstance()
        {
            return getInstance(PediatricVersion.LATEST);
        }

        //========================================================================================================================
        // Return the Pediatric provider for a specified version
        // @param version Pediatric version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static PediatricDataProvider getInstance(PediatricVersion version)
        {
            PediatricDataProvider provider;
            if (!_PROVIDERS.TryGetValue(version, out provider))
            {
                provider = new PediatricDataProvider(version);
                _PROVIDERS[version] = provider;
            }

            return provider;
        }
    }


}
