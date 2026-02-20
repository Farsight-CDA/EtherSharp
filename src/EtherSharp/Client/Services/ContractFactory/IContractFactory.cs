using System.Reflection;

namespace EtherSharp.Client.Services.ContractFactory;

public interface IContractFactory
{
    public void AddContractTypesFromAssembly(Assembly assembly);
}
