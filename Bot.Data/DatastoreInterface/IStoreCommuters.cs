using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreCommuters
    {
        Task<bool> AddCommutersAsync(IList<Commuter> commuters);
        Task<bool> UpdateCommutersAsync(IList<Commuter> commuters);
        Task<List<Commuter>> GetCommuters(IList<Guid> commuterIds);
        Task<List<Commuter>> GetCommutersForMediaIds(List<string> list);
    }
}
