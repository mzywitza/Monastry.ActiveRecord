﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
    public class NhConversationContext : ConversationContext, INhConversationContext
    {
        public new INhConversation CurrentConversation
        {
            get { return (INhConversation)base.CurrentConversation; }
        }

        public new INhScope CurrentScope
        {
            get { return (INhScope)base.CurrentScope; }
        }
    }
}
