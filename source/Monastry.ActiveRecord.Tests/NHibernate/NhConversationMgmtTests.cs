using NUnit.Framework;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.NHibernate
{
	[TestFixture]
	public class NhConversationMgmtTests
	{
		[Test]
		public void ConversationIsSet()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			INhScope scope = new NhScope(conv);
			Assert.That(scope.AssociatedConversation, Is.SameAs(conv));
		}

		[Test]
		public void CanUseNhScopeAsIScope()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			IScope scope = new NhScope(conv);
			Assert.That(scope.AssociatedConversation, Is.SameAs(conv));
		}

		[Test]
		public void CanQueryInvalidStatus()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			INhScope scope = new NhScope(conv);
			Assert.That(scope.IsValid, Is.True);
		}

		[Test]
		public void CanSetInvalidStatus()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			INhScope scope = new NhScope(conv);
			Assert.That(scope.IsValid, Is.True);
			scope.Invalidate();
			Assert.That(scope.IsValid, Is.False);
		}

	}
}
