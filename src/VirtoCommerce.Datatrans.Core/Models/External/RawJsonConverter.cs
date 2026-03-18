using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.Datatrans.Core.Models.External;

/// <summary>
/// Serializes string values as raw JSON (parsed and embedded) instead of as a JSON string.
/// Used for properties that may receive a JSON string but must be sent as an object/array in the payload.
/// </summary>
public class RawJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is string s && !string.IsNullOrEmpty(s))
        {
            var token = JToken.Parse(s);
            token.WriteTo(writer);
        }
        else if (value != null)
        {
            var token = JToken.FromObject(value, serializer);
            token.WriteTo(writer);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        return token.Type == JTokenType.Object || token.Type == JTokenType.Array ? token : token.ToString();
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(object);
}
