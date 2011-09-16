using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.Model
{
	public interface IMockQuery : INhQuery<int>
	{ 
	}

	public class MockQuery : NhQueryBase<int>, IMockQuery
	{
		public MockQuery()
		{
			ISession session = MockRepository.GenerateStub<ISession>();
			INhConversation conversation = MockRepository.GenerateStub<INhConversation>();
			INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
			context.Stub(c => c.CurrentConversation).Return(conversation);
			conversation.Stub(c => c.Execute(Arg<Action<ISession>>.Is.Anything)).WhenCalled(i => ((Action<ISession>)i.Arguments[0]).Invoke(session));
			ConversationContext = context;
		}

		public bool ExecuteCalled { get; set; }
		public int Result { get; set; }

		public override int Execute(ISession session)
		{
			ExecuteCalled = true;
			return Result;
		}
	}
}
