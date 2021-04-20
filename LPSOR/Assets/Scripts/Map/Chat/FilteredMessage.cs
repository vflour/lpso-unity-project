using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Game.Map
{
    public class FilteredMessage
    {
        public string message;
        public string[] invalidWords;
    }

    public enum FilterType{None,Whitelist,Blacklist}
    [Serializable]
    public class ServerFilter
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterType filterType;
        public string[] filterWords;
        public int maxLength;
    }
}