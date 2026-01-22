using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.TNM;

namespace TNMStaging_UnitTestApp.Src.Staging.TNM
{
    class TnmIntegrationSchemaStage
    {

        // * Process all schemas in the TNM file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processTNMSchema(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, bool bJSONFormat) //throws IOException, InterruptedException 
        {
            return processTNMSchema(staging, fileName, inputStream, -1, bJSONFormat);
        }


        public class MultiTask_DataObj
        {
            public Dictionary<TnmInput, String> mInputValues;
            public Dictionary<TnmOutput, String> mOutputValues;
            public bool mbJSONFormat;
            public String msExpectedResult;
            public String msFileName;
            public int miLineNum;
        }

        public static TNMStagingCSharp.Src.Staging.Staging mMultiTask_Staging;
        public static int miMultiTask_FailedCases = 0;
        public static int miMultiTask_ThreadProcessedCases = 0;


        // * Process all schemas in TNM file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @param singleLineNumber if not null, only process this line number
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processTNMSchema(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, int singleLineNumber, bool bJSONFormat) //throws IOException, InterruptedException 
        {
            Dictionary<TnmOutput, String> output_values = null;
            Dictionary<TnmInput, String> input_values = null;

            // initialize the threads pool
            int n = Math.Min(9, Environment.ProcessorCount + 1);
            ThreadPool.SetMaxThreads(n, n);

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
                System.Diagnostics.Trace.WriteLine("Starting " + fileName + ", line # " + singleLineNumber + " [" + n + " threads]");
            else
                System.Diagnostics.Trace.WriteLine("Starting " + fileName + " [" + (iThreads + 1) + " threads]");

            // loop over each line in the file
            TextReader reader = new StreamReader(inputStream);
            String line = reader.ReadLine();
            String input_line = "";
            String output_line = "";
            String[] input_strs;
            String[] output_strs;
            String[] entries;
            String sExpectedResult = "";
            bool bStageThis = true;

            while (line != null)
            {
                iLineNumber++;

                if (iLineNumber >= 0)
                {
                    bStageThis = true;
                    if (bJSONFormat)
                    {
                        bStageThis = false;
                        if (line.IndexOf("input=") >= 0) input_line = line.Trim();
                        if (line.IndexOf("expectedOutput=") >= 0) output_line = line.Trim();

                        if (output_line.Length > 0)
                        {
                            bStageThis = true;
                            input_values = new Dictionary<TnmInput, String>();
                            output_values = new Dictionary<TnmOutput, String>();

                            input_line = input_line.Substring(7, input_line.Length - 8).Trim();
                            output_line = output_line.Substring(16, output_line.Length - 17).Trim();

                            input_strs = input_line.Split(",".ToCharArray());
                            output_strs = output_line.Split(",".ToCharArray());

                            // set up a mapping of output field positions in the CSV file

                            foreach (String s in input_strs)
                            {
                                entries = s.Split("=".ToCharArray());
                                if (entries.Length == 2)
                                {
                                    entries[0] = entries[0].Trim();
                                    entries[1] = entries[1].Trim();
                                    foreach (TnmInput inp in TnmInput.Values)
                                    {
                                        if (inp.toString() == entries[0])
                                        {
                                            input_values.Add(inp, entries[1]);
                                        }
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Trace.WriteLine("Line " + iLineNumber + " has " + entries.Length + " cells; it should be 2. (" + input_line + ")");
                                }
                            }

                            foreach (String s in output_strs)
                            {
                                entries = s.Split("=".ToCharArray());
                                if (entries.Length == 2)
                                {
                                    entries[0] = entries[0].Trim();
                                    entries[1] = entries[1].Trim();
                                    foreach (TnmOutput outp in TnmOutput.Values)
                                    {
                                        if (outp.toString() == entries[0])
                                        {
                                            output_values.Add(outp, entries[1]);
                                        }
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Trace.WriteLine("Line " + iLineNumber + " has " + entries.Length + " cells; it should be 2. (" + output_line + ")");
                                }
                            }
                        }
                    }
                    else
                    {
                        input_values = new Dictionary<TnmInput, String>();
                        output_values = new Dictionary<TnmOutput, String>();

                        // Each line is a comma delimited string.
                        input_strs = line.Split(",".ToCharArray());

                        if (input_strs.Length == 54)
                        {
                            String sVal = "";
                            for (int i=0; i < 54; i++)
                            {
                                sVal = input_strs[i].Trim();
                                if (sVal.Length > 0)
                                {
                                    TnmInput in_key = TnmInput.PRIMARY_SITE;
                                    TnmOutput out_key = TnmOutput.DERIVED_VERSION;
                                    switch (i)
                                    {
                                        case 0: in_key = TnmInput.PRIMARY_SITE; break;
                                        case 1: in_key = TnmInput.HISTOLOGY; break;
                                        case 2: in_key = TnmInput.DX_YEAR; break;
                                        case 3: in_key = TnmInput.BEHAVIOR; break;
                                        case 4: in_key = TnmInput.GRADE; break;
                                        case 5: in_key = TnmInput.SEX; break;
                                        case 6: in_key = TnmInput.AGE_AT_DX; break;
                                        case 7: in_key = TnmInput.RX_SUMM_SURGERY; break;
                                        case 8: in_key = TnmInput.RX_SUMM_RADIATION; break;
                                        case 9: in_key = TnmInput.REGIONAL_NODES_POSITIVE; break;
                                        case 10: in_key = TnmInput.CLIN_T; break;
                                        case 11: in_key = TnmInput.CLIN_N; break;
                                        case 12: in_key = TnmInput.CLIN_M; break;
                                        case 13: in_key = TnmInput.CLIN_STAGE_GROUP_DIRECT; break;
                                        case 14: in_key = TnmInput.PATH_T; break;
                                        case 15: in_key = TnmInput.PATH_N; break;
                                        case 16: in_key = TnmInput.PATH_M; break;
                                        case 17: in_key = TnmInput.PATH_STAGE_GROUP_DIRECT; break;
                                        case 18: in_key = TnmInput.SSF1; break;
                                        case 19: in_key = TnmInput.SSF2; break;
                                        case 20: in_key = TnmInput.SSF3; break;
                                        case 21: in_key = TnmInput.SSF4; break;
                                        case 22: in_key = TnmInput.SSF5; break;
                                        case 23: in_key = TnmInput.SSF6; break;
                                        case 24: in_key = TnmInput.SSF7; break;
                                        case 25: in_key = TnmInput.SSF8; break;
                                        case 26: in_key = TnmInput.SSF9; break;
                                        case 27: in_key = TnmInput.SSF10; break;
                                        case 28: in_key = TnmInput.SSF11; break;
                                        case 29: in_key = TnmInput.SSF12; break;
                                        case 30: in_key = TnmInput.SSF13; break;
                                        case 31: in_key = TnmInput.SSF14; break;
                                        case 32: in_key = TnmInput.SSF15; break;
                                        case 33: in_key = TnmInput.SSF16; break;
                                        case 34: in_key = TnmInput.SSF17; break;
                                        case 35: in_key = TnmInput.SSF18; break;
                                        case 36: in_key = TnmInput.SSF19; break;
                                        case 37: in_key = TnmInput.SSF20; break;
                                        case 38: in_key = TnmInput.SSF21; break;
                                        case 39: in_key = TnmInput.SSF22; break;
                                        case 40: in_key = TnmInput.SSF23; break;
                                        case 41: in_key = TnmInput.SSF24; break;
                                        //case 42: in_key = TnmInput.SSF25; break;
                                        case 43: out_key = TnmOutput.DERIVED_VERSION; break;
                                        case 44: out_key = TnmOutput.CLIN_STAGE_GROUP; break;
                                        case 45: out_key = TnmOutput.PATH_STAGE_GROUP; break;
                                        case 46: out_key = TnmOutput.COMBINED_STAGE_GROUP; break;
                                        case 47: out_key = TnmOutput.COMBINED_T; break;
                                        case 48: out_key = TnmOutput.COMBINED_N; break;
                                        case 49: out_key = TnmOutput.COMBINED_M; break;
                                        case 50: out_key = TnmOutput.SOURCE_T; break;
                                        case 51: out_key = TnmOutput.SOURCE_N; break;
                                        case 52: out_key = TnmOutput.SOURCE_M; break;
                                    }
                                    if (i <= 42)
                                    {
                                        input_values.Add(in_key, sVal);
                                    }
                                    else if (i <= 52)
                                    {
                                        output_values.Add(out_key, sVal);
                                    }
                                }
                            }
                            sExpectedResult = input_strs[53];
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine("Error: Line " + iLineNumber + " has " + input_strs.Length + " entries.");
                        }
                    }

                    if (bStageThis)
                    {
                        processedCases++;

                        MultiTask_DataObj obj = new MultiTask_DataObj();
                        obj.mInputValues = input_values;
                        obj.mOutputValues = output_values;
                        obj.mbJSONFormat = bJSONFormat;
                        obj.msExpectedResult = sExpectedResult;
                        obj.msFileName = fileName;
                        obj.miLineNum = iLineNumber;

                        thisMultiTasksExecutor.AddDataItem(obj);

                        // DEBUG
                        /*
                        iLineCounter++;
                        if (iLineCounter >= 50000)
                        {
                            IntegrationUtils.WritelineToLog("Line: " + iLineNumber + "   Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
                            iLineCounter = 0;
                        }
                        */

                        input_line = "";
                        output_line = "";

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
                System.Diagnostics.Trace.WriteLine("There were " + miMultiTask_FailedCases + " failures.");
            else
                System.Diagnostics.Trace.WriteLine("");


            System.Diagnostics.Trace.WriteLine("-----------------------------------------------");

            inputStream.Close();

            return new IntegrationUtils.IntegrationResult(processedCases, miMultiTask_FailedCases);
        }

        public static void MultiTask_TaskCompute(int id, Object task_data)
        {
            MultiTask_DataObj thisDataObj = (MultiTask_DataObj)task_data;

            try
            {
                // load up inputs
                TnmStagingData data = new TnmStagingData();
                foreach (KeyValuePair<TnmInput, String> kp in thisDataObj.mInputValues)
                {
                    data.setInput(kp.Key, kp.Value);
                }
                
                // save the expected outputs
                Dictionary<String, String> output = new Dictionary<String, String>(100, StringComparer.Ordinal);
                String sKeyValue = "";
                foreach (KeyValuePair<TnmOutput, String> entry in thisDataObj.mOutputValues)
                {
                    sKeyValue = entry.Key.toString();
                    output[sKeyValue] = entry.Value;
                }


                // run collaborative stage; if no schema found, set the output to empty
                SchemaLookup lookup = new SchemaLookup(data.getInput(TnmInput.PRIMARY_SITE), data.getInput(TnmInput.HISTOLOGY));
                lookup.setInput(TnmStagingData.SEX_AT_BIRTH_KEY, data.getInput(TnmInput.SEX));
                //lookup.setInput(TnmStagingData.SSF25_KEY, data.getInput(TnmInput.SSF25));
                List<Schema> schemas = mMultiTask_Staging.lookupSchema(lookup);

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
                if (!thisDataObj.mbJSONFormat)
                {
                    String sNewResultStr = "";
                    String sOldResultStr = thisDataObj.msExpectedResult.Trim();
                    if (data.getResult() == StagingData.Result.STAGED) sNewResultStr = "STAGED";
                    else if (data.getResult() == StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY) sNewResultStr = "FAILED_MISSING_SITE_OR_HISTOLOGY";
                    else if (data.getResult() == StagingData.Result.FAILED_NO_MATCHING_SCHEMA) sNewResultStr = "FAILED_NO_MATCHING_SCHEMA";
                    else if (data.getResult() == StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS) sNewResultStr = "FAILED_MULITPLE_MATCHING_SCHEMAS";
                    else if (data.getResult() == StagingData.Result.FAILED_INVALID_YEAR_DX) sNewResultStr = "FAILED_INVALID_YEAR_DX";
                    else if (data.getResult() == StagingData.Result.FAILED_INVALID_INPUT) sNewResultStr = "FAILED_INVALID_INPUT";

                    if (sNewResultStr != sOldResultStr)
                    {
                        mismatches.Add("   " + thisDataObj.miLineNum + " --> Result: EXPECTED '" + sOldResultStr + "' ACTUAL: '" + sNewResultStr + "'");
                    }
                }

                // compare output
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
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Input: " + IntegrationUtils.convertInputMap(data.getInput()));
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Output: " + IntegrationUtils.convertInputMap(data.getOutput()));
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