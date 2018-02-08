using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.DecisionEngine;



namespace TNMStagingCSharp.Src.Tools
{

    class TestingTools
    {
    }

    public class DebugSettings
    {
        public const bool DEBUG_LOADED_TABLES = false;
        public const bool DEBUG_LOADED_SCHEMAS = false;

        public const bool RUN_LARGE_TNM_TESTS = true;
        public const bool RUN_LARGE_CS_TESTS = true;

        public const bool RUN_HUGE_GOOD_TNM_TESTS = false;
        public const bool RUN_HUGE_BAD_TNM_TESTS = false;
        public const bool USE_LOCAL_ZIP_FILE_FOR_TNM_TESTS = true;

        public const bool RUN_HUGE_GOOD_CS_TESTS = false;
        public const bool RUN_HUGE_BAD_CS_TESTS = false;

    }


    public class Strings
    {
        public const String EOL = "\r\n";
    }

    public class CustomHashSetConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<HashSet<T>>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class CustomListConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<List<T>>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class CustomDictionaryConverter<T, V> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<T, V>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<Dictionary<T, V>>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }


    public class CustomListConverter_StagingKeyValue_IKeyValue : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<IKeyValue>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IKeyValue> lstU = new List<IKeyValue>();
            List<StagingKeyValue> lstT = serializer.Deserialize<List<StagingKeyValue>>(reader);
            foreach (StagingKeyValue objT in lstT)
            {
                lstU.Add(objT);
            }
            return lstU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

   
    public class CustomListConverter_StagingTablePath_ITablePath : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<ITablePath>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<ITablePath> lstU = new List<ITablePath>();
            List<StagingTablePath> lstT = serializer.Deserialize<List<StagingTablePath>>(reader);
            foreach (StagingTablePath objT in lstT)
            {
                lstU.Add(objT);
            }
            return lstU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class CustomListConverter_StagingMapping_IMapping : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<IMapping>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IMapping> lstU = new List<IMapping>();
            List<StagingMapping> lstT = serializer.Deserialize<List<StagingMapping>>(reader);
            foreach (StagingMapping objT in lstT)
            {
                lstU.Add(objT);
            }
            return lstU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class CustomListConverter_StagingColumnDefinition_IColumnDefinition : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<IColumnDefinition>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IColumnDefinition> lstU = new List<IColumnDefinition>();
            List<StagingColumnDefinition> lstT = serializer.Deserialize<List<StagingColumnDefinition>>(reader);
            foreach (StagingColumnDefinition objT in lstT)
            {
                lstU.Add(objT);
            }
            return lstU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class CustomHashSetConverter_StagingKeyValue_IKeyValue : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<IKeyValue>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<IKeyValue> hashU = new HashSet<IKeyValue>();
            HashSet<StagingKeyValue> hashT = serializer.Deserialize<HashSet<StagingKeyValue>>(reader);
            foreach (StagingKeyValue objT in hashT)
            {
                hashU.Add(objT);
            }
            return hashU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    
    public class CustomHashSetConverter_StagingKeyMapping_IKeyMapping : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<IKeyMapping>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HashSet<IKeyMapping> hashU = new HashSet<IKeyMapping>();
            HashSet<StagingKeyMapping> hashT = serializer.Deserialize<HashSet<StagingKeyMapping>>(reader);
            foreach (StagingKeyMapping objT in hashT)
            {
                hashU.Add(objT);
            }
            return hashU;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public class DebugStringUtils
    {
        public static bool EnableDebugging = false;

        public static String GetStringDictionary(Dictionary<string, string> d)
        {
            String sRetval = "NULL";
            if (d != null)
            {
                sRetval = "";
                foreach (KeyValuePair<String, String> entry in d)
                {
                    sRetval += "<" + entry.Key + "," + entry.Value + ">, ";
                }
            }
            return sRetval;
        }
        public static String GetStringHashSet(HashSet<string> h)
        {
            String sRetval = "NULL";
            if (h != null)
            {
                sRetval = "";
                foreach (String s in h)
                {
                    sRetval += s + ", ";
                }
            }
            return sRetval;
        }
    }


}
