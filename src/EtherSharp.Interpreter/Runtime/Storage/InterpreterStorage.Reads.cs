using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed partial class InterpreterStorage
{
    public ValueTask<TValue> GetAsync<TValue>(StateRequest<TValue> request)
        => TryResolve(request, out var value)
            ? ValueTask.FromResult(value)
            : host.GetAsync(request.CreateHostRequest());

    public async ValueTask<(T1 First, T2 Second)> GetAsync<T1, T2>(
        StateRequest<T1> first,
        StateRequest<T2> second
    )
    {
        bool hasFirst = TryResolve(first, out var firstValue);
        bool hasSecond = TryResolve(second, out var secondValue);
        return (hasFirst, hasSecond) switch
        {
            (true, true) => (firstValue, secondValue),
            (false, false) => await host.GetAsync(
                first.CreateHostRequest(),
                second.CreateHostRequest()
            ),
            (true, false) => (firstValue, await host.GetAsync(second.CreateHostRequest())),
            (false, true) => (await host.GetAsync(first.CreateHostRequest()), secondValue)
        };
    }

    public async ValueTask<(T1 First, T2 Second, T3 Third)> GetAsync<T1, T2, T3>(
        StateRequest<T1> first,
        StateRequest<T2> second,
        StateRequest<T3> third
    )
    {
        bool hasFirst = TryResolve(first, out var firstValue);
        bool hasSecond = TryResolve(second, out var secondValue);
        bool hasThird = TryResolve(third, out var thirdValue);
        switch((hasFirst, hasSecond, hasThird))
        {
            case (true, true, true):
                return (firstValue, secondValue, thirdValue);
            case (false, false, false):
                return await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest(),
                    third.CreateHostRequest()
                );
            case (false, false, true):
            {
                var (firstResult, secondResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest()
                );
                return (firstResult, secondResult, thirdValue);
            }
            case (false, true, false):
            {
                var (firstResult, thirdResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    third.CreateHostRequest()
                );
                return (firstResult, secondValue, thirdResult);
            }
            case (true, false, false):
            {
                var (secondResult, thirdResult) = await host.GetAsync(
                    second.CreateHostRequest(),
                    third.CreateHostRequest()
                );
                return (firstValue, secondResult, thirdResult);
            }
            case (false, true, true):
                firstValue = await host.GetAsync(first.CreateHostRequest());
                return (firstValue, secondValue, thirdValue);
            case (true, false, true):
                secondValue = await host.GetAsync(second.CreateHostRequest());
                return (firstValue, secondValue, thirdValue);
            default:
                thirdValue = await host.GetAsync(third.CreateHostRequest());
                return (firstValue, secondValue, thirdValue);
        }
    }

    public async ValueTask<(T1 First, T2 Second, T3 Third, T4 Fourth)> GetAsync<T1, T2, T3, T4>(
        StateRequest<T1> first,
        StateRequest<T2> second,
        StateRequest<T3> third,
        StateRequest<T4> fourth
    )
    {
        bool hasFirst = TryResolve(first, out var firstValue);
        bool hasSecond = TryResolve(second, out var secondValue);
        bool hasThird = TryResolve(third, out var thirdValue);
        bool hasFourth = TryResolve(fourth, out var fourthValue);
        switch((hasFirst, hasSecond, hasThird, hasFourth))
        {
            case (true, true, true, true):
                return (firstValue, secondValue, thirdValue, fourthValue);
            case (false, false, false, false):
                return await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest(),
                    third.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
            case (false, false, false, true):
            {
                var (firstResult, secondResult, thirdResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest(),
                    third.CreateHostRequest()
                );
                return (firstResult, secondResult, thirdResult, fourthValue);
            }
            case (false, false, true, false):
            {
                var (firstResult, secondResult, fourthResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstResult, secondResult, thirdValue, fourthResult);
            }
            case (false, true, false, false):
            {
                var (firstResult, thirdResult, fourthResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    third.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstResult, secondValue, thirdResult, fourthResult);
            }
            case (true, false, false, false):
            {
                var (secondResult, thirdResult, fourthResult) = await host.GetAsync(
                    second.CreateHostRequest(),
                    third.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstValue, secondResult, thirdResult, fourthResult);
            }
            case (false, false, true, true):
            {
                var (firstResult, secondResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    second.CreateHostRequest()
                );
                return (firstResult, secondResult, thirdValue, fourthValue);
            }
            case (false, true, false, true):
            {
                var (firstResult, thirdResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    third.CreateHostRequest()
                );
                return (firstResult, secondValue, thirdResult, fourthValue);
            }
            case (false, true, true, false):
            {
                var (firstResult, fourthResult) = await host.GetAsync(
                    first.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstResult, secondValue, thirdValue, fourthResult);
            }
            case (true, false, false, true):
            {
                var (secondResult, thirdResult) = await host.GetAsync(
                    second.CreateHostRequest(),
                    third.CreateHostRequest()
                );
                return (firstValue, secondResult, thirdResult, fourthValue);
            }
            case (true, false, true, false):
            {
                var (secondResult, fourthResult) = await host.GetAsync(
                    second.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstValue, secondResult, thirdValue, fourthResult);
            }
            case (true, true, false, false):
            {
                var (thirdResult, fourthResult) = await host.GetAsync(
                    third.CreateHostRequest(),
                    fourth.CreateHostRequest()
                );
                return (firstValue, secondValue, thirdResult, fourthResult);
            }
            case (false, true, true, true):
                firstValue = await host.GetAsync(first.CreateHostRequest());
                return (firstValue, secondValue, thirdValue, fourthValue);
            case (true, false, true, true):
                secondValue = await host.GetAsync(second.CreateHostRequest());
                return (firstValue, secondValue, thirdValue, fourthValue);
            case (true, true, false, true):
                thirdValue = await host.GetAsync(third.CreateHostRequest());
                return (firstValue, secondValue, thirdValue, fourthValue);
            default:
                fourthValue = await host.GetAsync(fourth.CreateHostRequest());
                return (firstValue, secondValue, thirdValue, fourthValue);
        }
    }

    private bool TryResolve<TValue>(StateRequest<TValue> request, out TValue value)
        => request.TryResolve(out value)
            || GetAccountStorage(request.Address).TryGetLocal(request, out value);

    public async ValueTask<Bytes32> GetExtCodeHashAsync(Address address)
    {
        bool hasLocalPresence = GetAccountStorage(address).TryGetLocalPresence(out bool isPresent);
        if(hasLocalPresence && !isPresent)
        {
            return Bytes32.Zero;
        }

        var codeHash = await GetAsync(StateRequest.CodeHash(address));
        if(!hasLocalPresence)
        {
            return codeHash ?? Bytes32.Zero;
        }

        var effectiveCodeHash = codeHash ?? Bytes32.EmptyCodeHash;
        if(effectiveCodeHash != Bytes32.EmptyCodeHash)
        {
            return effectiveCodeHash;
        }

        var (nonce, balance) = await GetAsync(
            StateRequest.Nonce(address),
            StateRequest.Balance(address)
        );
        return nonce == 0 && balance.IsZero
            ? Bytes32.Zero
            : effectiveCodeHash;
    }
}
