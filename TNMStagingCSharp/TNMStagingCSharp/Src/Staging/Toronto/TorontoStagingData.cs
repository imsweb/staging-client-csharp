/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Toronto
{
    using TNMStagingCSharp.Src.Staging.Entities;

    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    public class TorontoInput
    {
        // input key definitions; note this only includes keys that are required for staging
        public static readonly TorontoInput PRIMARY_SITE = new TorontoInput("site");
        public static readonly TorontoInput HISTOLOGY = new TorontoInput("hist");
        public static readonly TorontoInput BEHAVIOR = new TorontoInput("behavior");
        public static readonly TorontoInput AGE_DX = new TorontoInput("age_dx");
        public static readonly TorontoInput YEAR_DX = new TorontoInput("year_dx");
        public static readonly TorontoInput EOD_PRIMARY_TUMOR = new TorontoInput("eod_primary_tumor");
        public static readonly TorontoInput EOD_REGIONAL_NODES = new TorontoInput("eod_regional_nodes");
        public static readonly TorontoInput EOD_METS = new TorontoInput("eod_mets");
        public static readonly TorontoInput NODES_POS = new TorontoInput("nodes_pos");
        public static readonly TorontoInput GRADE_PATH = new TorontoInput("grade_path");
        public static readonly TorontoInput GRADE_CLIN = new TorontoInput("grade_clin");
        public static readonly TorontoInput GRADE_POST_THERAPY_CLIN = new TorontoInput("grade_post_therapy_clin");
        public static readonly TorontoInput GRADE_POST_THERAPY_PATH = new TorontoInput("grade_post_therapy_path");
        public static readonly TorontoInput SCHEMA_ID = new TorontoInput("schema_id");
        public static readonly TorontoInput SHIMADA_CLASSIFICATION = new TorontoInput("shimada_classification");
        public static readonly TorontoInput S_CATEGORY_CLIN = new TorontoInput("s_category_clin");
        public static readonly TorontoInput DNA_PLOIDY = new TorontoInput("dna_ploidy");
        public static readonly TorontoInput N_MYC_APMLIFICATION = new TorontoInput("n_myc_amplification");
        public static readonly TorontoInput S_CATEGORY_PATH = new TorontoInput("s_category_path");
        public static readonly TorontoInput SIZE_SUMMARY = new TorontoInput("size_summary");
        public static readonly TorontoInput INGRSS = new TorontoInput("ingrss");
        public static readonly TorontoInput B_SYMPTOMS = new TorontoInput("b_symptoms");
        public static readonly TorontoInput MONTH_DX = new TorontoInput("month_dx");
        public static readonly TorontoInput MONTH_BIRTH = new TorontoInput("month_birth");
        public static readonly TorontoInput CHROM_11Q_STATUS = new TorontoInput("chrom_11q_status");

        public static IEnumerable<TorontoInput> Values
        {
            get
            {
                yield return PRIMARY_SITE;
                yield return HISTOLOGY;
                yield return BEHAVIOR;
                yield return AGE_DX;
                yield return YEAR_DX;
                yield return EOD_PRIMARY_TUMOR;
                yield return EOD_REGIONAL_NODES;
                yield return EOD_METS;
                yield return NODES_POS;
                yield return GRADE_PATH;
                yield return GRADE_CLIN;
                yield return GRADE_POST_THERAPY_CLIN;
                yield return GRADE_POST_THERAPY_PATH;
                yield return SCHEMA_ID;
                yield return SHIMADA_CLASSIFICATION;
                yield return S_CATEGORY_CLIN;
                yield return DNA_PLOIDY;
                yield return N_MYC_APMLIFICATION;
                yield return S_CATEGORY_PATH;
                yield return SIZE_SUMMARY;
                yield return INGRSS;
                yield return B_SYMPTOMS;
                yield return MONTH_DX;
                yield return MONTH_BIRTH;
                yield return CHROM_11Q_STATUS;
            }
        }

        private readonly String _name;

        TorontoInput(String name)
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
    public class TorontoOutput
    {
        public static readonly TorontoOutput TORONTO_ID = new TorontoOutput("toronto_id");
        public static readonly TorontoOutput TORONTO_VERSION_NUMBER = new TorontoOutput("toronto_version_number");
        public static readonly TorontoOutput TORONTO_T = new TorontoOutput("toronto_t");
        public static readonly TorontoOutput TORONTO_N = new TorontoOutput("toronto_n");
        public static readonly TorontoOutput TORONTO_M = new TorontoOutput("toronto_m");
        public static readonly TorontoOutput TORONTO_GROUP = new TorontoOutput("toronto_group");
        public static readonly TorontoOutput TORONTO_GRADE = new TorontoOutput("toronto_grade");
        public static readonly TorontoOutput DERIVED_VERSION = new TorontoOutput("derived_version");
        public static readonly TorontoOutput DERIVED_RISK_LEVEL = new TorontoOutput("derived_risk_level");
        public static readonly TorontoOutput DERIVED_ANN_ARBOR_STAGE = new TorontoOutput("derived_ann_arbor_stage");

        public static IEnumerable<TorontoOutput> Values
        {
            get
            {
                yield return TORONTO_ID;
                yield return TORONTO_VERSION_NUMBER;
                yield return TORONTO_T;
                yield return TORONTO_N;
                yield return TORONTO_M;
                yield return TORONTO_GROUP;
                yield return TORONTO_GRADE;
                yield return DERIVED_VERSION;
                yield return DERIVED_RISK_LEVEL;
                yield return DERIVED_ANN_ARBOR_STAGE;
            }
        }

        private readonly String _name;

        TorontoOutput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }

    public class TorontoStagingData : StagingData
    {
        //========================================================================================================================
        // Default constructor
        //========================================================================================================================
        public TorontoStagingData() : base()
        {
        }

        // Construct with site and histology
        // @param site primary site
        // @param hist histology
        public TorontoStagingData(String site, String hist) : base(site, hist)
        {
        }

        // Construct with site, histology and age at diagnosis
        // @param site primary site
        // @param hist histology
        // @param ageAtDx age at diagnosis
        public TorontoStagingData(String site, String hist, String ageAtDx) : base(site, hist)
        {
            setInput(TorontoInput.AGE_DX, ageAtDx);
        }

        // Return the specified input value
        // @param key input key
        // @return input
        public String getInput(TorontoInput key)
        {
            return getInput(key.toString());
        }

        // Set the specified input value
        // @param key input key
        // @param value value
        public void setInput(TorontoInput key, String value)
        {
            setInput(key.toString(), value);
        }

        // Return the specified output value
        // @param key output key
        // @return output
        public String getOutput(TorontoOutput key)
        {
            return getOutput(key.toString());
        }


        // TorontoStagingInputBuilder builder
        public class TorontoStagingInputBuilder
        {
            private readonly TorontoStagingData _data = new TorontoStagingData();

            public TorontoStagingInputBuilder()
            {
                //_data = new TorontoStagingData();
            }

            public TorontoStagingInputBuilder withInput(TorontoInput key, String value)
            {
                _data.setInput(key, value);
                return this;
            }

            public TorontoStagingData build()
            {
                return _data;
            }
        }
    }

}
