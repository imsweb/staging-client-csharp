// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.CS
{

    public class CsVersion
    {

        public static readonly CsVersion LATEST = new CsVersion("02.05.50");
        public static readonly CsVersion v020550 = new CsVersion("02.05.50");

        public static IEnumerable<CsVersion> Values
        {
            get
            {
                yield return LATEST;
                yield return v020550;
            }
        }

        private readonly String _version;

        CsVersion(String version)
        {
            this._version = version;
        }

        public String getVersion()
        {
            return _version;
        }
    }


    public sealed class CsDataProvider: StagingFileDataProvider
    {
        private static readonly Dictionary<CsVersion, CsDataProvider> _PROVIDERS = new Dictionary<CsVersion, CsDataProvider>();

        //========================================================================================================================
        // Construct a CS data provider with the passed version
        // @param version version to initialize
        //========================================================================================================================
        private CsDataProvider(CsVersion version): base("cs", version.getVersion())
        {
        }

        //========================================================================================================================
        // Return the CS provider for the latest version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static CsDataProvider getInstance()
        {
            return getInstance(CsVersion.LATEST);
        }

        //========================================================================================================================
        // Return the CS provider for a specified version
        // @param version CS version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static CsDataProvider getInstance(CsVersion version)
        {
            CsDataProvider provider;
            if (!_PROVIDERS.TryGetValue(version, out provider)) 
            {
                provider = new CsDataProvider(version);
                _PROVIDERS[version] = provider;
            }

            return provider;
        }
    }
}

