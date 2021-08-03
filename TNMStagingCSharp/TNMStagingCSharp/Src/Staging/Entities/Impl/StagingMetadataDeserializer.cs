/*
 * Copyright (C) 2021 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    public class StagingMetadataDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StagingMetadata);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            StagingMetadata meta = new StagingMetadata();
            try
            {
                bool endObject = true;
                do
                {
                    endObject = true;
                    if (reader.TokenType == JsonToken.String)
                    {
                        meta.setName(reader.Value as string ?? string.Empty);
                        endObject = true;
                    }
                    else if (reader.TokenType == JsonToken.StartObject)
                    {
                        reader.Read();
                        endObject = false;
                    }
                    else if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string val = (reader.Value as string) ?? string.Empty;
                        if (val.Equals("name"))
                        {
                            meta.setName(reader.ReadAsString());
                        }
                        else if (val.Equals("start"))
                        {
                            meta.setStart((int)reader.ReadAsInt32());
                        }
                        else if (val.Equals("end"))
                        {
                            meta.setEnd((int)reader.ReadAsInt32());
                        }
                        reader.Read();
                        endObject = false;
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        //reader.Read();
                        endObject = true;
                    }
                } while (!endObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }

            meta.ComputeHashCode();
            return meta;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}

/*
public class StagingMetadataDeserializer extends StdDeserializer<StagingMetadata> {

    public StagingMetadataDeserializer() {
        this(null);
    }

    protected StagingMetadataDeserializer(Class<?> vc) {
        super(vc);
    }

    @Override
    public StagingMetadata deserialize(JsonParser jp, DeserializationContext ctxt) throws IOException, JsonProcessingException {
        JsonNode node = jp.getCodec().readTree(jp);

        if (node.isNull())
            return null;

        if (node.isObject()) {
            StagingMetadata meta = new StagingMetadata();

            if (node.get("name") != null)
                meta.setName(node.get("name").asText());
            if (node.get("start") != null)
                meta.setStart(node.get("start").asInt());
            if (node.get("end") != null)
                meta.setEnd(node.get("end").asInt());

            return meta;
        }

        return new StagingMetadata(node.asText());
    }
}

*/

