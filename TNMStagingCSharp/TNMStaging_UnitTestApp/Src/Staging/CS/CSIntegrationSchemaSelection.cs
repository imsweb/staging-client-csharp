using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.CS;


namespace TNMStaging_UnitTestApp.Src.Staging.CS
{
    public class CSIntegrationSchemaSelection
    {

        public static IntegrationUtils.IntegrationResult processSchemaSelection(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, TestContext testLog) //throws IOException, InterruptedException 
        {
            return processSchemaSelection_MultiTask(staging, fileName, inputStream, testLog);
        }


        public class MultiTask_DataObj
        {
            public String[] mParts;
            public String msFullLine;
            public int miLineNum;
        }

        public static TNMStagingCSharp.Src.Staging.Staging mMultiTask_Staging;
        public static int miMultiTask_FailedCases = 0;
        public static int miMultiTask_ThreadProcessedCases = 0;


        // * Process all schemas in file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @param singleLineNumber if not null, only process this line number
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processSchemaSelection_MultiTask(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, TestContext testLog) 
        {
            IntegrationUtils.TestReportLog = testLog;


            // go over each file
            int processedCases = 0;
            int iLineNumber = 0;
            //int iLineCounter = 0;

            MultiTasksExecutor thisMultiTasksExecutor = new MultiTasksExecutor();
            thisMultiTasksExecutor.AddAction(new MultiTasksExecutor.ActionCallBack(MultiTask_TaskCompute));
            thisMultiTasksExecutor.StartTasks();

            mMultiTask_Staging = staging;
            miMultiTask_FailedCases = 0;
            miMultiTask_ThreadProcessedCases = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // loop over each line in the file
            TextReader reader = new StreamReader(inputStream);
            String line = reader.ReadLine();

            while (line != null)
            {
                iLineNumber++;

                if (iLineNumber >= 0)
                {
                    processedCases++;

                    // split the string; important to keep empty trailing values in the resulting array
                    String[] parts = line.Split(",".ToCharArray(), 1000);

                    if (parts.Length != 4)
                    {
                        throw new System.InvalidOperationException("Bad record in schema_selection.txt on line number: " + iLineNumber);
                    }
                    else
                    {
                        MultiTask_DataObj obj = new MultiTask_DataObj();
                        obj.mParts = parts;
                        obj.msFullLine = line;
                        obj.miLineNum = iLineNumber;

                        thisMultiTasksExecutor.AddDataItem(obj);

                        /*
                        iLineCounter++;
                        if (iLineCounter >= 500000)
                        {
                            IntegrationUtils.WritelineToLog("Processed Cases: " + processedCases);
                            iLineCounter = 0;
                        }
                        */

                    }

                }

                line = reader.ReadLine();
            }

            thisMultiTasksExecutor.WaitForCompletion();


            stopwatch.Stop();
            String perMs = String.Format("{0,12:F4}", ((float)stopwatch.Elapsed.TotalMilliseconds / processedCases)).Trim();
            IntegrationUtils.WritelineToLog("Completed " + processedCases + " cases for " + fileName + " in " + TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.GenerateTotalTimeString(stopwatch) + " (" + perMs + " ms/case).");

            IntegrationUtils.WritelineToLog("Threads Completed " + miMultiTask_ThreadProcessedCases + " cases.");


            if (miMultiTask_FailedCases > 0)
                IntegrationUtils.WritelineToLog("There were " + miMultiTask_FailedCases + " failures.");
            else
                IntegrationUtils.WritelineToLog("");


            IntegrationUtils.WritelineToLog("-----------------------------------------------");

            inputStream.Close();

            return new IntegrationUtils.IntegrationResult(processedCases, miMultiTask_FailedCases);
        }


        public static void MultiTask_TaskCompute(int id, Object task_data)
        {
            MultiTask_DataObj thisDataObj = (MultiTask_DataObj)task_data;
            try
            {
                SchemaLookup lookup = new SchemaLookup(thisDataObj.mParts[0], thisDataObj.mParts[1]);
                lookup.setInput(CsStagingData.SSF25_KEY, thisDataObj.mParts[2]);


                List<StagingSchema> lookups = mMultiTask_Staging.lookupSchema(lookup);
                if (lookups == null)
                {
                    IntegrationUtils.WritelineToLog("Line #" + thisDataObj.miLineNum + " [" + thisDataObj.msFullLine + "] --> mStaging.lookupSchema returned NULL.");
                    Interlocked.Increment(ref miMultiTask_FailedCases);
                }
                else if (thisDataObj.mParts[3].Length == 0)
                {
                    if (lookups.Count == 1)
                    {
                        IntegrationUtils.WritelineToLog("Line #" + thisDataObj.miLineNum + " [" + thisDataObj.msFullLine + "] --> The schema selection should not have found any schema but did: " + lookups[0].getId());
                        Interlocked.Increment(ref miMultiTask_FailedCases);
                    }
                }
                else
                {
                    if (lookups.Count != 1)
                    {
                        IntegrationUtils.WritelineToLog("Line #" + thisDataObj.miLineNum + " [" + thisDataObj.msFullLine + "] --> The schema selection should have found a schema, " + thisDataObj.mParts[3] + ", but did not.");
                        Interlocked.Increment(ref miMultiTask_FailedCases);
                    }
                    else if (lookups[0].getId() != thisDataObj.mParts[3])
                    {
                        IntegrationUtils.WritelineToLog("Line #" + thisDataObj.miLineNum + " [" + thisDataObj.msFullLine + "] --> The schema selection found schema " + lookups[0].getId() + " but it should have been " + thisDataObj.mParts[3] + ".");
                        Interlocked.Increment(ref miMultiTask_FailedCases);
                    }
                }

            }
            catch (Exception e)
            {
                if (miMultiTask_FailedCases == 0)
                {
                    IntegrationUtils.WritelineToLog("Line #" + thisDataObj.miLineNum + " --> Exception processing schema selection: " + e.Message);
                    IntegrationUtils.WritelineToLog("  StackTrace: " + e.StackTrace);
                }
                Interlocked.Increment(ref miMultiTask_FailedCases);
            }

            Interlocked.Increment(ref miMultiTask_ThreadProcessedCases);
        }


    }
}
