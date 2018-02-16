using System;

namespace Bot.Data.Models
{
    public class ModelBase
    {
        public Guid OperationId { get; set; }

        public ModelBase(Guid operationId)
        {
            OperationId = operationId;
        }
    }
}
