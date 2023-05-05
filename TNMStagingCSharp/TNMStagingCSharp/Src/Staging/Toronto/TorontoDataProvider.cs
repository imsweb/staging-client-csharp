/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Toronto
{

    public class TorontoVersion
    {

        public static readonly TorontoVersion LATEST = new TorontoVersion("0.1");
        public static readonly TorontoVersion V0_1 = new TorontoVersion("0.1");

        public static IEnumerable<TorontoVersion> Values
        {
            get
            {
                yield return LATEST;
                yield return V0_1;
            }
        }

        private readonly String _version;

        TorontoVersion(String version)
        {
            this._version = version;
        }

        public String getVersion()
        {
            return _version;
        }
    }


    public sealed class TorontoDataProvider : StagingFileDataProvider
    {
        private static readonly Dictionary<TorontoVersion, TorontoDataProvider> _PROVIDERS = new Dictionary<TorontoVersion, TorontoDataProvider>();

        //========================================================================================================================
        // Construct a Toronto data provider with the passed version
        // @param version version to initialize
        //========================================================================================================================
        private TorontoDataProvider(TorontoVersion version) : base("toronto", version.getVersion())
        {
        }

        //========================================================================================================================
        // Return the Toronto provider for the latest version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static TorontoDataProvider getInstance()
        {
            return getInstance(TorontoVersion.LATEST);
        }

        //========================================================================================================================
        // Return the Toronto provider for a specified version
        // @param version Toronto version
        // @return the data provider
        //========================================================================================================================
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static TorontoDataProvider getInstance(TorontoVersion version)
        {
            TorontoDataProvider provider;
            if (!_PROVIDERS.TryGetValue(version, out provider))
            {
                provider = new TorontoDataProvider(version);
                _PROVIDERS[version] = provider;
            }

            return provider;
        }
    }


}
