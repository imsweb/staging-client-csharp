using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.TNM;


namespace TNMStaging_UnitTestApp.Src.Staging.TNM
{
    public class TnmBigIntegrationTest
    {


        public void testTNMBigIntegrationTestGood() 
        {
            if (DebugSettings.RUN_HUGE_GOOD_TNM_TESTS)
            {

                List<String> _SCHEMA_FILES = new List<String>();

                IntegrationUtils.WritelineToLog("Starting TNMBigIntegrationTest::testBigIntegrationTestGood...");

                TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(TnmDataProvider.getInstance(TnmVersion.v1_5));

                // hard-code data directory based on Windows vs Linux
                String dataDirectory;
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    dataDirectory = "\\\\omni\\btp\\csb\\Staging\\TNM\\";
                }
                else
                {
                    dataDirectory = "/prj/csb/Staging/TNM";
                }

                long totalFailures = 0;

                String sFilePath = dataDirectory + "schema_selection\\tnm_schema_identification.txt.gz";

                totalFailures = PerformTNMSchemaSelection(staging, sFilePath);

                totalFailures = PerformTNMStaging(staging, dataDirectory, _SCHEMA_FILES);
            }
        }

        public void testTNMBigIntegrationTestBad() 
        {
            if (DebugSettings.RUN_HUGE_BAD_TNM_TESTS)
            {
                List<String> _SCHEMA_FILES = new List<String>();

                IntegrationUtils.WritelineToLog("Starting TNMBigIntegrationTest::testBigIntegrationTestBad...");

                TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(TnmDataProvider.getInstance(TnmVersion.v1_5));

                // hard-code data directory based on Windows vs Linux
                String dataDirectory;
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    dataDirectory = "\\\\omni\\btp\\csb\\Staging\\TNM_ErrorCases\\";
                }
                else
                {
                    dataDirectory = "/prj/csb/Staging/TNM";
                }

                long totalFailures = 0;


                String sFilePath = dataDirectory + "schema_selection\\tnm_schema_identification.txt.gz";

                totalFailures = PerformTNMSchemaSelection(staging, sFilePath);

                totalFailures = PerformTNMStaging(staging, dataDirectory, _SCHEMA_FILES);

            }
        }


        public long PerformTNMSchemaSelection(TNMStagingCSharp.Src.Staging.Staging staging, String sFilePath) 
        {
            FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);
            IntegrationUtils.IntegrationResult result =
                    TnmIntegrationSchemaSelection.processSchemaSelection(
                        staging, "tnm_schema_identification.txt.gz", decompressionStream, null);


            IntegrationUtils.WritelineToLog("-----------------------------------------------");

            return result.getNumFailures();
        }

        public long PerformTNMStaging(TNMStagingCSharp.Src.Staging.Staging staging, String dataDirectory, List<String> _SCHEMA_FILES) 
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long totalFiles = 0;
            long totalCases = 0;
            long totalFailures = 0;

            // get the complete list of files

            string[] arrfiles = Directory.GetFiles(dataDirectory, "*.*");

            if (arrfiles.Length > 0)
            {
                // sort the files by name
                Array.Sort(arrfiles);

                foreach (String f in arrfiles)
                {
                    String sFilename = Path.GetFileName(f);
                    if (File.Exists(f) && sFilename.EndsWith(".gz"))
                    {
                        if (_SCHEMA_FILES.Count == 0 || _SCHEMA_FILES.Contains(sFilename))
                        {
                            totalFiles += 1;
                            IntegrationUtils.WritelineToLog("Staging File: " + f);

                            FileStream fstream = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read);
                            GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);
                            IntegrationUtils.IntegrationResult result =
                                TnmIntegrationSchemaStage.processTNMSchema(staging, sFilename, decompressionStream);
                            totalCases += result.getNumCases();
                            totalFailures += result.getNumFailures();

                        }
                    }
                }
            }

            stopwatch.Stop();

            String perMs = String.Format("{0,12:F4}", ((float)stopwatch.Elapsed.TotalMilliseconds / totalCases)).Trim();
            IntegrationUtils.WritelineToLog("");
            IntegrationUtils.WritelineToLog("Completed " + totalCases + " cases (" + totalFiles + " files) in " + IntegrationUtils.GenerateTotalTimeString(stopwatch) + " (" + perMs + " ms/case).");
            if (totalFailures > 0)
                IntegrationUtils.WritelineToLog("There were " + totalFailures + " failing cases.");

            return totalFailures;
        }


    }
}




