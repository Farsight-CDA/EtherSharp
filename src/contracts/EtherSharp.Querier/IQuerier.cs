using EtherSharp.Contract;

namespace EtherSharp.Querier;

[AbiFile("iquerier.abi.json")]
internal partial interface IQuerier : IEVMContract;