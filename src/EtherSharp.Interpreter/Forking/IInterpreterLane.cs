using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking;

/// <summary>Executes sequential operations on one interpreter during a structured fork run.</summary>
public interface IInterpreterLane
{
    /// <summary>Executes a legacy transaction and retains its state changes.</summary>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Executes an EIP-1559 transaction and retains its state changes.</summary>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Executes a call and retains its state changes.</summary>
    public ValueTask<TxCallResult> ExecuteCallAsync(
        Address sender,
        ITxInput call,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Executes a call, retains its state changes, and returns its decoded value.</summary>
    public ValueTask<T> ExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Executes a call, retains its state changes, and returns its typed outcome.</summary>
    public ValueTask<CallResult<T>> SafeExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Simulates a legacy transaction and discards its state changes.</summary>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Simulates an EIP-1559 transaction and discards its state changes.</summary>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Simulates a call and discards its state changes.</summary>
    public ValueTask<TxCallResult> SimulateCallAsync(
        Address sender,
        ITxInput call,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Simulates a call, discards its state changes, and returns its decoded value.</summary>
    public ValueTask<T> SimulateCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    );

    /// <summary>Simulates a call, discards its state changes, and returns its typed outcome.</summary>
    public ValueTask<CallResult<T>> SafeSimulateCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    );
}
