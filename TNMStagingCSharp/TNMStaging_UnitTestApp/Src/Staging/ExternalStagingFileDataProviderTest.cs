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
    [TestClass]
    public class ExternalStagingFileDataProviderTest : FileDataProviderTest
    {
        private static TNMStagingCSharp.Src.Staging.Staging _STAGING;
        private static String _basePath = string.Empty;


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String relPath = "\\..\\..\\..\\";
            if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) relPath += "\\..\\";

            _basePath = Path.GetFullPath(System.IO.Directory.GetCurrentDirectory() + relPath);

            String sFilePath = _basePath + "Resources\\Test\\external_algorithm.zip";

            FileStream SourceStream = File.Open(sFilePath, FileMode.Open);

            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
        }

        public override TNMStagingCSharp.Src.Staging.Staging getStaging()
        {
            return _STAGING;
        }

        /*
        [TestMethod]
        public void testConstructorWithPath()
        {
            string zipPath = Paths.get("src/test/resources/external_algorithm.zip");
            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(zipPath);
            Assert.IsTrue(provider.getAlgorithm().Length > 0);
            Assert.IsTrue(provider.getVersion().Length > 0);
            Assert.IsTrue(provider.getSchemaIds().Count > 0);
            Assert.IsTrue(provider.getTableIds().Count > 0);

            // test the direct Staging instance
            Staging staging = Staging.getInstance(zipPath);
            Assert.IsTrue(staging.getSchemaIds().Count > 0);
            Assert.IsTrue(staging.getTableIds().Count > 0);
        }
        */

        [TestMethod]
        public void testConstructorWithString()
        {
            String zipFileName = _basePath + "resources\\test\\external_algorithm.zip";
            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(zipFileName);
            Assert.IsTrue(provider.getAlgorithm().Length > 0);
            Assert.IsTrue(provider.getVersion().Length > 0);
            Assert.IsTrue(provider.getSchemaIds().Count > 0);
            Assert.IsTrue(provider.getTableIds().Count > 0);

            // test the direct Staging instance
            TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(zipFileName);
            Assert.IsTrue(staging.getSchemaIds().Count > 0);
            Assert.IsTrue(staging.getTableIds().Count > 0);
        }

        [TestMethod]
        public void testInvalidPathThrowsException() 
        {
            try
            {
                string invalidPath = _basePath + "\\resources\\test\\missing.zip";
                ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(invalidPath);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("missing.zip"));
            }
        }
    }
}

