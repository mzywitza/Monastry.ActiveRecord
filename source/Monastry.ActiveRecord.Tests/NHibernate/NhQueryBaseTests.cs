using System;
using Monastry.ActiveRecord.Tests.Model;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.NHibernate
{
	[TestFixture]
	public class NhQueryBaseTests
	{

		[Test]
		public void BaseRequiresConversationContext()
		{
			var q = new MockQuery();
			q.ConversationContext = null;
			var e = Assert.Throws<InvalidOperationException>(() => q.Execute());
			Assert.That(e.Message, Contains.Substring("ConversationContext"));
			Assert.That(e.Message, Contains.Substring("register"));
		}

		[Test]
		public void BaseCallsExecuteWithSession()
		{
			var q = new MockQuery();
			q.Execute();
			Assert.That(((MockQuery)q).ExecuteCalled, Is.True);
		}

	}
}
