using Newtonsoft.Json;

namespace VirtoCommerce.Datatrans.Core.Models.External;

public class DatatransCaptureRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string Refno { get; set; }

    /// <summary>
    /// Airline data. When this is a string containing JSON, it is serialized as a JSON object/array in the request body.
    /// https://api-reference.datatrans.ch/#tag/v1transactions/operation/settle
    /// </summary>
    [JsonConverter(typeof(RawJsonConverter))]
    public object AirlineData { get; set; }
}
