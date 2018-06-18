//Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.TNM
{

    public class TnmVersion
    {

        public static readonly TnmVersion LATEST = new TnmVersion("1.9");
        public static readonly TnmVersion v1_9 = new TnmVersion("1.9");

        public static IEnumerable<TnmVersion> Values
        {
            get
            {
                yield return LATEST;
                yield return v1_9;
            }
        }

        private readonly String _version;

        TnmVersion(String version)
        {
            this._version = version;
        }

        public String getVersion()
        {
            return _version;
        }
    }

    public sealed class TnmDataProvider : StagingFileDataProvider
    {
        private static readonly Dictionary<TnmVersion, TnmDataProvider> _PROVIDERS = new Dictionary<TnmVersion, TnmDataProvider>();

        // Construct a TNM data provider with the passed version
        // @param version version to initialize
        private TnmDataProvider(TnmVersion version): base("tnm", version.getVersion())
        {
        }

        // Return the TNM provider for the latest version
        // @return the data provider
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static TnmDataProvider getInstance()
        {
            return getInstance(TnmVersion.LATEST);
        }

        // Return the TNM provider for a specified version
        // @param version TNM version
        // @return the data provider
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static TnmDataProvider getInstance(TnmVersion version)
        {
            TnmDataProvider provider;
            if (!_PROVIDERS.TryGetValue(version, out provider))
            {
                provider = new TnmDataProvider(version);
                _PROVIDERS[version] = provider;
            }

            return provider;
        }
    }
}

