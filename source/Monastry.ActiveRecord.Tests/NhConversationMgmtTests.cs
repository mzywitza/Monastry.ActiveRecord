using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Monastry.ActiveRecord;

namespace Monastry.ActiveRecord.Tests
{
	[TestFixture]
	public class NhConversationMgmtTests
	{
		[Test]
		public void NhScopeExists()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			INhScope scope = new NhScope(conv);
			Assert.That(scope.AssociatedConversation, Is.SameAs(conv));
		}

		[Test]
		public void CanUseNhScopeAsIScope()
		{
			var conv = MockRepository.GenerateStub<INhConversation>();
			INhScope scope = new NhScope(conv);
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
