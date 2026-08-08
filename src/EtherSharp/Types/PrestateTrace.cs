namespace EtherSharp.Types;

/// <summary>
/// Maps every account required to execute a traced call to its captured prestate.
/// </summary>
public sealed class PrestateTrace : Dictionary<Address, PrestateAccount>;
