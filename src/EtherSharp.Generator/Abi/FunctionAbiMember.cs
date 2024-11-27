﻿using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class FunctionAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    [JsonRequired]
    public StateMutability StateMutability { get; set; }

    [JsonRequired]
    public AbiValue[] Inputs { get; set; } = null!;

    [JsonRequired]
    public AbiValue[] Outputs { get; set; } = null!;
}
