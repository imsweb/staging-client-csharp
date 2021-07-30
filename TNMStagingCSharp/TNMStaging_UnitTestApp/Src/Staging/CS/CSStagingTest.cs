using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStaging_UnitTestApp.Src
{
    [TestClass]
    public class CsStagingTest : StagingTest
    {

        private TestContext testContextInstance;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(CsDataProvider.getInstance(CsVersion.v020550));

            /*
            String filename = "CS_02_05_50.zip";
            FileStream SourceStream = File.Open(filename, FileMode.Open);

            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
            */
        }

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        public override String getAlgorithm()
        {
            return "cs";
        }

        public override String getVersion()
        {
            return CsVersion.v020550.getVersion();
        }


        public override StagingFileDataProvider getProvider()
        {
            return CsDataProvider.getInstance(CsVersion.v020550);
        }

        [TestMethod]
        public void testBasicInitialization()
        {
            Assert.AreEqual(153, _STAGING.getSchemaIds().Count);
            Assert.IsTrue(_STAGING.getTableIds().Count > 0);

            Assert.IsNotNull(_STAGING.getSchema("urethra"));
            Assert.IsNotNull(_STAGING.getTable("extension_bdi"));
        }

        [TestMethod]
        public void testVersionInitializationTypes()
        {
            TNMStagingCSharp.Src.Staging.Staging staging020550 = TNMStagingCSharp.Src.Staging.Staging.getInstance(CsDataProvider.getInstance(CsVersion.v020550));
            Assert.AreEqual("02.05.50", staging020550.getVersion());

            TNMStagingCSharp.Src.Staging.Staging stagingLatest = TNMStagingCSharp.Src.Staging.Staging.getInstance(CsDataProvider.getInstance());
            Assert.AreEqual("02.05.50", stagingLatest.getVersion());
        }

        [TestMethod]
        public void testDescriminatorKeys()
        {
            HashSet<String> hash1 = new HashSet<String>() { "ssf25" };
            HashSet<String> hash2 = _STAGING.getSchema("nasopharynx").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "ssf25" };
            hash2 = _STAGING.getSchema("peritoneum_female_gen").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));
        }

        [TestMethod]
        public void testSchemaSelection() 
        {
            // test bad values
            List<Schema> lookup = _STAGING.lookupSchema(new SchemaLookup());
            Assert.AreEqual(0, lookup.Count);

            lookup = _STAGING.lookupSchema(new CsSchemaLookup("XXX", "YYY"));
            Assert.AreEqual(0, lookup.Count);

            // test valid combinations that do not require a discriminator
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", ""));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());
            Assert.AreEqual(122, lookup[0].getSchemaNum());
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", null));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());
            Assert.AreEqual(122, lookup[0].getSchemaNum());

            // now test one that does do AJCC7
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9100", ""));
            Assert.AreEqual(1, lookup.Count);

            // test value combinations that do not require a discriminator and are supplied 988
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", "988"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            // test valid combination that requires a discriminator but is not supplied one
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C111", "8200"));
            Assert.AreEqual(2, lookup.Count);

            HashSet<String> hash1 = null;
            HashSet<String> hash2 = null;
            foreach (Schema schema in lookup)
            {
                hash1 = new HashSet<String>() { "ssf25" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }

            // test valid combination that requires discriminator and a good discriminator is supplied
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C111", "8200", "010"));
            Assert.AreEqual(1, lookup.Count);
            foreach (Schema schema in lookup)
            {
                hash1 = new HashSet<String>() { "ssf25" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }

            Assert.AreEqual("nasopharynx", lookup[0].getId());
            Assert.AreEqual(34, lookup[0].getSchemaNum());

            // test valid combination that requires a discriminator but is supplied a bad disciminator value
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C111", "8200", "999"));
            Assert.AreEqual(0, lookup.Count);

            // test specific failure case:  Line #1995826 [C695,9701,100,lacrimal_gland] --> The schema selection should have found a schema, lacrimal_gland, but did not.
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C695", "9701", "100"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("lacrimal_gland", lookup[0].getId());
            Assert.AreEqual(138, lookup[0].getSchemaNum());

            // test searching on only site
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C401", null));
            Assert.AreEqual(5, lookup.Count);

            // test searching on only hist
            lookup = _STAGING.lookupSchema(new CsSchemaLookup(null, "9702"));
            Assert.AreEqual(2, lookup.Count);

            // test that searching on only ssf25 returns no results
            lookup = _STAGING.lookupSchema(new CsSchemaLookup(null, null, "001"));
            Assert.AreEqual(0, lookup.Count);
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("", null, "001"));
            Assert.AreEqual(0, lookup.Count);
            lookup = _STAGING.lookupSchema(new CsSchemaLookup(null, "", "001"));
            Assert.AreEqual(0, lookup.Count);
        }

        [TestMethod]
        public void testLookupCache()
        {
            // do the same lookup twice
            List<Schema> lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", ""));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", ""));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            // now invalidate the cache
            getProvider().invalidateCache();

            // try the lookup again
            lookup = _STAGING.lookupSchema(new CsSchemaLookup("C629", "9231", ""));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());
        }


        [TestMethod]
        public void testOldSchemaNamesExist()
        {
            List<String> oldNames = new List<String>() {"AdnexaUterineOther", "AdrenalGland", "AmpullaVater", "Anus", "Appendix", "BileDuctsDistal", "BileDuctsIntraHepat", "BileDuctsPerihilar", "BiliaryOther",
                    "Bladder", "Bone", "Brain", "Breast", "BuccalMucosa", "CarcinoidAppendix", "Cervix", "CNSOther", "Colon", "Conjunctiva", "CorpusAdenosarcoma", "CorpusCarcinoma", "CorpusSarcoma",
                    "CysticDuct", "DigestiveOther", "EndocrineOther", "EpiglottisAnterior", "Esophagus", "EsophagusGEJunction", "EyeOther", "FallopianTube", "FloorMouth", "Gallbladder",
                    "GenitalFemaleOther", "GenitalMaleOther", "GISTAppendix", "GISTColon", "GISTEsophagus", "GISTPeritoneum", "GISTRectum", "GISTSmallIntestine", "GISTStomach", "GumLower", "GumOther",
                    "GumUpper", "HeartMediastinum", "HemeRetic", "Hypopharynx", "IllDefinedOther", "IntracranialGland", "KaposiSarcoma", "KidneyParenchyma", "KidneyRenalPelvis", "LacrimalGland",
                    "LacrimalSac", "LarynxGlottic", "LarynxOther", "LarynxSubglottic", "LarynxSupraglottic", "LipLower", "LipOther", "LipUpper", "Liver", "Lung", "Lymphoma", "LymphomaOcularAdnexa",
                    "MelanomaBuccalMucosa", "MelanomaChoroid", "MelanomaCiliaryBody", "MelanomaConjunctiva", "MelanomaEpiglottisAnterior", "MelanomaEyeOther", "MelanomaFloorMouth", "MelanomaGumLower",
                    "MelanomaGumOther", "MelanomaGumUpper", "MelanomaHypopharynx", "MelanomaIris", "MelanomaLarynxGlottic", "MelanomaLarynxOther", "MelanomaLarynxSubglottic", "MelanomaLarynxSupraglottic",
                    "MelanomaLipLower", "MelanomaLipOther", "MelanomaLipUpper", "MelanomaMouthOther", "MelanomaNasalCavity", "MelanomaNasopharynx", "MelanomaOropharynx", "MelanomaPalateHard",
                    "MelanomaPalateSoft", "MelanomaPharynxOther", "MelanomaSinusEthmoid", "MelanomaSinusMaxillary", "MelanomaSinusOther", "MelanomaSkin", "MelanomaTongueAnterior", "MelanomaTongueBase",
                    "MerkelCellPenis", "MerkelCellScrotum", "MerkelCellSkin", "MerkelCellVulva", "MiddleEar", "MouthOther", "MycosisFungoides", "MyelomaPlasmaCellDisorder", "NasalCavity", "Nasopharynx",
                    "NETAmpulla", "NETColon", "NETRectum", "NETSmallIntestine", "NETStomach", "Orbit", "Oropharynx", "Ovary", "PalateHard", "PalateSoft", "PancreasBodyTail", "PancreasHead",
                    "PancreasOther", "ParotidGland", "Penis", "Peritoneum", "PeritoneumFemaleGen", "PharyngealTonsil", "PharynxOther", "Placenta", "Pleura", "Prostate", "Rectum", "RespiratoryOther",
                    "Retinoblastoma", "Retroperitoneum", "SalivaryGlandOther", "Scrotum", "SinusEthmoid", "SinusMaxillary", "SinusOther", "Skin", "SkinEyelid", "SmallIntestine", "SoftTissue", "Stomach",
                    "SubmandibularGland", "Testis", "Thyroid", "TongueAnterior", "TongueBase", "Trachea", "Urethra", "UrinaryOther", "Vagina", "Vulva" };

            foreach (String id in _STAGING.getSchemaIds())
            {
                Schema schema = _STAGING.getSchema(id);
                if (!oldNames.Contains(schema.getName()))
                    Assert.Fail("The schema name " + schema.getName() + " is not one of the original names.");
            }
        }

        [TestMethod]
        public void testFindTableRow()
        {
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("size_apa", "size", null));
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("size_apa", "size", "X"));
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("size_apa", "size", "996"));

            Assert.AreEqual(0, _STAGING.findMatchingTableRow("size_apa", "size", "000"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow("size_apa", "size", "055"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow("size_apa", "size", "988"));
            Assert.AreEqual(2, _STAGING.findMatchingTableRow("size_apa", "size", "989"));
            Assert.AreEqual(9, _STAGING.findMatchingTableRow("size_apa", "size", "999"));

            Dictionary<String, String> context = new Dictionary<String, String>();
            context["size"] = "992";
            context["size"] = "992";
            context["size"] = "992";
            Assert.AreEqual(5, _STAGING.findMatchingTableRow("size_apa", context));

            // test a table that has multiple inputs
            context = new Dictionary<String, String>();
            context["t"] = "RE";
            context["n"] = "U";
            context["m"] = "U";
            Assert.AreEqual(167, _STAGING.findMatchingTableRow("summary_stage_rpa", context));
        }

        [TestMethod]
        public void testInputBuilder()
        {
            CsStagingData data1 = new CsStagingData();
            data1.setInput(CsInput.PRIMARY_SITE, "C680");
            data1.setInput(CsInput.HISTOLOGY, "8000");
            data1.setInput(CsInput.BEHAVIOR, "3");
            data1.setInput(CsInput.GRADE, "9");
            data1.setInput(CsInput.DX_YEAR, "2013");
            data1.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            data1.setInput(CsInput.TUMOR_SIZE, "075");
            data1.setInput(CsInput.EXTENSION, "100");
            data1.setInput(CsInput.EXTENSION_EVAL, "9");
            data1.setInput(CsInput.LYMPH_NODES, "100");
            data1.setInput(CsInput.LYMPH_NODES_EVAL, "9");
            data1.setInput(CsInput.REGIONAL_NODES_POSITIVE, "99");
            data1.setInput(CsInput.REGIONAL_NODES_EXAMINED, "99");
            data1.setInput(CsInput.METS_AT_DX, "10");
            data1.setInput(CsInput.METS_EVAL, "9");
            data1.setInput(CsInput.LVI, "9");
            data1.setInput(CsInput.AGE_AT_DX, "060");
            data1.setSsf(1, "020");

            CsStagingData data2 = new CsStagingData.CsStagingInputBuilder().withInput(CsInput.PRIMARY_SITE, "C680").withInput(CsInput.HISTOLOGY, "8000").withInput(
                    CsInput.BEHAVIOR, "3").withInput(CsInput.GRADE, "9").withInput(CsInput.DX_YEAR, "2013").withInput(CsInput.CS_VERSION_ORIGINAL,
                    "020550").withInput(CsInput.TUMOR_SIZE, "075").withInput(CsInput.EXTENSION, "100").withInput(CsInput.EXTENSION_EVAL, "9").withInput(
                    CsInput.LYMPH_NODES, "100").withInput(CsInput.LYMPH_NODES_EVAL, "9").withInput(CsInput.REGIONAL_NODES_POSITIVE, "99").withInput(
                    CsInput.REGIONAL_NODES_EXAMINED, "99").withInput(CsInput.METS_AT_DX, "10").withInput(CsInput.METS_EVAL, "9").withInput(
                    CsInput.LVI, "9").withInput(CsInput.AGE_AT_DX, "060").withSsf(1, "020").build();

            Assert.IsTrue(TNMStaging_UnitTestApp.Src.Staging.ComparisonUtils.CompareStringDictionaries(data1.getInput(), data2.getInput()));
        }

        [TestMethod]
        public void testBlankValues()
        {
            CsStagingData data = new CsStagingData();
            data.setInput(CsInput.PRIMARY_SITE, "C700");
            data.setInput(CsInput.HISTOLOGY, "9530");
            data.setInput(CsInput.BEHAVIOR, "0");
            data.setInput(CsInput.GRADE, "9");
            data.setInput(CsInput.DX_YEAR, "2010");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020520");
            data.setInput(CsInput.TUMOR_SIZE, "999");
            data.setInput(CsInput.EXTENSION, "050");
            data.setInput(CsInput.EXTENSION_EVAL, "9");
            data.setInput(CsInput.LYMPH_NODES, "988");
            data.setInput(CsInput.LYMPH_NODES_EVAL, "9");
            data.setInput(CsInput.REGIONAL_NODES_POSITIVE, "99");
            data.setInput(CsInput.REGIONAL_NODES_EXAMINED, "99");
            data.setInput(CsInput.METS_AT_DX, "00");
            data.setInput(CsInput.METS_EVAL, "9");
            data.setInput(CsInput.LVI, "8");
            data.setInput(CsInput.AGE_AT_DX, "060");
            data.setSsf(1, "999");
            data.setSsf(2, "999");
            data.setSsf(3, "999");
            // do not supply SSF4
            data.setSsf(5, "999");
            data.setSsf(6, "999");
            data.setSsf(7, "000");
            data.setSsf(8, "001");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("brain", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // now change SSF4 to blank; blank values are not validated and since this is not used in staging there should be no errors
            data.setSsf(4, "");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("brain", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // now change extension to blank; the only errors we get should be of type MATCH_NOT_FOUND
            data.setInput(CsInput.EXTENSION, "");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("brain", data.getSchemaId());
            foreach (Error error in data.getErrors())
                Assert.AreEqual(Error.Type.MATCH_NOT_FOUND, error.getType());
        }

        [TestMethod]
        public void testErrors()
        {
            CsStagingData data = new CsStagingData();
            data.setInput(CsInput.PRIMARY_SITE, "C209");
            data.setInput(CsInput.HISTOLOGY, "8490");
            data.setInput(CsInput.BEHAVIOR, "3");
            data.setInput(CsInput.GRADE, "9");
            data.setInput(CsInput.DX_YEAR, "2015");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            data.setInput(CsInput.TUMOR_SIZE, "999");
            data.setInput(CsInput.EXTENSION, "455");
            data.setInput(CsInput.EXTENSION_EVAL, "9");
            data.setInput(CsInput.LYMPH_NODES, "300");
            data.setInput(CsInput.LYMPH_NODES_EVAL, "9");
            data.setInput(CsInput.REGIONAL_NODES_POSITIVE, "99");
            data.setInput(CsInput.REGIONAL_NODES_EXAMINED, "99");
            data.setInput(CsInput.METS_AT_DX, "00");
            data.setInput(CsInput.METS_EVAL, "9");
            data.setInput(CsInput.LVI, "9");
            data.setInput(CsInput.AGE_AT_DX, "050");
            data.setSsf(1, "999");
            data.setSsf(2, "000");
            data.setSsf(3, "988");
            data.setSsf(4, "988");
            data.setSsf(5, "988");
            data.setSsf(6, "988");
            data.setSsf(7, "988");
            data.setSsf(8, "988");
            data.setSsf(9, "999");
            data.setSsf(10, "988");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual(4, data.getErrors().Count);
            Error error = data.getErrors()[0];
            Assert.AreEqual("lymph_nodes_clinical_eval_v0205_ajcc7_xch", error.getTable());
            Assert.AreEqual(new List<string> { "ajcc7_n" }, error.getColumns());
            Assert.AreEqual("Matching resulted in an error in table 'lymph_nodes_clinical_eval_v0205_ajcc7_xch' for column 'ajcc7_n' (000)", error.getMessage());
        }

        [TestMethod]
        public void testStageUrethra()
        {
            // test this case:  http://seer.cancer.gov/seertools/cstest/?mets=10&lnexam=99&diagnosis_year=2013&grade=9&exteval=9&age=060&site=C680&metseval=9&hist=8000&ext=100&version=020550&nodeseval=9&behav=3&lnpos=99&nodes=100&csver_original=020440&lvi=9&ssf1=020&size=075
            CsStagingData data = new CsStagingData();
            data.setInput(CsInput.PRIMARY_SITE, "C680");
            data.setInput(CsInput.HISTOLOGY, "8000");
            data.setInput(CsInput.BEHAVIOR, "3");
            data.setInput(CsInput.GRADE, "9");
            data.setInput(CsInput.DX_YEAR, "2013");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            data.setInput(CsInput.TUMOR_SIZE, "075");
            data.setInput(CsInput.EXTENSION, "100");
            data.setInput(CsInput.EXTENSION_EVAL, "9");
            data.setInput(CsInput.LYMPH_NODES, "100");
            data.setInput(CsInput.LYMPH_NODES_EVAL, "9");
            data.setInput(CsInput.REGIONAL_NODES_POSITIVE, "99");
            data.setInput(CsInput.REGIONAL_NODES_EXAMINED, "99");
            data.setInput(CsInput.METS_AT_DX, "10");
            data.setInput(CsInput.METS_EVAL, "9");
            data.setInput(CsInput.LVI, "9");
            data.setInput(CsInput.AGE_AT_DX, "060");
            data.setSsf(1, "020");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(37, data.getPath().Count);

            // check output
            Assert.AreEqual("129", data.getOutput(CsOutput.SCHEMA_NUMBER));
            Assert.AreEqual("020550", data.getOutput(CsOutput.CSVER_DERIVED));

            // AJCC 6
            Assert.AreEqual("T1", data.getOutput(CsOutput.AJCC6_T));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC6_TDESCRIPTOR));
            Assert.AreEqual("N1", data.getOutput(CsOutput.AJCC6_N));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC6_NDESCRIPTOR));
            Assert.AreEqual("M1", data.getOutput(CsOutput.AJCC6_M));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC6_MDESCRIPTOR));
            Assert.AreEqual("IV", data.getOutput(CsOutput.AJCC6_STAGE));
            Assert.AreEqual("10", data.getOutput(CsOutput.STOR_AJCC6_T));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC6_TDESCRIPTOR));
            Assert.AreEqual("10", data.getOutput(CsOutput.STOR_AJCC6_N));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC6_NDESCRIPTOR));
            Assert.AreEqual("10", data.getOutput(CsOutput.STOR_AJCC6_M));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC6_MDESCRIPTOR));
            Assert.AreEqual("70", data.getOutput(CsOutput.STOR_AJCC6_STAGE));

            // AJCC 7
            Assert.AreEqual("T1", data.getOutput(CsOutput.AJCC7_T));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC7_TDESCRIPTOR));
            Assert.AreEqual("N1", data.getOutput(CsOutput.AJCC7_N));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC7_NDESCRIPTOR));
            Assert.AreEqual("M1", data.getOutput(CsOutput.AJCC7_M));
            Assert.AreEqual("c", data.getOutput(CsOutput.AJCC7_MDESCRIPTOR));
            Assert.AreEqual("IV", data.getOutput(CsOutput.AJCC7_STAGE));
            Assert.AreEqual("100", data.getOutput(CsOutput.STOR_AJCC7_T));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC6_TDESCRIPTOR));
            Assert.AreEqual("100", data.getOutput(CsOutput.STOR_AJCC7_N));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC7_NDESCRIPTOR));
            Assert.AreEqual("100", data.getOutput(CsOutput.STOR_AJCC7_M));
            Assert.AreEqual("c", data.getOutput(CsOutput.STOR_AJCC7_MDESCRIPTOR));
            Assert.AreEqual("700", data.getOutput(CsOutput.STOR_AJCC7_STAGE));

            // Summary Stage
            Assert.AreEqual("L", data.getOutput(CsOutput.SS1977_T));
            Assert.AreEqual("RN", data.getOutput(CsOutput.SS1977_N));
            Assert.AreEqual("D", data.getOutput(CsOutput.SS1977_M));
            Assert.AreEqual("D", data.getOutput(CsOutput.SS1977_STAGE));
            Assert.AreEqual("L", data.getOutput(CsOutput.SS2000_T));
            Assert.AreEqual("RN", data.getOutput(CsOutput.SS2000_N));
            Assert.AreEqual("D", data.getOutput(CsOutput.SS2000_M));
            Assert.AreEqual("D", data.getOutput(CsOutput.SS2000_STAGE));
            Assert.AreEqual("7", data.getOutput(CsOutput.STOR_SS1977_STAGE));
            Assert.AreEqual("7", data.getOutput(CsOutput.STOR_SS2000_STAGE));

            // make sure defaulted inputs are not in the output
            HashSet<String> outputKeys = new HashSet<String>();
            foreach (KeyValuePair<String, String> entry in data.getOutput())
            {
                outputKeys.Add(entry.Key);
            }
            foreach (CsOutput output in CsOutput.Values)
            {
                outputKeys.Remove(output.toString());
            }
            Assert.IsTrue(outputKeys.Count == 0, "The keys " + outputKeys + " were in the output but are not CS output fields.");

            // test case with valid year_dx and invalid version original
            data.setInput(CsInput.DX_YEAR, "2013");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "1111");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(1, data.getErrors().Count);
            Assert.AreEqual(Error.Type.INVALID_REQUIRED_INPUT, data.getErrors()[0].getType());

            // test case with missing year_dx and valid version original
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // test case with missing year_dx and valid version original
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020001");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // test case with space-filled year_dx and valid version original
            data.setInput(CsInput.DX_YEAR, "    ");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020001");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // test case with missing year_dx and invalid version original
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "012345");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // test case with missing year_dx and invalid version original
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "1");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            // test case with space-filled year_dx and invalid version original
            data.setInput(CsInput.DX_YEAR, "    ");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "012345");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);

            data.setInput(CsInput.DX_YEAR, "2003");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getOutput().Count);
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(0, data.getPath().Count);

            data.setInput(CsInput.DX_YEAR, "2050");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020550");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getOutput().Count);
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(0, data.getPath().Count);
        }

        [TestMethod]
        public void testColonUnknownDxYear()
        {
            CsStagingData data = new CsStagingData();
            data.setInput(CsInput.PRIMARY_SITE, "C183");
            data.setInput(CsInput.HISTOLOGY, "8140");
            data.setInput(CsInput.BEHAVIOR, "0");
            data.setInput(CsInput.GRADE, "1");
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "010401");
            data.setInput(CsInput.TUMOR_SIZE, "000");
            data.setInput(CsInput.EXTENSION, "000");
            data.setInput(CsInput.EXTENSION_EVAL, "0");
            data.setInput(CsInput.LYMPH_NODES, "000");
            data.setInput(CsInput.LYMPH_NODES_EVAL, "0");
            data.setInput(CsInput.REGIONAL_NODES_POSITIVE, "00");
            data.setInput(CsInput.REGIONAL_NODES_EXAMINED, "00");
            data.setInput(CsInput.METS_AT_DX, "00");
            data.setInput(CsInput.METS_EVAL, "0");
            data.setInput(CsInput.LVI, "0");
            data.setInput(CsInput.AGE_AT_DX, "0");
            data.setSsf(1, "000");
            data.setSsf(2, "000");
            data.setSsf(3, "000");
            data.setSsf(4, "000");
            data.setSsf(5, "000");
            data.setSsf(6, "000");
            data.setSsf(7, "020");
            data.setSsf(8, "000");
            data.setSsf(9, "010");
            data.setSsf(10, "010");
            for (int i = 11; i <= 25; i++)
            {
                data.setSsf(i, "988");
            }

            // perform the staging
            _STAGING.stage(data);

            // verify the AJCC7 values should be null
            foreach (KeyValuePair<string,string> entry in data.getOutput())
            {
                if (entry.Key.Contains("ajcc7"))
                    Assert.IsNull(entry.Value, "AJCC7 Key '" + entry.Key + " should be null");
                else
                    Assert.IsNotNull(entry.Value, "Key '" + entry.Key + " should not be null");
            }
        }

        [TestMethod]
        public void testBadLookupInStage()
        {
            CsStagingData data = new CsStagingData();

            // if site/hist are not supplied, no lookup
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // add hist only and it should fail with same result
            data.setInput(CsInput.PRIMARY_SITE, "C489");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // put a site/hist combo that doesn't match a schema
            data.setInput(CsInput.HISTOLOGY, "9898");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_NO_MATCHING_SCHEMA, data.getResult());

            // now a site/hist that returns multiple results
            data.setInput(CsInput.PRIMARY_SITE, "C111");
            data.setInput(CsInput.HISTOLOGY, "8200");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS, data.getResult());
        }

        [TestMethod]
        public void testInvolvedTables()
        {
            HashSet<String> tables = _STAGING.getInvolvedTables("brain");
            HashSet<String> hash1 = new HashSet<String>() {
                "cs_year_validation", "schema_selection_brain", "ajcc6_m_codes", "ajcc7_m_codes", "ssf22_snq", "nodes_exam_gna", "ssf13_snh",
                "ssf7_sqk", "lvi", "ajcc6_n_codes", "ssf18_snm", "ssf15_snj", "ssf20_sno", "ssf10_sne", "ssf17_snl", "ssf6_opf", "summary_stage_rpa",
                "histology", "ss_codes", "mets_haw", "nodes_pos_fna", "ajcc7_stage_una", "ajcc7_year_validation", "ssf4_mpn", "mets_eval_ina", "ssf3_lpm",
                "primary_site", "nodes_dna", "ajcc6_stage_qna", "ssf8_sql", "ssf19_snn", "ssf2_kpl", "ajcc7_t_codes", "behavior", "nodes_eval_ena",
                "ajcc_tdescriptor_cleanup", "ssf21_snp", "ajcc_descriptor_codes", "ssf16_snk", "ajcc6_t_codes", "ssf5_nph", "ajcc7_n_codes",
                "ajcc6_stage_codes", "extension_bcc", "grade", "size_apa", "ajcc_ndescriptor_cleanup", "ssf12_sng", "ssf23_snr", "ajcc7_inclusions_tqf",
                "ajcc7_stage_codes", "extension_eval_cna", "ajcc_mdescriptor_cleanup", "cs_input_version_original", "ajcc6_year_validation", "ssf1_jpo",
                "ssf25_snt", "ssf11_snf", "ssf9_snd", "ssf14_sni", "ssf24_sns"};
            Assert.IsTrue(tables.SetEquals(hash1));
        }

        [TestMethod]
        public void testInvolvedSchemas()
        {
            HashSet<String> schemas = _STAGING.getInvolvedSchemas("ssf1_jpd");
            HashSet<String> hash1 = new HashSet<String>() { "kidney_renal_pelvis", "bladder", "urethra" };

            Assert.IsTrue(hash1.SetEquals(schemas));
        }

        [TestMethod]
        public void testGetInputs()
        {
            HashSet<String> test1 = _STAGING.getInputs(_STAGING.getSchema("adnexa_uterine_other"));
            HashSet<String> test2 = new HashSet<String>() { "extension", "site", "extension_eval", "mets_eval", "nodes_eval", "nodes", "hist", "year_dx", "cs_input_version_original", "mets" };
            Assert.IsTrue(test1.SetEquals(test2));


            test1 = new HashSet<String>() {"site", "nodes_pos", "mets_eval", "nodes_eval", "ssf16", "ssf15", "ssf13", "cs_input_version_original", "lvi", "extension",
                            "extension_eval", "ssf1", "ssf2", "ssf3", "hist", "ssf4", "nodes", "ssf5", "year_dx", "mets" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("testis"));
            Assert.IsTrue(test1.SetEquals(test2));

            // test with context
            Dictionary<String, String> context = new Dictionary<String, String>();
            context[StagingData.PRIMARY_SITE_KEY] = "C619";
            context[StagingData.HISTOLOGY_KEY] = "8120";
            context[StagingData.YEAR_DX_KEY] = "2004";

            // for that context, neither AJCC6 or 7 should be calculated so "grade" and "ssf1" should not be list of inputs
            test1 = new HashSet<String>() {"site", "nodes_eval", "mets_eval", "ssf10", "cs_input_version_original", "ssf8", "extension", "extension_eval",
                    "ssf3", "hist", "nodes", "year_dx", "mets" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("prostate"), context);
            Assert.IsTrue(test1.SetEquals(test2));

            // test without context
            test1 = new HashSet<String>() {"site", "nodes_eval", "mets_eval", "ssf10", "cs_input_version_original", "ssf8", "extension", "extension_eval", "ssf1",
                            "ssf3", "hist", "nodes", "year_dx", "grade", "mets" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("prostate"));
            Assert.IsTrue(test1.SetEquals(test2));
        }

        [TestMethod]
        public void testIsCodeValid()
        {
            // test bad parameters for schema or field
            Assert.IsFalse(_STAGING.isCodeValid("bad_schema_name", "site", "C509"));
            Assert.IsFalse(_STAGING.isCodeValid("testis", "bad_field_name", "C509"));

            // test null values
            Assert.IsFalse(_STAGING.isCodeValid(null, null, null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", null, null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "site", null));

            // test fields that have a "value" specified
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "200"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2003"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2145"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "year_dx", "2004"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "year_dx", "2015"));

            // test valid and invalid fields
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "extension", "050"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "extension", "025"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "ssf1", "020"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "ssf1", "030"));
        }

        [TestMethod]
        public void testIsContextValid()
        {
            CsStagingData data = new CsStagingData();

            data.setInput(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT, "2015");

            // test valid year
            data.setInput(CsInput.DX_YEAR, "2004");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020001");
            Assert.IsTrue(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test invalid year
            data.setInput(CsInput.DX_YEAR, "2003");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test blank year with valid version
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020001");
            Assert.IsTrue(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test space-filled year with valid version
            data.setInput(CsInput.DX_YEAR, "    ");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "020001");
            Assert.IsTrue(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test blank year with invalid version
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "000000");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test blank year with invalid version of wrong length
            data.setInput(CsInput.DX_YEAR, "");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "1");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test space-filled year with invalid version
            data.setInput(CsInput.DX_YEAR, "    ");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "000000");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test space-filled year with invalid version of wrong length
            data.setInput(CsInput.DX_YEAR, "    ");
            data.setInput(CsInput.CS_VERSION_ORIGINAL, "1");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));
        }

        [TestMethod]
        public void testGetSchemaIds()
        {
            HashSet<String> algorithms = _STAGING.getSchemaIds();

            Assert.IsTrue(algorithms.Count > 0);
            Assert.IsTrue(algorithms.Contains("testis"));
        }

        [TestMethod]
        public void testGetTableIds()
        {
            HashSet<String> tables = _STAGING.getTableIds();

            Assert.IsTrue(tables.Count > 0);
            Assert.IsTrue(tables.Contains("ajcc7_stage_uaz"));
        }

        [TestMethod]
        public void testGetSchema()
        {
            Assert.IsNull(_STAGING.getSchema("bad_schema_name"));
            Assert.IsNotNull(_STAGING.getSchema("brain"));
            Assert.AreEqual(143, _STAGING.getSchema("brain").getSchemaNum());
        }

        [TestMethod]
        public void testStagingInputsAndOutputs()
        {
            Schema schema = _STAGING.getSchema("testis");

            HashSet<String> hash1 = new HashSet<String>() {
                "cs_input_version_original", "extension", "extension_eval", "site", "hist", "lvi", "mets_eval", "mets", "nodes",
                            "nodes_eval", "nodes_pos", "ssf1", "ssf2", "ssf3", "ssf4", "ssf5", "ssf13", "ssf15", "ssf16", "year_dx" };
            HashSet<String> hash2 = _STAGING.getInputs(schema);
            Assert.IsTrue(hash1.SetEquals(hash2), "Inputs do not match");

            // note that outputs should NOT include values produced by staging that are not in the defined output list (if an output list exists on the schema)
            hash1 = new HashSet<String>()
                { "schema_number", "csver_derived", "ss77", "stor_ajcc7_m", "t2000", "stor_ajcc7_n", "stor_ajcc6_tdescriptor", "ajcc7_stage",
                    "stor_ajcc6_mdescriptor", "stor_ss2000", "ajcc6_tdescriptor", "stor_ajcc7_t", "ajcc6_stage", "n2000", "ajcc7_ndescriptor", "ajcc6_ndescriptor",
                    "ajcc7_mdescriptor", "ajcc6_mdescriptor", "stor_ajcc7_stage", "m77", "ajcc6_m", "ss2000", "stor_ajcc7_ndescriptor", "ajcc7_m", "ajcc7_n",
                    "stor_ajcc7_mdescriptor", "t77", "ajcc6_n", "stor_ss77", "ajcc6_t", "stor_ajcc6_ndescriptor", "stor_ajcc6_stage", "m2000", "ajcc7_t", "n77",
                    "ajcc7_tdescriptor", "stor_ajcc6_m", "stor_ajcc6_n", "stor_ajcc6_t", "stor_ajcc7_tdescriptor" };
            Assert.IsTrue(hash1.SetEquals(_STAGING.getOutputs(schema)), "Outputs do not match");

            // test used for staging
            Assert.IsFalse(schema.getInputMap()["ssf14"].getUsedForStaging());
            Assert.IsTrue(schema.getInputMap()["ssf15"].getUsedForStaging());

            // test metadata
            Assert.IsNull(schema.getInputMap()["ssf11"].getMetadata());
            Assert.IsTrue(schema.getInputMap()["ssf17"].getMetadata().Contains(new StagingMetadata("UNDEFINED_SSF")));
            Assert.IsTrue(schema.getInputMap()["ssf7"].getMetadata().Contains(new StagingMetadata("SEER_CLINICALLY_SIGNIFICANT")));

            Dictionary<String, String> context = new Dictionary<String, String>();
            context[StagingData.PRIMARY_SITE_KEY] = "C629";
            context[StagingData.HISTOLOGY_KEY] = "9231";
            HashSet<String> inputs = _STAGING.getInputs(schema, context);

            // this is a case that summary stages only.  Testing to make sure "hist", which is used in the inclusion/exclusion criteria
            // is included in the list even though the mappings for AJCC6 and 7 are not included
            Assert.IsTrue(inputs.Contains("hist"), "Inclusion/exclusion input is not included");

            Assert.IsTrue(inputs.Contains("mets"));

            // these are no used when only doing summary stage
            Assert.IsFalse(inputs.Contains("ssf1"));
            Assert.IsFalse(inputs.Contains("ssf2"));
            Assert.IsFalse(inputs.Contains("ssf3"));
            Assert.IsFalse(inputs.Contains("ssf13"));
            Assert.IsFalse(inputs.Contains("ssf15"));
            Assert.IsFalse(inputs.Contains("ssf16"));

            // now test one that does do AJCC7 (inputs should include extra SSFs used in AJCC calculations)
            context[StagingData.HISTOLOGY_KEY] = "9100";
            inputs = _STAGING.getInputs(schema, context);
            Assert.IsTrue(inputs.Contains("hist"));
            Assert.IsTrue(inputs.Contains("ssf1"));
            Assert.IsTrue(inputs.Contains("ssf2"));
            Assert.IsTrue(inputs.Contains("ssf3"));
            Assert.IsTrue(inputs.Contains("ssf13"));
            Assert.IsTrue(inputs.Contains("ssf15"));
            Assert.IsTrue(inputs.Contains("ssf16"));

            // the prostate schema tables use a reference to {{ssf8}} and {{ssf10}}; make sure they are picked up in the list of required inputs
            inputs = _STAGING.getInputs(_STAGING.getSchema("prostate"));
            Assert.IsTrue(inputs.Contains("ssf8"));
            Assert.IsTrue(inputs.Contains("ssf10"));
        }

        [TestMethod]
        public void testAllValidInputs() //throws IOException
        {
            if (DebugSettings.RUN_LARGE_CS_TESTS)
            {
                String basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";

                String sFilePath = basedir + "Resources\\Test\\CS\\valid_inputs.020550.txt.gz";

                FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);
                TextReader input = new StreamReader(decompressionStream);

                // cache a list of schemas by name
                Dictionary<String, String> nameMap = new Dictionary<String, String>();
                foreach (String id in _STAGING.getSchemaIds())
                    nameMap[_STAGING.getSchema(id).getName()] = id;

                String line = input.ReadLine();
                int iNumLines = 0;
                while (line != null)
                {
                    iNumLines++;
                    // split the CSV record
                    String[] values = line.Split("\\|".ToCharArray());
                    Assert.AreEqual(3, values.Length);

                    // get schema by schema name
                    Schema schema = _STAGING.getSchema(nameMap[values[0]]);
                    Assert.IsTrue(_STAGING.isCodeValid(schema.getId(), values[1], values[2]),
                        "The value '" + values[2] + "' is not valid for table '" + values[1] + "' and schema '" + values[0] + "'");

                    line = input.ReadLine();
                }

                System.Diagnostics.Trace.WriteLine("Processed " + iNumLines + " lines.");

                input.Close();
            }

        }

        // * This tests that INPUT fields in tables that have a validation table associated with them are the correct length.In other words,
        // * if a table has an INPUT column for "ssf4" but has a value for that column of "00" this would catch that that field should be
        // * 3 characters long based on the ssf4_lookup_table.
        [TestMethod]
        public void testInvalidTableInputs()
        {
            HashSet<String> errors = new HashSet<String>();

            foreach (String schemaId in _STAGING.getSchemaIds())
            {
                Schema schema = _STAGING.getSchema(schemaId);

                // build a list of input tables that should be excluded
                Dictionary<String, int> inputTableLengths = new Dictionary<String, int>();
                foreach (IInput input in schema.getInputs())
                    if (input.getTable() != null)
                        inputTableLengths[input.getTable()] = getInputLength(input.getTable(), input.getKey());



                // loop over involved tables
                foreach (String tableId in schema.getInvolvedTables())
                {
                    if (inputTableLengths.ContainsKey(tableId))
                        continue;

                    ITable table = _STAGING.getTable(tableId);

                    // loop over each row
                    foreach (ITableRow row in table.getTableRows())
                    {
                        // loop over all input cells
                        foreach (String key in row.getColumns())
                        {
                            // only validate keys that are actually INPUT values
                            if (!schema.getInputMap().ContainsKey(key))
                                continue;

                            // only validate inputs that have an associated table
                            String validationTableId = schema.getInputMap()[key].getTable();
                            if (validationTableId == null)
                                continue;

                            int expectedFieldLength = inputTableLengths[validationTableId];

                            // loop over list of ranges
                            foreach (Range range in row.getColumnInput(key))
                            {
                                String low = range.getLow();
                                String high = range.getHigh();

                                // if it matches all, continue
                                if (range.matchesAll() || low.Length == 0)
                                    continue;

                                if (low.StartsWith("{{") && low.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                                    low = DateTime.Now.Year.ToString();
                                if (high.StartsWith("{{") && high.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                                    high = DateTime.Now.Year.ToString();

                                // change that ranges are the same length
                                if (low.Length != high.Length)
                                    errors.Add(schemaId + " -> " + tableId + ": " + key + " = '" + low + "-" + high + "' : lengths differ");

                                // make sure the fields that have input validation match the length in that input validation table
                                if (expectedFieldLength >= 0 && (!expectedFieldLength.Equals(low.Length) || !expectedFieldLength.Equals(high.Length)))
                                {
                                    if (low.Equals(high))
                                        errors.Add(schemaId + " -> " + tableId + ": " + key + " = '" + low + "' : length does not match lookup table " + validationTableId);
                                    else
                                        errors.Add(schemaId + " -> " + tableId + ": " + key + " = '" + low + "-" + high + "' : lengths do not match lookup table " + validationTableId);
                                }
                            }
                        }
                    }
                }
            }

            assertNoErrors(errors, "inputs values in tables which are not valid");
        }

        [TestMethod]
        public void testSchemaSelectionIntegration() 
        {
            if (DebugSettings.RUN_LARGE_CS_TESTS)
            {
                String basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";

                // test complete file of cases
                String sFilePath = basedir + "Resources\\Test\\CS\\cs_schema_identification_unit_test.txt.gz";

                FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);

                TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult result =
                        TNMStaging_UnitTestApp.Src.Staging.CS.CSIntegrationSchemaSelection.processSchemaSelection(_STAGING, sFilePath, decompressionStream, TestContext);

                Assert.AreEqual(0, result.getNumFailures());
            }
        }

        [TestMethod]
        public void testExpectedOutput() 
        {
            if (DebugSettings.RUN_LARGE_CS_TESTS)
            {
                String basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";

                String sFilePath = basedir + "Resources\\Test\\CS\\AJCC_6.V020550.txt.gz";

                FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);

                TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult ajcc6Result =
                        TNMStaging_UnitTestApp.Src.Staging.CS.CSIntegrationSchemaStage.processSchema(_STAGING, sFilePath, decompressionStream);

                fstream.Close();
                decompressionStream.Close();


                sFilePath = basedir + "Resources\\Test\\CS\\AJCC_7.V020550.txt.gz";

                fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);

                TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult ajcc7Result =
                        TNMStaging_UnitTestApp.Src.Staging.CS.CSIntegrationSchemaStage.processSchema(_STAGING, sFilePath, decompressionStream);

                fstream.Close();
                decompressionStream.Close();

                // make sure there were no errors returned
                Assert.AreEqual(0, ajcc6Result.getNumFailures(), "There were failures in the AJCC6 tests.");
                Assert.AreEqual(0, ajcc7Result.getNumFailures(), "There were failures in the AJCC7 tests.");
            }
        }
    }
}


