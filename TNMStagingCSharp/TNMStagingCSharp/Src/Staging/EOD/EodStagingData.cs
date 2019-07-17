/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.EOD
{

    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    public class EodInput
    {
        public static readonly EodInput PRIMARY_SITE = new EodInput("site");
        public static readonly EodInput HISTOLOGY = new EodInput("hist");
        public static readonly EodInput BEHAVIOR = new EodInput("behavior");
        public static readonly EodInput SEX = new EodInput("sex");
        public static readonly EodInput AGE_AT_DX = new EodInput("age_dx");
        public static readonly EodInput DISCRIMINATOR_1 = new EodInput("discriminator_1");
        public static readonly EodInput DISCRIMINATOR_2 = new EodInput("discriminator_2");
        public static readonly EodInput NODES_POS = new EodInput("nodes_pos");
        public static readonly EodInput NODES_EXAM = new EodInput("nodes_exam");
        public static readonly EodInput EOD_PRIMARY_TUMOR = new EodInput("eod_primary_tumor");
        public static readonly EodInput EOD_REGIONAL_NODES = new EodInput("eod_regional_nodes");
        public static readonly EodInput EOD_METS = new EodInput("eod_mets");
        public static readonly EodInput GRADE_CLIN = new EodInput("grade_clin");
        public static readonly EodInput GRADE_PATH = new EodInput("grade_path");
        public static readonly EodInput GRADE_POST_THERAPY = new EodInput("grade_post_therapy");
        public static readonly EodInput DX_YEAR = new EodInput("year_dx");
        public static readonly EodInput TUMOR_SIZE_CLIN = new EodInput("size_clin");
        public static readonly EodInput TUMOR_SIZE_PATH = new EodInput("size_path");
        public static readonly EodInput TUMOR_SIZE_SUMMARY = new EodInput("size_summary");
        public static readonly EodInput RADIATION_SURG_SEQ = new EodInput("radiation_surg_seq");
        public static readonly EodInput SYSTEMIC_SURG_SEQ = new EodInput("systemic_surg_seq");
        public static readonly EodInput SS_2018 = new EodInput("ss2018");

        public static IEnumerable<EodInput> Values
        {
            get
            {
                yield return PRIMARY_SITE;
                yield return HISTOLOGY;
                yield return BEHAVIOR;
                yield return SEX;
                yield return AGE_AT_DX;
                yield return DISCRIMINATOR_1;
                yield return DISCRIMINATOR_2;
                yield return NODES_POS;
                yield return NODES_EXAM;
                yield return EOD_PRIMARY_TUMOR;
                yield return EOD_REGIONAL_NODES;
                yield return EOD_METS;
                yield return GRADE_CLIN;
                yield return GRADE_PATH;
                yield return GRADE_POST_THERAPY;
                yield return DX_YEAR;
                yield return TUMOR_SIZE_CLIN;
                yield return TUMOR_SIZE_PATH;
                yield return TUMOR_SIZE_SUMMARY;
                yield return RADIATION_SURG_SEQ;
                yield return SYSTEMIC_SURG_SEQ;
                yield return SS_2018;
            }
        }


        private readonly String _name;

        EodInput(String name)
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
    public class EodOutput
    {
        public static readonly EodOutput NAACCR_SCHEMA_ID = new EodOutput("naaccr_schema_id");
        public static readonly EodOutput AJCC_ID = new EodOutput("ajcc_id");
        public static readonly EodOutput DERIVED_VERSION = new EodOutput("derived_version");
        public static readonly EodOutput EOD_2018_T = new EodOutput("eod_2018_t");
        public static readonly EodOutput EOD_2018_N = new EodOutput("eod_2018_n");
        public static readonly EodOutput EOD_2018_M = new EodOutput("eod_2018_m");
        public static readonly EodOutput EOD_2018_STAGE_GROUP = new EodOutput("eod_2018_stage_group");
        public static readonly EodOutput SS_2018_DERIVED = new EodOutput("ss2018_derived");

        public static IEnumerable<EodOutput> Values
        {
            get
            {
                yield return NAACCR_SCHEMA_ID;
                yield return AJCC_ID;
                yield return DERIVED_VERSION;
                yield return EOD_2018_T;
                yield return EOD_2018_N;
                yield return EOD_2018_M;
                yield return EOD_2018_STAGE_GROUP;
                yield return SS_2018_DERIVED;
            }
        }

        private readonly String _name;

        EodOutput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }

    public class EodStagingData : StagingData
    {
        //========================================================================================================================
        // Default constructor
        //========================================================================================================================
        public EodStagingData() : base()
        {
        }

        // Construct with site and histology
        // @param site primary site
        // @param hist histology
        public EodStagingData(String site, String hist) : base(site, hist)
        {
        }

        // Construct with site, histology and discriminators
        // @param site primary site
        // @param hist histology
        // @param discriminator1 first discriminator
        public EodStagingData(String site, String hist, String discriminator1) : base(site, hist)
        {
            setInput(EodInput.DISCRIMINATOR_1, discriminator1);
        }

        // Construct with site, histology and discriminators
        // @param site primary site
        // @param hist histology
        // @param discriminator1 first discriminator
        // @param discriminator2 second discriminator
        public EodStagingData(String site, String hist, String discriminator1, String discriminator2) : base(site, hist)
        {
            setInput(EodInput.DISCRIMINATOR_1, discriminator1);
            setInput(EodInput.DISCRIMINATOR_2, discriminator2);
        }

        // Return the specified input value
        // @param key input key
        // @return input
        public String getInput(EodInput key)
        {
            return getInput(key.toString());
        }

        // Set the specified input value
        // @param key input key
        // @param value value
        public void setInput(EodInput key, String value)
        {
            setInput(key.toString(), value);
        }

        // Return the specified output value
        // @param key output key
        // @return output
        public String getOutput(EodOutput key)
        {
            return getOutput(key.toString());
        }


        // EodStagingInputBuilder builder
        public class EodStagingInputBuilder
        {
            private static EodStagingData _data;

            public EodStagingInputBuilder()
            {
                _data = new EodStagingData();
            }

            public EodStagingInputBuilder withDisciminator1(String discriminator1)
            {
                _data.setInput(EodInput.DISCRIMINATOR_1, discriminator1);
                return this;
            }

            public EodStagingInputBuilder withDisciminator2(String discriminator2)
            {
                _data.setInput(EodInput.DISCRIMINATOR_2, discriminator2);
                return this;
            }

            public EodStagingInputBuilder withInput(EodInput key, String value)
            {
                _data.setInput(key, value);
                return this;
            }

            public EodStagingData build()
            {
                return _data;
            }
        }
    }
    
}
