using Newtonsoft.Json;
using System;

namespace MDD4All.SpecIF.ViewModels.Models.JsTree
{
    public class JsTreeNode
    {
        [JsonProperty("id")]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("parent")]
        public string Parent { get; set; } = "#";

        [JsonProperty("text")]
        public string Text { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";
    }
}
