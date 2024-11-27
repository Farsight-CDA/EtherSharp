﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class EventAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;
}
