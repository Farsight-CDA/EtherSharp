using EtherSharp.Types;

namespace EtherSharp.Tx;

public record StateAccess(
    Address Address,
    byte[][] StorageKeys
);
