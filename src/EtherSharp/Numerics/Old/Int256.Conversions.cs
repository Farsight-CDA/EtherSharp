// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#pragma warning disable CS1591

using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Numerics.Old;

public readonly partial struct Int256
{
    public override string ToString()
        => ((BigInteger) this).ToString();
    public string ToString(string? format)
        => ((BigInteger) this).ToString(format);
    public string ToString(IFormatProvider? provider)
        => ((BigInteger) this).ToString(provider);

}
