using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data.Models
{
    public class CommuterConversationReference : ModelBase
    {
        public Guid CommuterId { get; set; }
        public string ConversationReference { get; set; }

        public CommuterConversationReference(Guid operationId) 
            : base(operationId)
        {
        }
    }
}
