using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Logger;

namespace Bot.Data.EfDatastores
{
    public class StoreCommuters : IStoreCommuters
    {
        public async Task<bool> AddCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters)
        {
            if (commuters == null || commuters.Count == 0)
                return false;

            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.AddingCommutersToEntityFrameworkSqlDb, commuters)
                .Debug();

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    using (var dbCtx = new DatabaseContext())
                    {
                        dbCtx.EfCommuters.AddRange(commuters.Select(x => (EfCommuter)x));
                        dbCtx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    new BotLogger<IList<Commuter>>(operationId, flowId,
                        EventCodes.ExceptionWhileAddingCommutersToEntityFrameworkSqlDb,
                        commuters, ex)
                    .Exception();
                    return false;
                }
                return true;
            }).Result;
            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.CommutersAddedToEntityFrameworkSqlDb, commuters)
                .Debug();
            return result;
        }

        public async Task<bool> UpdateCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters)
        {
            if (commuters == null || commuters.Count == 0)
            {
                new BotLogger<string>(operationId, flowId, EventCodes.InvalidArguments, string.Empty)
                {
                    Message = "List of commuters is " + (commuters == null ? "null" : "empty")
                }.Error();
                return false;
            }

            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.UpdatingCommutersToEntityFrameworkSqlDb, commuters)
                .Debug();

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    var commuterIds = commuters.Select(c => c.CommuterId).ToList();
                    using (var dbCtx = new DatabaseContext())
                    {
                        var efCommutersToUpdate =
                            dbCtx.EfCommuters.Where(ec => commuterIds.Any(c => ec.CommuterId == c)).ToList();

                        if (efCommutersToUpdate.Count != commuters.Count)
                        {
                            var commutersLogObject =
                                new Tuple<IList<EfCommuter>, IList<Commuter>>(efCommutersToUpdate, commuters);
                            new BotLogger<Tuple<IList<EfCommuter>, IList<Commuter>>>(operationId, flowId,
                                EventCodes.OneOrMoreCommuterDoesNotExist, commutersLogObject)
                            {
                                Message = "order of log object is EfCommuters : Commuters"
                            }.Error();
                            return false;
                        }

                        foreach (var efCommuter in efCommutersToUpdate)
                        {
                            var commuter = commuters.FirstOrDefault(x => x.CommuterId == efCommuter.CommuterId);
                            if (commuter == null)
                                return false;
                            efCommuter.CommuterName = commuter.CommuterName;
                            efCommuter.HomeCoordinate = (EfCoordinate)commuter.HomeCoordinate;
                            efCommuter.OfficeCoordinate = (EfCoordinate)commuter.OfficeCoordinate;
                            efCommuter.MediaId = commuter.MediaId;
                            efCommuter.Status = commuter.Status;
                        }

                        dbCtx.SaveChanges();
                        new BotLogger<IList<EfCommuter>>(operationId, flowId,
                                EventCodes.CommutersUpdatedToEntityFrameworkSqlDb, efCommutersToUpdate)
                            .Debug();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.ExceptionWhileUpdatingCommutersToEntityFrameworkSqlDb, commuters, ex)
                        .Exception();
                    return false;
                }
            }).Result;

            return result;
        }

        public async Task<List<Commuter>> GetCommuters(IList<Guid> commuterIds)
        {
            if (commuterIds == null || commuterIds.Count == 0)
                return new List<Commuter>();

            var result = new TaskFactory().StartNew<List<Commuter>>(() =>
            {
                using (var dbCtx = new DatabaseContext())
                {
                    var eftrips = dbCtx.EfCommuters.Where(x => commuterIds.Contains(x.CommuterId)).ToList();

                    return eftrips.Select(x => x.GetCommuter(Guid.NewGuid(), Guid.NewGuid())).ToList();
                }
            }).Result;
            return result;
        }

        public async Task<List<Commuter>> GetCommutersForMediaIds(List<string> mediaIds)
        {
            if (mediaIds == null || mediaIds.Count == 0)
                return new List<Commuter>();

            var result = new TaskFactory().StartNew<List<Commuter>>(() =>
            {
                using (var dbCtx = new DatabaseContext())
                {
                    var eftrips = dbCtx.EfCommuters.Where(x => mediaIds.Contains(x.MediaId)).ToList();

                    return eftrips.Select(x => x.GetCommuter(Guid.NewGuid(), Guid.NewGuid())).ToList();
                }
            }).Result;
            return result;
        }
    }
}
