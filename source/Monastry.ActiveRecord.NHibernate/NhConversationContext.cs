using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.NHibernate
{
    public class NhConversationContext : ConversationContext<INhConversation, INhScope>, INhConversationContext
    {
    }
}
