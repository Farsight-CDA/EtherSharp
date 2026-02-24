using System.Text.Json.Serialization;

namespace EtherSharp.Types;

/// <summary>
/// Represents the EVM call operation type used in call traces.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<CallType>))]
public enum CallType
{
    /// <summary>
    /// Standard message call.
    /// </summary>
    Call,
    /// <summary>
    /// Read-only message call.
    /// </summary>
    StaticCall,
    /// <summary>
    /// Message call that executes in the caller context.
    /// </summary>
    DelegateCall,
    /// <summary>
    /// Contract creation via CREATE.
    /// </summary>
    Create,
    /// <summary>
    /// Contract creation via CREATE2.
    /// </summary>
    Create2
}
