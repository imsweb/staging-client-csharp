using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.DecisionEngine;

namespace TNMStaging_UnitTestApp.Src.Staging.CS
{
    public class CSIntegrationSchemaStage
    {

        // * Process all schemas in file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processSchema(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream) //throws IOException, InterruptedException 
        {
            return processSchema_MultiTask(staging, fileName, inputStream, -1);
        }



        public class MultiTask_DataObj
        {
            public Dictionary<CsOutput, int> mMappings;
            public String[] mValues;
            public String msFileName;
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
        public static IntegrationUtils.IntegrationResult processSchema_MultiTask(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, int singleLineNumber) //throws IOException, InterruptedException 
        {
            // set up a mapping of output field positions in the CSV file
            Dictionary<CsOutput, int> mappings = new Dictionary<CsOutput, int>(100);
            mappings[CsOutput.AJCC6_T] = 42;
            mappings[CsOutput.AJCC6_TDESCRIPTOR] = 43;
            mappings[CsOutput.AJCC6_N] = 44;
            mappings[CsOutput.AJCC6_NDESCRIPTOR] = 45;
            mappings[CsOutput.AJCC6_M] = 46;
            mappings[CsOutput.AJCC6_MDESCRIPTOR] = 47;
            mappings[CsOutput.AJCC6_STAGE] = 48;
            mappings[CsOutput.AJCC7_T] = 49;
            mappings[CsOutput.AJCC7_TDESCRIPTOR] = 50;
            mappings[CsOutput.AJCC7_N] = 51;
            mappings[CsOutput.AJCC7_NDESCRIPTOR] = 52;
            mappings[CsOutput.AJCC7_M] = 53;
            mappings[CsOutput.AJCC7_MDESCRIPTOR] = 54;
            mappings[CsOutput.AJCC7_STAGE] = 55;
            mappings[CsOutput.SS1977_T] = 56;
            mappings[CsOutput.SS1977_N] = 57;
            mappings[CsOutput.SS1977_M] = 58;
            mappings[CsOutput.SS1977_STAGE] = 59;
            mappings[CsOutput.SS2000_T] = 60;
            mappings[CsOutput.SS2000_N] = 61;
            mappings[CsOutput.SS2000_M] = 62;
            mappings[CsOutput.SS2000_STAGE] = 63;
            mappings[CsOutput.STOR_AJCC6_T] = 64;
            mappings[CsOutput.STOR_AJCC6_TDESCRIPTOR] = 65;
            mappings[CsOutput.STOR_AJCC6_N] = 66;
            mappings[CsOutput.STOR_AJCC6_NDESCRIPTOR] = 67;
            mappings[CsOutput.STOR_AJCC6_M] = 68;
            mappings[CsOutput.STOR_AJCC6_MDESCRIPTOR] = 69;
            mappings[CsOutput.STOR_AJCC6_STAGE] = 70;
            mappings[CsOutput.STOR_AJCC7_T] = 71;
            mappings[CsOutput.STOR_AJCC7_TDESCRIPTOR] = 72;
            mappings[CsOutput.STOR_AJCC7_N] = 73;
            mappings[CsOutput.STOR_AJCC7_NDESCRIPTOR] = 74;
            mappings[CsOutput.STOR_AJCC7_M] = 75;
            mappings[CsOutput.STOR_AJCC7_MDESCRIPTOR] = 76;
            mappings[CsOutput.STOR_AJCC7_STAGE] = 77;
            mappings[CsOutput.STOR_SS1977_STAGE] = 78;
            mappings[CsOutput.STOR_SS2000_STAGE] = 79;

            // go over each file
            int processedCases = 0;
            int iLineNumber = 0;
            //int iLineCounter = 0;

            MultiTasksExecutor thisMultiTasksExecutor = new MultiTasksExecutor();
            thisMultiTasksExecutor.AddAction(new MultiTasksExecutor.ActionCallBack(MultiTask_TaskCompute));

            int iThreads = thisMultiTasksExecutor.GetNumThreads();

            mMultiTask_Staging = staging;
            miMultiTask_FailedCases = 0;
            miMultiTask_ThreadProcessedCases = 0;
            thisMultiTasksExecutor.StartTasks();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (singleLineNumber >= 0)
                IntegrationUtils.WritelineToLog("Starting " + fileName + ", line # " + singleLineNumber + "...");
            else
                IntegrationUtils.WritelineToLog("Starting " + fileName + " [" + (iThreads + 1) + " threads]");

            // loop over each line in the file
            TextReader reader = new StreamReader(inputStream);
            String line = reader.ReadLine();

            while (line != null)
            {
                iLineNumber++;

                if (iLineNumber >= 0)
                {
                    processedCases++;

                    // split the CSV record
                    String[] values = line.Split(",".ToCharArray());

                    // if a single line was requested, skip all other lines
                    if (singleLineNumber >= 0 && singleLineNumber != iLineNumber)
                        continue;

                    if (values.Length != 80)
                        IntegrationUtils.WritelineToLog("Line " + iLineNumber + " has " + values.Length + " cells; it should be 80.");
                    else
                    {
                        MultiTask_DataObj obj = new MultiTask_DataObj();
                        obj.mMappings = mappings;
                        obj.mValues = values;
                        obj.msFileName = fileName;
                        obj.miLineNum = iLineNumber;

                        thisMultiTasksExecutor.AddDataItem(obj);

                        /*
                        iLineCounter++;
                        if (iLineCounter > 50000)
                        {
                            IntegrationUtils.WritelineToLog("Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
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

            //IntegrationUtils.WritelineToLog("Task " + id + " - Executing Line " + thisDataObj.miLineNum + " - " + DateTime.Now.ToString("HH:mm:ss.ffff"));

            // load up inputs
            CsStagingData data = new CsStagingData();
            data.setInput(CsInput.PRIMARY_SITE, thisDataObj.mValues[0]);
            data.setInput(CsInput.HISTOLOGY, thisDataObj.mValues[1]);
            data.setInput(CsInput.DX_YEAR, thisDataObj.mValues[2]);
            data.setInput(CsInput.CS_VERSION_ORIGINAL, thisDataObj.mValues[3]);
            data.setInput(CsInput.BEHAVIOR, thisDataObj.mValues[4]);
            data.setInput(CsInput.GRADE, thisDataObj.mValues[5]);
            data.setInput(CsInput.AGE_AT_DX, thisDataObj.mValues[6]);
            data.setInput(CsInput.LVI, thisDataObj.mValues[7]);
            data.setInput(CsInput.TUMOR_SIZE, thisDataObj.mValues[8]);
            data.setInput(CsInput.EXTENSION, thisDataObj.mValues[9]);
            data.setInput(CsInput.EXTENSION_EVAL, thisDataObj.mValues[10]);
            data.setInput(CsInput.LYMPH_NODES, thisDataObj.mValues[11]);
            data.setInput(CsInput.LYMPH_NODES_EVAL, thisDataObj.mValues[12]);
            data.setInput(CsInput.REGIONAL_NODES_POSITIVE, thisDataObj.mValues[13]);
            data.setInput(CsInput.REGIONAL_NODES_EXAMINED, thisDataObj.mValues[14]);
            data.setInput(CsInput.METS_AT_DX, thisDataObj.mValues[15]);
            data.setInput(CsInput.METS_EVAL, thisDataObj.mValues[16]);
            data.setInput(CsInput.SSF1, thisDataObj.mValues[17]);
            data.setInput(CsInput.SSF2, thisDataObj.mValues[18]);
            data.setInput(CsInput.SSF3, thisDataObj.mValues[19]);
            data.setInput(CsInput.SSF4, thisDataObj.mValues[20]);
            data.setInput(CsInput.SSF5, thisDataObj.mValues[21]);
            data.setInput(CsInput.SSF6, thisDataObj.mValues[22]);
            data.setInput(CsInput.SSF7, thisDataObj.mValues[23]);
            data.setInput(CsInput.SSF8, thisDataObj.mValues[24]);
            data.setInput(CsInput.SSF9, thisDataObj.mValues[25]);
            data.setInput(CsInput.SSF10, thisDataObj.mValues[26]);
            data.setInput(CsInput.SSF11, thisDataObj.mValues[27]);
            data.setInput(CsInput.SSF12, thisDataObj.mValues[28]);
            data.setInput(CsInput.SSF13, thisDataObj.mValues[29]);
            data.setInput(CsInput.SSF14, thisDataObj.mValues[30]);
            data.setInput(CsInput.SSF15, thisDataObj.mValues[31]);
            data.setInput(CsInput.SSF16, thisDataObj.mValues[32]);
            data.setInput(CsInput.SSF17, thisDataObj.mValues[33]);
            data.setInput(CsInput.SSF18, thisDataObj.mValues[34]);
            data.setInput(CsInput.SSF19, thisDataObj.mValues[35]);
            data.setInput(CsInput.SSF20, thisDataObj.mValues[36]);
            data.setInput(CsInput.SSF21, thisDataObj.mValues[37]);
            data.setInput(CsInput.SSF22, thisDataObj.mValues[38]);
            data.setInput(CsInput.SSF23, thisDataObj.mValues[39]);
            data.setInput(CsInput.SSF24, thisDataObj.mValues[40]);
            data.setInput(CsInput.SSF25, thisDataObj.mValues[41]);

            try
            {
                // save the expected outputs
                Dictionary<String, String> output = new Dictionary<String, String>(100, StringComparer.Ordinal);
                String sKeyValue = "";
                foreach (KeyValuePair<CsOutput, int> entry in thisDataObj.mMappings)
                {
                    sKeyValue = entry.Key.toString();
                    output[sKeyValue] = thisDataObj.mValues[entry.Value];
                }

                // run collaborative stage; if no schema found, set the output to empty
                SchemaLookup lookup = new SchemaLookup(data.getInput(CsInput.PRIMARY_SITE), data.getInput(CsInput.HISTOLOGY));
                lookup.setInput(CsStagingData.SSF25_KEY, data.getInput(CsInput.SSF25));
                List<StagingSchema> schemas = mMultiTask_Staging.lookupSchema(lookup);

                if (schemas.Count == 1)
                    mMultiTask_Staging.stage(data);
                else
                {
                    Dictionary<String, String> outResult = new Dictionary<String, String>(2, StringComparer.Ordinal);
                    outResult["schema_id"] = "<invalid>";

                    data.setOutput(outResult);
                }

                List<String> mismatches = new List<String>();

                // compare results
                foreach (KeyValuePair<String, String> entry in output)
                {
                    String expected = "";
                    output.TryGetValue(entry.Key, out expected);
                    if (expected == null) expected = "";
                    expected = expected.Trim();

                    String actual = "";
                    data.getOutput().TryGetValue(entry.Key, out actual);
                    if (actual == null) actual = "";
                    actual = actual.Trim();

                    if (expected != actual)
                        mismatches.Add("   " + thisDataObj.miLineNum + " --> " + entry.Key + ": EXPECTED '" + expected + "' ACTUAL: '" + actual + "'");
                }

                if (mismatches.Count != 0)
                {
                    String sSchemaIDValue = data.getSchemaId();

                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> [" + sSchemaIDValue + "] Mismatches in " + thisDataObj.msFileName);
                    foreach (String mismatch in mismatches)
                    {
                        IntegrationUtils.WritelineToLog(mismatch);
                    }
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " *** RESULT: " + data.getResult());
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> " + IntegrationUtils.convertInputMap(data.getInput()));
                    if (data.getErrors().Count > 0)
                    {
                        IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> ERRORS: ");
                        foreach (Error e in data.getErrors())
                            IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> (" + e.getTable() + ": " + e.getMessage() + ") ");
                    }

                    Interlocked.Increment(ref miMultiTask_FailedCases);
                }
            }
            catch (Exception e)
            {
                IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Exception processing " + thisDataObj.msFileName + " : " + e.Message);
                Interlocked.Increment(ref miMultiTask_FailedCases);
            }

            Interlocked.Increment(ref miMultiTask_ThreadProcessedCases);
        }


        
    }
}
