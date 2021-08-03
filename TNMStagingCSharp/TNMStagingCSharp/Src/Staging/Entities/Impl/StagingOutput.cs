// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;

namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    // A wrapper class for the decision engine Result class.  It exists to add the JSON mappings.
    public class StagingOutput
    {
        Result _result;
        Dictionary<String, String> _input;

        // Constructor
        // @param result Result of staging call
        public StagingOutput(Result result)
        {
            _result = result;
        }

        public Dictionary<String, String> getOutput()
        {
            return _result.getContext();
        }

        public List<String> getPath()
        {
            return _result.getPath();
        }

        public List<Error> getErrors()
        {
            return _result.getErrors();
        }

        public Dictionary<String, String> getInput()
        {
            return _input;
        }

        public void setInput(Dictionary<String, String> input)
        {
            _input = input;
        }
    }
}
