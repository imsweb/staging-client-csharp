using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;
using System.Linq;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    public class StagingFileDataProviderTest : FileDataProviderTest
    {
        private static TNMStagingCSharp.Src.Staging.Staging _STAGING;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(new StagingFileDataProvider("testing", "99.99"));
        }

        public override TNMStagingCSharp.Src.Staging.Staging getStaging()
        {
            return _STAGING;
        }
    }
}

