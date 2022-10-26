﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eindwerkstuk.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Saved
    {
        [JsonProperty("RecipeId")]
        public int RecipeId { get; set; }
    }
}
