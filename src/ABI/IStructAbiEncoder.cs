﻿namespace EtherSharp.ABI;
public interface IStructAbiEncoder : IAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public new IStructAbiEncoder Struct(Func<IStructAbiEncoder, IStructAbiEncoder> func);

    public void WriteToParent(Span<byte> result, Span<byte> payload, uint payloadOffset);
}