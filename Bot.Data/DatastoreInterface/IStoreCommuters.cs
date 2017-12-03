using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreCommuters
    {
        Task<bool> AddCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters);
        Task<bool> UpdateCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters);
        Task<List<Commuter>> GetCommuters(IList<Guid> commuterIds);
        Task<List<Commuter>> GetCommutersForMediaIds(List<string> list);
    }
}
