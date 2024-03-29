﻿using Newtonsoft.Json;

namespace subtree;

public record Contentavailability
{
    [JsonProperty(Order = 2)]
    public int availableCount { get; set; }
    [JsonProperty(Order = 1)]
    public int? bitstream { get; set; }
    [JsonProperty(Order = 3)]
    public int? constant { get; set; }
}
