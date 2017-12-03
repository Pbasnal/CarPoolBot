using System;

namespace Bot.Data.Models
{
    public class ModelBase
    {
        public Guid OperationId { get; set; }
        public Guid FlowId { get; set; }

        public ModelBase(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            FlowId = flowId;
        }
    }
}
