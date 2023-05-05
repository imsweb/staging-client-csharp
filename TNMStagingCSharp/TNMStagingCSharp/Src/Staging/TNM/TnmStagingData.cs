// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStagingCSharp.Src.Staging.TNM
{

    // input key definitions
    public class TnmInput
    {
        public static readonly TnmInput PRIMARY_SITE = new TnmInput("site");
        public static readonly TnmInput HISTOLOGY = new TnmInput("hist");
        public static readonly TnmInput BEHAVIOR = new TnmInput("behavior");
        public static readonly TnmInput GRADE = new TnmInput("grade");
        public static readonly TnmInput SEX = new TnmInput("sex");
        public static readonly TnmInput DX_YEAR = new TnmInput("year_dx");
        public static readonly TnmInput AGE_AT_DX = new TnmInput("age_dx");
        public static readonly TnmInput CLIN_STAGE_GROUP_DIRECT = new TnmInput("clin_stage_group_direct");
        public static readonly TnmInput PATH_STAGE_GROUP_DIRECT = new TnmInput("path_stage_group_direct");
        public static readonly TnmInput RX_SUMM_SURGERY = new TnmInput("systemic_surg_seq");
        public static readonly TnmInput RX_SUMM_RADIATION = new TnmInput("radiation_surg_seq");
        public static readonly TnmInput CLIN_T = new TnmInput("clin_t");
        public static readonly TnmInput CLIN_N = new TnmInput("clin_n");
        public static readonly TnmInput CLIN_M = new TnmInput("clin_m");
        public static readonly TnmInput PATH_T = new TnmInput("path_t");
        public static readonly TnmInput PATH_N = new TnmInput("path_n");
        public static readonly TnmInput PATH_M = new TnmInput("path_m");
        public static readonly TnmInput REGIONAL_NODES_POSITIVE = new TnmInput("nodes_pos");
        public static readonly TnmInput SSF1 = new TnmInput("ssf1");
        public static readonly TnmInput SSF2 = new TnmInput("ssf2");
        public static readonly TnmInput SSF3 = new TnmInput("ssf3");
        public static readonly TnmInput SSF4 = new TnmInput("ssf4");
        public static readonly TnmInput SSF5 = new TnmInput("ssf5");
        public static readonly TnmInput SSF6 = new TnmInput("ssf6");
        public static readonly TnmInput SSF7 = new TnmInput("ssf7");
        public static readonly TnmInput SSF8 = new TnmInput("ssf8");
        public static readonly TnmInput SSF9 = new TnmInput("ssf9");
        public static readonly TnmInput SSF10 = new TnmInput("ssf10");
        public static readonly TnmInput SSF11 = new TnmInput("ssf11");
        public static readonly TnmInput SSF12 = new TnmInput("ssf12");
        public static readonly TnmInput SSF13 = new TnmInput("ssf13");
        public static readonly TnmInput SSF14 = new TnmInput("ssf14");
        public static readonly TnmInput SSF15 = new TnmInput("ssf15");
        public static readonly TnmInput SSF16 = new TnmInput("ssf16");
        public static readonly TnmInput SSF17 = new TnmInput("ssf17");
        public static readonly TnmInput SSF18 = new TnmInput("ssf18");
        public static readonly TnmInput SSF19 = new TnmInput("ssf19");
        public static readonly TnmInput SSF20 = new TnmInput("ssf20");
        public static readonly TnmInput SSF21 = new TnmInput("ssf21");
        public static readonly TnmInput SSF22 = new TnmInput("ssf22");
        public static readonly TnmInput SSF23 = new TnmInput("ssf23");
        public static readonly TnmInput SSF24 = new TnmInput("ssf24");

        public static IEnumerable<TnmInput> Values
        {
            get
            {
                yield return PRIMARY_SITE;
                yield return HISTOLOGY;
                yield return BEHAVIOR;
                yield return GRADE;
                yield return SEX;
                yield return DX_YEAR;
                yield return AGE_AT_DX;
                yield return CLIN_STAGE_GROUP_DIRECT;
                yield return PATH_STAGE_GROUP_DIRECT;
                yield return RX_SUMM_SURGERY;
                yield return RX_SUMM_RADIATION;
                yield return CLIN_T;
                yield return CLIN_N;
                yield return CLIN_M;
                yield return PATH_T;
                yield return PATH_N;
                yield return PATH_M;
                yield return REGIONAL_NODES_POSITIVE;
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
            }
        }

        private readonly String _name;

        TnmInput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }


    // output key definitions
    public class TnmOutput
    {
        public static readonly TnmOutput DERIVED_VERSION = new TnmOutput("derived_version");
        public static readonly TnmOutput CLIN_STAGE_GROUP = new TnmOutput("clin_stage_group");
        public static readonly TnmOutput PATH_STAGE_GROUP = new TnmOutput("path_stage_group");
        public static readonly TnmOutput COMBINED_STAGE_GROUP = new TnmOutput("combined_stage_group");
        public static readonly TnmOutput SOURCE_T = new TnmOutput("source_t");
        public static readonly TnmOutput COMBINED_T = new TnmOutput("combined_t");
        public static readonly TnmOutput SOURCE_N = new TnmOutput("source_n");
        public static readonly TnmOutput COMBINED_N = new TnmOutput("combined_n");
        public static readonly TnmOutput SOURCE_M = new TnmOutput("source_m");
        public static readonly TnmOutput COMBINED_M = new TnmOutput("combined_m");

        public static IEnumerable<TnmOutput> Values
        {
            get
            {
                yield return DERIVED_VERSION;
                yield return CLIN_STAGE_GROUP;
                yield return PATH_STAGE_GROUP;
                yield return COMBINED_STAGE_GROUP;
                yield return SOURCE_T;
                yield return COMBINED_T;
                yield return SOURCE_N;
                yield return COMBINED_N;
                yield return SOURCE_M;
                yield return COMBINED_M;
            }
        }

        private readonly String _name;

        TnmOutput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }



    public class TnmStagingData: StagingData
    {
        // key definitions
        public static readonly String SEX_KEY = "sex";
        public static readonly String SSF25_KEY = "ssf25";

        // SSF prefix
        public static readonly String INPUT_SSF_PREFIX = "ssf";


        // Default constructor
        public TnmStagingData(): base ()
        {
        }

        // Construct with site and histology
        // @param site primary site
        // @param hist histology
        public TnmStagingData(String site, String hist): base(site, hist)
        {
        }

        // Construct with site, histology and SSF25
        // @param site primary site
        // @param hist histology
        // @param ssf25 site-specific factor 25
        public TnmStagingData(String site, String hist, String ssf25): base(site, hist)
        {
            setSsf(25, ssf25);
        }

        // Return the specified input value
        // @param key input key
        // @return input
        public String getInput(TnmInput key)
        {
            return getInput(key.toString());
        }


        // Set the specified input value
        // @param key input key
        // @param value value
        public void setInput(TnmInput key, String value)
        {
            setInput(key.toString(), value);
        }


        // Return the specified output value
        // @param key output key
        // @return output
        public String getOutput(TnmOutput key)
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


        // TnmStagingInputBuilder builder
        public class TnmStagingInputBuilder
        {

            private readonly TnmStagingData _data = new TnmStagingData();

            public TnmStagingInputBuilder()
            {
                //_data = new TnmStagingData();
            }

            public TnmStagingInputBuilder withSsf(int id, String ssf)
            {
                _data.setSsf(id, ssf);
                return this;
            }

            public TnmStagingInputBuilder withInput(TnmInput key, String value)
            {
                _data.setInput(key, value);
                return this;
            }

            public TnmStagingData build()
            {
                return _data;
            }
        }
    }
}


 