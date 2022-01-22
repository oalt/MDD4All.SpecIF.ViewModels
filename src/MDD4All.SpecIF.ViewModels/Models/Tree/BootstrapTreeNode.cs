using Newtonsoft.Json;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels.Tree
{
    public class BootstrapTreeNode
    {
        public BootstrapTreeNode()
        {
            Nodes = new List<BootstrapTreeNode>();
            Selectable = true;
        }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("selectedIcon")]
        public string SelectedIcon { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("backColor")]
        public string BackColor { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("selectable")]
        public bool Selectable { get; set; }

        [JsonProperty("nodes")]
        public List<BootstrapTreeNode> Nodes { get; set; }

    }
}
