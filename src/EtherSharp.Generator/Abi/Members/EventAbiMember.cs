using EtherSharp.Generator.Abi.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;
public class EventAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("anonymous")]
    public bool IsAnonymous { get; set; }

    [JsonRequired]
    public AbiEventParameter[] Inputs { get; set; } = null!;
}
