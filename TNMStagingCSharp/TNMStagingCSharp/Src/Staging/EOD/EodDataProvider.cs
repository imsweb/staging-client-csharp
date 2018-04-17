/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.EOD
{

    public class EodVersion
    {

        public static readonly EodVersion LATEST = new EodVersion("1.0");
        public static readonly EodVersion v1_0 = new EodVersion("1.0");

        public static IEnumerable<EodVersion> Values
        {
            get
            {
                yield return LATEST;
                yield return v1_0;
            }
        }

        private readonly String _version;

        EodVersion(String version)
        {
            this._version = version;
        }

        public String getVersion()
        {
            return _version;
        }
    }


    public sealed class EodDataProvider : StagingFileDataProvider
    {
        private static readonly Dictionary<EodVersion, EodDataProvider> _PROVIDERS = new Dictionary<EodVersion, EodDataProvider>();

        //========================================================================================================================
        // Construct a EOD data provider with the passed version
        // @param version version to initialize
        //========================================================================================================================
        private EodDataProvider(EodVersion version) : base("eod_public", version.getVersion())
        {
        }

        //========================================================================================================================
        // Return the EOD provider for the latest version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static EodDataProvider getInstance()
        {
            return getInstance(EodVersion.LATEST);
        }

        //========================================================================================================================
        // Return the EOD provider for a specified version
        // @param version EOD version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static EodDataProvider getInstance(EodVersion version)
        {
            EodDataProvider provider;
            if (!_PROVIDERS.TryGetValue(version, out provider))
            {
                provider = new EodDataProvider(version);
                _PROVIDERS[version] = provider;
            }

            return provider;
        }
    }


}
