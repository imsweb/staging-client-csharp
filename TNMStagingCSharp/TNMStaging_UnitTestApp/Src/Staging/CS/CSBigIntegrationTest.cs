using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging.CS;


namespace TNMStaging_UnitTestApp.Src.Staging.CS
{
    public class CSBigIntegrationTest
    {

        public void testCSBigIntegrationTestGood()
        {
            if (DebugSettings.RUN_HUGE_GOOD_CS_TESTS)
            {
                List<String> _SCHEMA_FILES = new List<String>();

                IntegrationUtils.WritelineToLog("Starting CSBigIntegrationTest::testBigIntegrationTestGood...");

                TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(CsDataProvider.getInstance(CsVersion.v020550));

                // hard-code data directory based on Windows vs Linux
                String dataDirectory;
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    dataDirectory = "\\\\omni\\btp\\csb\\Staging\\CS\\";
                }
                else
                {
                    dataDirectory = "/prj/csb/Staging/CS";
                }

                long totalFailures = 0;

                String sFilePath = dataDirectory + "schema_selection\\cs_schema_identification.txt.gz";

                totalFailures = PerformCSSchemaSelection(staging, sFilePath);

                totalFailures = PerformCSStaging(staging, dataDirectory, _SCHEMA_FILES);

            }
        }

        public void testCSBigIntegrationTestBad() 
        {
            if (DebugSettings.RUN_HUGE_BAD_CS_TESTS)
            {
                List<String> _SCHEMA_FILES = new List<String>();

                IntegrationUtils.WritelineToLog("Starting CSBigIntegrationTest::testBigIntegrationTestBad...");

                TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(CsDataProvider.getInstance(CsVersion.v020550));

                // hard-code data directory based on Windows vs Linux
                String dataDirectory;
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    dataDirectory = "\\\\omni\\btp\\csb\\Staging\\CS_ErrorCases\\";
                }
                else
                {
                    dataDirectory = "/prj/csb/Staging/CS";
                }

                long totalFailures = 0;


                String sFilePath = dataDirectory + "schema_selection\\cs_schema_identification.txt.gz";

                totalFailures = PerformCSSchemaSelection(staging, sFilePath);

                totalFailures = PerformCSStaging(staging, dataDirectory, _SCHEMA_FILES);

            }
        }


        public long PerformCSSchemaSelection(TNMStagingCSharp.Src.Staging.Staging staging, String sFilePath) 
        {
            FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);
            TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult result =
                    TNMStaging_UnitTestApp.Src.Staging.CS.CSIntegrationSchemaSelection.processSchemaSelection(
                        staging, "cs_schema_identification.txt.gz", decompressionStream, null);


            IntegrationUtils.WritelineToLog("-----------------------------------------------");

            return result.getNumFailures();
        }

        public long PerformCSStaging(TNMStagingCSharp.Src.Staging.Staging staging, String dataDirectory, List<String> _SCHEMA_FILES) 
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
                            TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult result =
                                TNMStaging_UnitTestApp.Src.Staging.CS.CSIntegrationSchemaStage.
                                    processSchema(staging, sFilename, decompressionStream);
                            totalCases += result.getNumCases();
                            totalFailures += result.getNumFailures();

                        }
                    }
                }
            }

            stopwatch.Stop();

            String perMs = String.Format("{0,12:F4}", ((float)stopwatch.Elapsed.TotalMilliseconds / totalCases)).Trim();
            IntegrationUtils.WritelineToLog("");
            IntegrationUtils.WritelineToLog("Completed " + totalCases + " cases (" + totalFiles + " files) in " + TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.GenerateTotalTimeString(stopwatch) + " (" + perMs + " ms/case).");
            if (totalFailures > 0)
                IntegrationUtils.WritelineToLog("There were " + totalFailures + " failing cases.");

            return totalFailures;
        }


    }
}
