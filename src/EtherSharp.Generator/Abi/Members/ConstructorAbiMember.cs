﻿using EtherSharp.Generator.Abi.Parameters;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;
public class ConstructorAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }

    [JsonRequired]
    public AbiInputParameter[] Inputs { get; set; } = null!;
}