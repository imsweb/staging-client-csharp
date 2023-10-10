/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Pediatric
{
    using TNMStagingCSharp.Src.Staging.Entities;

    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    //========================================================================================================================
    // input key definitions
    //========================================================================================================================
    public class PediatricInput
    {
        // input key definitions; note this only includes keys that are required for staging
        public static readonly PediatricInput PRIMARY_SITE = new PediatricInput("site");
        public static readonly PediatricInput HISTOLOGY = new PediatricInput("hist");
        public static readonly PediatricInput BEHAVIOR = new PediatricInput("behavior");
        public static readonly PediatricInput AGE_DX = new PediatricInput("age_dx");
        public static readonly PediatricInput YEAR_DX = new PediatricInput("year_dx");
        public static readonly PediatricInput PED_PRIMARY_TUMOR = new PediatricInput("ped_primary_tumor");
        public static readonly PediatricInput PED_REGIONAL_NODES = new PediatricInput("ped_regional_nodes");
        public static readonly PediatricInput PED_METS = new PediatricInput("ped_mets");
        public static readonly PediatricInput NODES_POS = new PediatricInput("nodes_pos");
        public static readonly PediatricInput S_CATEGORY_CLIN = new PediatricInput("s_category_clin");
        public static readonly PediatricInput S_CATEGORY_PATH = new PediatricInput("s_category_path");
        public static readonly PediatricInput SIZE_SUMMARY = new PediatricInput("size_summary");
        public static readonly PediatricInput B_SYMPTOMS = new PediatricInput("b_symptoms");
        public static readonly PediatricInput DERIVED_SUMMARY_GRADE = new PediatricInput("derived_summary_grade");
        public static readonly PediatricInput SURG_2023 = new PediatricInput("surg_2023");
        public static readonly PediatricInput SURGICAL_MARGINS = new PediatricInput("surgical_margins");

        public static IEnumerable<PediatricInput> Values
        {
            get
            {
                yield return PRIMARY_SITE;
                yield return HISTOLOGY;
                yield return BEHAVIOR;
                yield return AGE_DX;
                yield return YEAR_DX;
                yield return PED_PRIMARY_TUMOR;
                yield return PED_REGIONAL_NODES;
                yield return PED_METS;
                yield return NODES_POS;
                yield return S_CATEGORY_CLIN;
                yield return S_CATEGORY_PATH;
                yield return SIZE_SUMMARY;
                yield return B_SYMPTOMS;
                yield return DERIVED_SUMMARY_GRADE;
                yield return SURG_2023;
                yield return SURGICAL_MARGINS;
            }
        }

        private readonly String _name;

        PediatricInput(String name)
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
    public class PediatricOutput
    {
        public static readonly PediatricOutput PEDIATRIC_ID = new PediatricOutput("pediatric_id");
        public static readonly PediatricOutput TORONTO_VERSION_NUMBER = new PediatricOutput("toronto_version_number");
        public static readonly PediatricOutput PEDIATRIC_T = new PediatricOutput("pediatric_t");
        public static readonly PediatricOutput PEDIATRIC_N = new PediatricOutput("pediatric_n");
        public static readonly PediatricOutput PEDIATRIC_M = new PediatricOutput("pediatric_m");
        public static readonly PediatricOutput PEDIATRIC_GROUP = new PediatricOutput("pediatric_group");
        public static readonly PediatricOutput DERIVED_VERSION = new PediatricOutput("derived_version");

        public static IEnumerable<PediatricOutput> Values
        {
            get
            {
                yield return PEDIATRIC_ID;
                yield return TORONTO_VERSION_NUMBER;
                yield return PEDIATRIC_T;
                yield return PEDIATRIC_N;
                yield return PEDIATRIC_M;
                yield return PEDIATRIC_GROUP;
                yield return DERIVED_VERSION;
            }
        }

        private readonly String _name;

        PediatricOutput(String name)
        {
            this._name = name;
        }

        public String toString()
        {
            return _name;
        }
    }

    public class PediatricStagingData : StagingData
    {
        //========================================================================================================================
        // Default constructor
        //========================================================================================================================
        public PediatricStagingData() : base()
        {
        }

        // Construct with site and histology
        // @param site primary site
        // @param hist histology
        public PediatricStagingData(String site, String hist) : base(site, hist)
        {
        }

        // Construct with site, histology and age at diagnosis
        // @param site primary site
        // @param hist histology
        // @param ageAtDx age at diagnosis
        public PediatricStagingData(String site, String hist, String ageAtDx) : base(site, hist)
        {
            setInput(PediatricInput.AGE_DX, ageAtDx);
        }

        // Return the specified input value
        // @param key input key
        // @return input
        public String getInput(PediatricInput key)
        {
            return getInput(key.toString());
        }

        // Set the specified input value
        // @param key input key
        // @param value value
        public void setInput(PediatricInput key, String value)
        {
            setInput(key.toString(), value);
        }

        // Return the specified output value
        // @param key output key
        // @return output
        public String getOutput(PediatricOutput key)
        {
            return getOutput(key.toString());
        }


        // PediatricStagingInputBuilder builder
        public class PediatricStagingInputBuilder
        {
            private readonly PediatricStagingData _data = new PediatricStagingData();

            public PediatricStagingInputBuilder()
            {
                //_data = new PediatricStagingData();
            }

            public PediatricStagingInputBuilder withInput(PediatricInput key, String value)
            {
                _data.setInput(key, value);
                return this;
            }

            public PediatricStagingData build()
            {
                return _data;
            }
        }
    }

}
