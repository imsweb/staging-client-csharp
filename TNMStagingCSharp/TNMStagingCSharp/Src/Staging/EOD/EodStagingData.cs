/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.EOD
{
    using TNMStagingCSharp.Src.Staging.Entities;

    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    public class EodInput
    {
        // input key definitions; note this only includes keys that are required for staging
        public static readonly EodInput PRIMARY_SITE = new EodInput("site");
        public static readonly EodInput HISTOLOGY = new EodInput("hist");
        public static readonly EodInput BEHAVIOR = new EodInput("behavior");
        public static readonly EodInput SEX = new EodInput("sex");
        public static readonly EodInput AGE_AT_DX = new EodInput("age_dx");
        public static readonly EodInput DISCRIMINATOR_1 = new EodInput("discriminator_1");
        public static readonly EodInput DISCRIMINATOR_2 = new EodInput("discriminator_2");
        public static readonly EodInput YEAR_DX = new EodInput("year_dx");
        public static readonly EodInput NODES_POS = new EodInput("nodes_pos");
        public static readonly EodInput EOD_PRIMARY_TUMOR = new EodInput("eod_primary_tumor");
        public static readonly EodInput EOD_REGIONAL_NODES = new EodInput("eod_regional_nodes");
        public static readonly EodInput EOD_METS = new EodInput("eod_mets");
        public static readonly EodInput GRADE_CLIN = new EodInput("grade_clin");
        public static readonly EodInput GRADE_PATH = new EodInput("grade_path");
        public static readonly EodInput GRADE_POST_THERAPY_CLIN = new EodInput("grade_post_therapy_clin");
        public static readonly EodInput GRADE_POST_THERAPY_PATH = new EodInput("grade_post_therapy_path");
        public static readonly EodInput DX_YEAR = new EodInput("year_dx");
        public static readonly EodInput TUMOR_SIZE_SUMMARY = new EodInput("size_summary");
        public static readonly EodInput RADIATION_SURG_SEQ = new EodInput("radiation_surg_seq");
        public static readonly EodInput SYSTEMIC_SURG_SEQ = new EodInput("systemic_surg_seq");
        public static readonly EodInput BRESLOW_THINKNESS = new EodInput("breslow_thickness");
        public static readonly EodInput EOD_PROSTATE_PATH_EXTENSION = new EodInput("eod_prostate_path_extension");
        public static readonly EodInput ER = new EodInput("er");
        public static readonly EodInput ESOPH_TUMOR_EPICENTER = new EodInput("esoph_tumor_epicenter");
        public static readonly EodInput GESTATIONAL_PROG_INDEX = new EodInput("gestational_prog_index");
        public static readonly EodInput HER2_SUMMARY = new EodInput("her2_summary");
        public static readonly EodInput LDH_LEVEL = new EodInput("ldh_level");
        public static readonly EodInput LN_POS_AXILLARY_LEVEL_1_2 = new EodInput("ln_pos_axillary_level_1_2");
        public static readonly EodInput LN_SIZE_OF_METS = new EodInput("ln_size_of_mets");
        public static readonly EodInput MEASURED_BASAL_DIAMETER = new EodInput("measured_basal_diameter");
        public static readonly EodInput MEASURED_THICKNESS = new EodInput("measured_thickness");
        public static readonly EodInput ONCOTYPE_DX_SCORE = new EodInput("oncotype_dx_score");
        public static readonly EodInput PERIPHERAL_BLOOD_INVOLV = new EodInput("peripheral_blood_involv");
        public static readonly EodInput PERITONEAL_CYTOLOGY = new EodInput("peritoneal_cytology");
        public static readonly EodInput PR = new EodInput("pr");
        public static readonly EodInput PSA = new EodInput("psa");
        public static readonly EodInput S_CATEGORY_CLIN = new EodInput("s_category_clin");
        public static readonly EodInput S_CATEGORY_PATH = new EodInput("s_category_path");
        public static readonly EodInput ULCERATION = new EodInput("ulceration");
        public static readonly EodInput THROMBOCYTOPENIA = new EodInput("thrombocytopenia");
        public static readonly EodInput ORGANOMEGALY = new EodInput("organomegaly");
        public static readonly EodInput ADENOPATHY = new EodInput("adenopathy");
        public static readonly EodInput ANEMIA = new EodInput("anemia");
        public static readonly EodInput LYMPHOCYTOSIS = new EodInput("lymphocytosis");

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
                yield return YEAR_DX;
                yield return NODES_POS;
                yield return EOD_PRIMARY_TUMOR;
                yield return EOD_REGIONAL_NODES;
                yield return EOD_METS;
                yield return GRADE_CLIN;
                yield return GRADE_PATH;
                yield return GRADE_POST_THERAPY_CLIN;
                yield return GRADE_POST_THERAPY_PATH;
                yield return DX_YEAR;
                yield return TUMOR_SIZE_SUMMARY;
                yield return RADIATION_SURG_SEQ;
                yield return SYSTEMIC_SURG_SEQ;
                yield return BRESLOW_THINKNESS;
                yield return EOD_PROSTATE_PATH_EXTENSION;
                yield return ER;
                yield return ESOPH_TUMOR_EPICENTER;
                yield return GESTATIONAL_PROG_INDEX;
                yield return HER2_SUMMARY;
                yield return LDH_LEVEL;
                yield return LN_POS_AXILLARY_LEVEL_1_2;
                yield return LN_SIZE_OF_METS;
                yield return MEASURED_BASAL_DIAMETER;
                yield return MEASURED_THICKNESS;
                yield return ONCOTYPE_DX_SCORE;
                yield return PERIPHERAL_BLOOD_INVOLV;
                yield return PERITONEAL_CYTOLOGY;
                yield return PR;
                yield return PSA;
                yield return S_CATEGORY_CLIN;
                yield return S_CATEGORY_PATH;
                yield return ULCERATION;
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
        public static readonly EodOutput AJCC_VERSION_NUMBER = new EodOutput("ajcc_version_number");
        public static readonly EodOutput DERIVED_VERSION = new EodOutput("derived_version");
        public static readonly EodOutput EOD_2018_T = new EodOutput("eod_2018_t");
        public static readonly EodOutput EOD_2018_N = new EodOutput("eod_2018_n");
        public static readonly EodOutput EOD_2018_M = new EodOutput("eod_2018_m");
        public static readonly EodOutput EOD_2018_STAGE_GROUP = new EodOutput("eod_2018_stage_group");
        public static readonly EodOutput SS_2018_DERIVED = new EodOutput("ss2018_derived");
        public static readonly EodOutput DERIVED_RAI_STAGE = new EodOutput("derived_rai_stage");

        public static IEnumerable<EodOutput> Values
        {
            get
            {
                yield return NAACCR_SCHEMA_ID;
                yield return AJCC_ID;
                yield return AJCC_VERSION_NUMBER;
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
            private readonly EodStagingData _data = new EodStagingData();

            public EodStagingInputBuilder()
            {
                //_data = new EodStagingData();
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
