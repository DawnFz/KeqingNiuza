using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeqingNiuza.Wish
{
    class GachaTypeJsonConverter : JsonConverter<WishType>
    {
        public override WishType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (WishType)int.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, WishType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(((int)value).ToString());
        }
    }

    class ItemIdJsonConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var src = reader.GetString();
            if (src == "")
            {
                return 0;
            }
            else
            {
                return int.Parse(src);
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            if (value == 0)
            {
                writer.WriteStringValue("");
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }

        }
    }

    class TimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
            //return DateTime.ParseExact(reader.GetString(), @"yyyy'-'MM'-'dd hh':'mm':'ss", null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"yyyy'-'MM'-'dd hh':'mm':'ss"));
        }
    }
}
