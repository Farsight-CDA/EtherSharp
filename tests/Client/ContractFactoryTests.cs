using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Contract;
using EtherSharp.ERC.ERC20;
using EtherSharp.Types;

namespace EtherSharp.Tests.Client;

public class ContractFactoryTests
{
    [Fact]
    public void Should_Create_Generated_Contract_Without_Manual_Registration()
    {
        var factory = new ContractFactory(null!);

        IEVMContract contract = factory.Create<IERC20>(Address.Zero);

        Assert.IsType<IERC20>(contract, exactMatch: false);
    }
}
