using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Transactions;
using Orleans.Transactions.Abstractions;

namespace Grains
{
    public interface IGrain1 : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Execute();
    }

    public class Grain1 : Grain, IGrain1
    {
        public async Task Execute()
        {
            await GrainFactory.GetGrain<IGrain2>(1).Execute();
            await GrainFactory.GetGrain<IGrain2>(1).Execute();
        }
    }

    public interface IGrain2 : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        Task Execute();
    }

    public class Grain2 : Grain, IGrain2
    {
        private readonly ITransactionalState<Grain2State> _state;

        public Grain2([TransactionalState("grain2")] ITransactionalState<Grain2State> state)
        {
            _state = state;
        }

        public Task Execute()
        {
            return _state.PerformUpdate(s => s.Counter++);
        }
    }

    public class Grain2State
    {
        public int Counter { get; set; }
    }
}