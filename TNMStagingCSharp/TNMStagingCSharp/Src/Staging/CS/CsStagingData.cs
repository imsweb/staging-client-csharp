// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.Staging.Entities;

namespace TNMStagingCSharp.Src.Staging.CS
{

    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    public class CsInput
    {
        public static readonly CsInput PRIMARY_SITE = new CsInput("site");
        public static readonly CsInput HISTOLOGY = new CsInput("hist");
        public static readonly CsInput DX_YEAR = new CsInput("year_dx");
        public static readonly CsInput CS_VERSION_ORIGINAL = new CsInput("cs_input_version_original");
        public static readonly CsInput BEHAVIOR = new CsInput("behavior");
        public static readonly CsInput GRADE = new CsInput("grade");
        public static readonly CsInput LVI = new CsInput("lvi");
        public static readonly CsInput AGE_AT_DX = new CsInput("age_dx");
        public static readonly CsInput SEX = new CsInput("sex");
        public static readonly CsInput TUMOR_SIZE = new CsInput("size");
        public static readonly CsInput EXTENSION = new CsInput("extension");
        public static readonly CsInput EXTENSION_EVAL = new CsInput("extension_eval");
        public static readonly CsInput LYMPH_NODES = new CsInput("nodes");
        public static readonly CsInput LYMPH_NODES_EVAL = new CsInput("nodes_eval");
        public static readonly CsInput REGIONAL_NODES_POSITIVE = new CsInput("nodes_pos");
        public static readonly CsInput REGIONAL_NODES_EXAMINED = new CsInput("nodes_exam");
        public static readonly CsInput METS_AT_DX = new CsInput("mets");
        public static readonly CsInput METS_EVAL = new CsInput("mets_eval");
        public static readonly CsInput SSF1 = new CsInput("ssf1");
        public static readonly CsInput SSF2 = new CsInput("ssf2");
        public static readonly CsInput SSF3 = new CsInput("ssf3");
        public static readonly CsInput SSF4 = new CsInput("ssf4");
        public static readonly CsInput SSF5 = new CsInput("ssf5");
        public static readonly CsInput SSF6 = new CsInput("ssf6");
        public static readonly CsInput SSF7 = new CsInput("ssf7");
        public static readonly CsInput SSF8 = new CsInput("ssf8");
        public static readonly CsInput SSF9 = new CsInput("ssf9");
        public static readonly CsInput SSF10 = new CsInput("ssf10");
        public static readonly CsInput SSF11 = new CsInput("ssf11");
        public static readonly CsInput SSF12 = new CsInput("ssf12");
        public static readonly CsInput SSF13 = new CsInput("ssf13");
        public static readonly CsInput SSF14 = new CsInput("ssf14");
        public static readonly CsInput SSF15 = new CsInput("ssf15");
        public static readonly CsInput SSF16 = new CsInput("ssf16");
        public static readonly CsInput SSF17 = new CsInput("ssf17");
        public static readonly CsInput SSF18 = new CsInput("ssf18");
        public static readonly CsInput SSF19 = new CsInput("ssf19");
        public static readonly CsInput SSF20 = new CsInput("ssf20");
        public static readonly CsInput SSF21 = new CsInput("ssf21");
        public static readonly CsInput SSF22 = new CsInput("ssf22");
        public static readonly CsInput SSF23 = new CsInput("ssf23");
        public static readonly CsInput SSF24 = new CsInput("ssf24");
        public static readonly CsInput SSF25 = new CsInput("ssf25");

        public static IEnumerable<CsInput> Values
        {
            get
            {
                yield return PRIMARY_SITE;
                yield return HISTOLOGY;
                yield return DX_YEAR;
                yield return CS_VERSION_ORIGINAL;
                yield return BEHAVIOR;
                yield return GRADE;
                yield return LVI;
                yield return AGE_AT_DX;
                yield return SEX;
                yield return TUMOR_SIZE;
                yield return EXTENSION;
                yield return EXTENSION_EVAL;
                yield return LYMPH_NODES;
                yield return LYMPH_NODES_EVAL;
                yield return REGIONAL_NODES_POSITIVE;
                yield return REGIONAL_NODES_EXAMINED;
                yield return METS_AT_DX;
                yield return METS_EVAL;
                yield return SSF1;
                yield return SSF2;
                yield return SSF3;
                yield return SSF4;
                yield return SSF5;
                yield return SSF6;
                yield return SSF7;
                yield return SSF8;
                yield return SSF9;
                yield return SSF10;
                yield return SSF11;
                yield return SSF12;
                yield return SSF13;
                yield return SSF14;
                yield return SSF15;
                yield return SSF16;
                yield return SSF17;
                yield return SSF18;
                yield return SSF19;
                yield return SSF20;
                yield return SSF21;
                yield return SSF22;
                yield return SSF23;
                yield return SSF24;
                yield return SSF25;
            }
        }


        private readonly String _name;

        CsInput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }

    //========================================================================================================================
    // output key definitions
    //========================================================================================================================
    public class CsOutput
    {
        public static readonly CsOutput SCHEMA_NUMBER = new CsOutput("schema_number");
        public static readonly CsOutput CSVER_DERIVED = new CsOutput("csver_derived");
        public static readonly CsOutput AJCC6_T = new CsOutput("ajcc6_t");
        public static readonly CsOutput AJCC6_TDESCRIPTOR = new CsOutput("ajcc6_tdescriptor");
        public static readonly CsOutput AJCC6_N = new CsOutput("ajcc6_n");
        public static readonly CsOutput AJCC6_NDESCRIPTOR = new CsOutput("ajcc6_ndescriptor");
        public static readonly CsOutput AJCC6_M = new CsOutput("ajcc6_m");
        public static readonly CsOutput AJCC6_MDESCRIPTOR = new CsOutput("ajcc6_mdescriptor");
        public static readonly CsOutput AJCC6_STAGE = new CsOutput("ajcc6_stage");
        public static readonly CsOutput AJCC7_T = new CsOutput("ajcc7_t");
        public static readonly CsOutput AJCC7_TDESCRIPTOR = new CsOutput("ajcc7_tdescriptor");
        public static readonly CsOutput AJCC7_N = new CsOutput("ajcc7_n");
        public static readonly CsOutput AJCC7_NDESCRIPTOR = new CsOutput("ajcc7_ndescriptor");
        public static readonly CsOutput AJCC7_M = new CsOutput("ajcc7_m");
        public static readonly CsOutput AJCC7_MDESCRIPTOR = new CsOutput("ajcc7_mdescriptor");
        public static readonly CsOutput AJCC7_STAGE = new CsOutput("ajcc7_stage");
        public static readonly CsOutput SS1977_T = new CsOutput("t77");
        public static readonly CsOutput SS1977_N = new CsOutput("n77");
        public static readonly CsOutput SS1977_M = new CsOutput("m77");
        public static readonly CsOutput SS1977_STAGE = new CsOutput("ss77");
        public static readonly CsOutput SS2000_T = new CsOutput("t2000");
        public static readonly CsOutput SS2000_N = new CsOutput("n2000");
        public static readonly CsOutput SS2000_M = new CsOutput("m2000");
        public static readonly CsOutput SS2000_STAGE = new CsOutput("ss2000");
        public static readonly CsOutput STOR_AJCC6_T = new CsOutput("stor_ajcc6_t");
        public static readonly CsOutput STOR_AJCC6_TDESCRIPTOR = new CsOutput("stor_ajcc6_tdescriptor");
        public static readonly CsOutput STOR_AJCC6_N = new CsOutput("stor_ajcc6_n");
        public static readonly CsOutput STOR_AJCC6_NDESCRIPTOR = new CsOutput("stor_ajcc6_ndescriptor");
        public static readonly CsOutput STOR_AJCC6_M = new CsOutput("stor_ajcc6_m");
        public static readonly CsOutput STOR_AJCC6_MDESCRIPTOR = new CsOutput("stor_ajcc6_mdescriptor");
        public static readonly CsOutput STOR_AJCC6_STAGE = new CsOutput("stor_ajcc6_stage");
        public static readonly CsOutput STOR_AJCC7_T = new CsOutput("stor_ajcc7_t");
        public static readonly CsOutput STOR_AJCC7_TDESCRIPTOR = new CsOutput("stor_ajcc7_tdescriptor");
        public static readonly CsOutput STOR_AJCC7_N = new CsOutput("stor_ajcc7_n");
        public static readonly CsOutput STOR_AJCC7_NDESCRIPTOR = new CsOutput("stor_ajcc7_ndescriptor");
        public static readonly CsOutput STOR_AJCC7_M = new CsOutput("stor_ajcc7_m");
        public static readonly CsOutput STOR_AJCC7_MDESCRIPTOR = new CsOutput("stor_ajcc7_mdescriptor");
        public static readonly CsOutput STOR_AJCC7_STAGE = new CsOutput("stor_ajcc7_stage");
        public static readonly CsOutput STOR_SS1977_STAGE = new CsOutput("stor_ss77");
        public static readonly CsOutput STOR_SS2000_STAGE = new CsOutput("stor_ss2000");

        public static IEnumerable<CsOutput> Values
        {
            get
            {
                yield return SCHEMA_NUMBER;
                yield return CSVER_DERIVED;
                yield return AJCC6_T;
                yield return AJCC6_TDESCRIPTOR;
                yield return AJCC6_N;
                yield return AJCC6_NDESCRIPTOR;
                yield return AJCC6_M;
                yield return AJCC6_MDESCRIPTOR;
                yield return AJCC6_STAGE;
                yield return AJCC7_T;
                yield return AJCC7_TDESCRIPTOR;
                yield return AJCC7_N;
                yield return AJCC7_NDESCRIPTOR;
                yield return AJCC7_M;
                yield return AJCC7_MDESCRIPTOR;
                yield return AJCC7_STAGE;
                yield return SS1977_T;
                yield return SS1977_N;
                yield return SS1977_M;
                yield return SS1977_STAGE;
                yield return SS2000_T;
                yield return SS2000_N;
                yield return SS2000_M;
                yield return SS2000_STAGE;
                yield return STOR_AJCC6_T;
                yield return STOR_AJCC6_TDESCRIPTOR;
                yield return STOR_AJCC6_N;
                yield return STOR_AJCC6_NDESCRIPTOR;
                yield return STOR_AJCC6_M;
                yield return STOR_AJCC6_MDESCRIPTOR;
                yield return STOR_AJCC6_STAGE;
                yield return STOR_AJCC7_T;
                yield return STOR_AJCC7_TDESCRIPTOR;
                yield return STOR_AJCC7_N;
                yield return STOR_AJCC7_NDESCRIPTOR;
                yield return STOR_AJCC7_M;
                yield return STOR_AJCC7_MDESCRIPTOR;
                yield return STOR_AJCC7_STAGE;
                yield return STOR_SS1977_STAGE;
                yield return STOR_SS2000_STAGE;
            }
        }

        private readonly String _name;

        CsOutput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }


    public class CsStagingData: StagingData
    {
        //========================================================================================================================
        // key definitions
        //========================================================================================================================
        public static readonly String SSF25_KEY = "ssf25";

        //========================================================================================================================
        // SSF prefix
        //========================================================================================================================
        public static readonly String INPUT_SSF_PREFIX = "ssf";

        //========================================================================================================================
        // Default constructor
        //========================================================================================================================
        public CsStagingData(): base() 
        {
        }

        // Construct with site and histology
        // @param site primary site
        // @param hist histology
        public CsStagingData(String site, String hist): base(site, hist)
        {
        }

        // Construct with site, histology and SSF25
        // @param site primary site
        // @param hist histology
        // @param ssf25 site-specific factor 25
        public CsStagingData(String site, String hist, String ssf25): base(site, hist)
        {
            setSsf(25, ssf25);
        }

        // Return the specified input value
        // @param key input key
        // @return input
        public String getInput(CsInput key)
        {
            return getInput(key.toString());
        }

        // Set the specified input value
        // @param key input key
        // @param value value
        public void setInput(CsInput key, String value)
        {
            setInput(key.toString(), value);
        }

        // Return the specified output value
        // @param key output key
        // @return output
        public String getOutput(CsOutput key)
        {
            return getOutput(key.toString());
        }

        // Get the specified input site-specific factor
        // @param id site-specific factor number
        // @return ssf value
        public String getSsf(int id)
        {
            if (id < 1 || id > 25)
                throw new System.InvalidOperationException("Site specific factor must be between 1 and 25.");

            return getInput(INPUT_SSF_PREFIX + id);
        }

        // Set the specified input site-specific factor
        // @param id site-specific factor number
        // @param ssf site-specfic factor value
        public void setSsf(int id, String ssf)
        {
            if (id < 1 || id > 25)
                throw new System.InvalidOperationException("Site specific factor must be between 1 and 25.");

            setInput(INPUT_SSF_PREFIX + id, ssf);
        }

        // CsStagingInputBuilder builder
        public class CsStagingInputBuilder
        {
            private readonly CsStagingData _data = new CsStagingData();

            public CsStagingInputBuilder()
            {
                //_data = new CsStagingData();
            }

            public CsStagingInputBuilder withSsf(int id, String ssf)
            {
                _data.setSsf(id, ssf);
                return this;
            }

            public CsStagingInputBuilder withInput(CsInput key, String value)
            {
                _data.setInput(key, value);
                return this;
            }

            public CsStagingData build()
            {
                return _data;
            }
        }
    }
}

