using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;
using TNMStagingCSharp.Src.Staging.Pediatric;

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

            String sFilePath = _basePath + "Resources\\external_algorithm.zip";

            FileStream SourceStream = File.Open(sFilePath, FileMode.Open);

            //ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(SourceStream);
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
            String zipFileName = _basePath + "resources\\external_algorithm.zip";
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
                string invalidPath = _basePath + "\\resources\\missing.zip";
                ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(invalidPath);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("missing.zip"));
            }
        }

        /*
        [TestMethod]
        public void testMisc()
        {
            String zipFileName = _basePath + "resources\\algorithms\\pediatric-1.3.zip";
            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(zipFileName);
            Assert.IsTrue(provider.getAlgorithm().Length > 0);
            Assert.IsTrue(provider.getVersion().Length > 0);
            Assert.IsTrue(provider.getSchemaIds().Count > 0);
            Assert.IsTrue(provider.getTableIds().Count > 0);

            // test the direct Staging instance
            TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(zipFileName);
            Assert.IsTrue(staging.getSchemaIds().Count > 0);
            Assert.IsTrue(staging.getTableIds().Count > 0);

            // Hepatoblastoma is indeed Pediatric ID 7a
            // - only C220 with 8970/3 go to this schema.

            // Renal Tumor: Rhabdoid Renal Tumor is 6a2
            // - C649 with 8963/3

            // a. site= C649, hist=8963. beh=3
            // b. site= C220, hist=8970, beh=3

            Debug.WriteLine($"=== Test 1 ===");
            PediatricStagingData data = new PediatricStagingData.PediatricStagingInputBuilder()
                    .withInput(PediatricInput.PRIMARY_SITE, "C649")
                    .withInput(PediatricInput.HISTOLOGY, "8963")
                    .withInput(PediatricInput.YEAR_DX, "2026")
                    //.withInput(PediatricInput.AGE_DX, "18")
                    .withInput(PediatricInput.BEHAVIOR, "3")
                    .build();

            Debug.WriteLine($"Inputs:");
            foreach (KeyValuePair<string, string> entry in data.getInput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }

            // perform the staging
            staging.stage(data);

            Debug.WriteLine($"Result = {data.getResult()}");
            foreach (Error error in data.getErrors())
            {
                Debug.WriteLine($"Error = {error.getMessage()}");
            }

            Debug.WriteLine($"Outputs:");
            foreach (KeyValuePair<string, string> entry in data.getOutput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }

            Debug.WriteLine($"=== Test 2 ===");
            PediatricStagingData data2 = new PediatricStagingData.PediatricStagingInputBuilder()
                    .withInput(PediatricInput.PRIMARY_SITE, "C220")
                    .withInput(PediatricInput.HISTOLOGY, "8970")
                    .withInput(PediatricInput.YEAR_DX, "2026")
                    //.withInput(PediatricInput.AGE_DX, "18")
                    .withInput(PediatricInput.BEHAVIOR, "3")
                    .build();

            Debug.WriteLine($"Inputs:");
            foreach (KeyValuePair<string, string> entry in data2.getInput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }

            // perform the staging
            staging.stage(data2);

            Debug.WriteLine($"Result = {data2.getResult()}");
            foreach (Error error in data2.getErrors())
            {
                Debug.WriteLine($"Error = {error.getMessage()}");
            }

            Debug.WriteLine($"Outputs:");
            foreach (KeyValuePair<string, string> entry in data2.getOutput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }

            // - site =C569
            // - hist = 9060
            // - ydx=2026
            // - beh = 3
            // the pediatric ID = 10c2

            Debug.WriteLine($"=== Test 3 ===");
            PediatricStagingData data3 = new PediatricStagingData.PediatricStagingInputBuilder()
                    .withInput(PediatricInput.PRIMARY_SITE, "C569")
                    .withInput(PediatricInput.HISTOLOGY, "9060")
                    .withInput(PediatricInput.YEAR_DX, "2026")
                    .withInput(PediatricInput.AGE_DX, "18")
                    .withInput(PediatricInput.BEHAVIOR, "3")
                    .build();

            Debug.WriteLine($"Inputs:");
            foreach (KeyValuePair<string, string> entry in data3.getInput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }

            // perform the staging
            staging.stage(data3);

            Debug.WriteLine($"Result = {data3.getResult()}");
            foreach (Error error in data3.getErrors())
            {
                Debug.WriteLine($"Error = {error.getMessage()}");
            }

            Debug.WriteLine($"Outputs:");
            foreach (KeyValuePair<string, string> entry in data3.getOutput())
            {
                Debug.WriteLine($"  {entry.Key} = {entry.Value}");
            }
        }
        */

        [TestMethod]
        void testGlossaryFromInMemoryZip() throws IOException 
        {
            ExternalStagingFileDataProvider provider = provider(
                "tables/site.json",
                tableJson("site", "TESTING", "1.0"),
                "glossary/cortex.json",
                "{\"name\":\"Cortex\",\"definition\":\"Outer tissue\",\"alternate_names\":[\"Cortical\"]}"
            );

            assertThat(provider.getAlgorithm()).isEqualTo("testing");
            assertThat(provider.getGlossaryTerms()).containsExactly("Cortex");
            assertThat(provider.getGlossaryDefinition("Cortex").getDefinition()).isEqualTo("Outer tissue");
            assertThat(provider.getGlossaryDefinition("missing")).isNull();
            assertThat(provider.getGlossaryMatches("The cortex is present")).extracting("term").containsExactly("Cortex");
            assertThat(provider.getGlossaryMatches("Cortexlike")).isEmpty();
        }

        [TestMethod]
        void testMalformedJsonIsRejected() throws IOException 
        {
            byte[] zip = zip("tables/broken.json", "{not-json");

            assertThatThrownBy(() -> new ExternalStagingFileDataProvider(new ByteArrayInputStream(zip))).isInstanceOf(
                IOException.class
            );
        }

        [TestMethod]
        void testArchiveRequiresAlgorithmData() throws IOException 
        {
            byte[] zip = zip("notes/readme.txt", "ignored", "glossary/term.json", "{\"name\":\"Term\"}");

            assertThatThrownBy(() -> new ExternalStagingFileDataProvider(new ByteArrayInputStream(zip)))
                .isInstanceOf(IllegalStateException.class)
                .hasMessage("Error initializing provider; only a single algorithm should be included in file");
        }

        [TestMethod]
        void testInconsistentAlgorithmsAndVersionsAreRejected() throws IOException 
        {
            byte[] algorithms = zip(
                "tables/one.json",
                tableJson("one", "FIRST", "1.0"),
                "tables/two.json",
                tableJson("two", "SECOND", "1.0")
            );
            assertThatThrownBy(() -> new ExternalStagingFileDataProvider(new ByteArrayInputStream(algorithms)))
                .isInstanceOf(IllegalStateException.class)
                .hasMessage("Error initializing provider; only a single algorithm should be included in file");

            byte[] versions = zip(
                "tables/one.json",
                tableJson("one", "TEST", "1.0"),
                "tables/two.json",
                tableJson("two", "TEST", "2.0")
            );
            assertThatThrownBy(() -> new ExternalStagingFileDataProvider(new ByteArrayInputStream(versions)))
                .isInstanceOf(IllegalStateException.class)
                .hasMessage("Error initializing provider; only a single version should be included in file");
        }

        private static ExternalStagingFileDataProvider provider(String... entries) throws IOException 
        {
            return new ExternalStagingFileDataProvider(new ByteArrayInputStream(zip(entries)));
        }

        private static byte[] zip(String... entries) throws IOException {
            ByteArrayOutputStream bytes = new ByteArrayOutputStream();
            try (ZipOutputStream zip = new ZipOutputStream(bytes)) {
                for (int i = 0; i < entries.length; i += 2) {
                    zip.putNextEntry(new ZipEntry(entries[i]));
                    zip.write(entries[i + 1].getBytes(StandardCharsets.UTF_8));
                    zip.closeEntry();
                }
            }
            return bytes.toByteArray();
        }

        private static String tableJson(String id, String algorithm, String version) 
        {
            return """
            {"id":"%s","algorithm":"%s","version":"%s","definition":[{"key":"value","type":"INPUT"}],"rows":[["1"]]}
            """.formatted(id, algorithm, version);
        }
    }
}

