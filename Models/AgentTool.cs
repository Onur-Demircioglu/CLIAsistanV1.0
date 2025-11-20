using System;
using System.Collections.Generic;

namespace CliAsistan.Models
{
    public class AgentTool
    {
        public string Ad { get; set; } = null!;
        public string Aciklama { get; set; } = null!;
        public Func<Dictionary<string, string>, string>? Fonksiyon { get; set; }
    }
}
