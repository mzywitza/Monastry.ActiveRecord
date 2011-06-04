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
	public class ScopeTests
	{
		[Test]
		public void ScopeExists()
		{
			var conv = MockRepository.GenerateStub<IConversation>();
			IScope<IConversation> scope = new Scope<IConversation>(conv);
			Assert.That(scope.AssociatedConversation, Is.SameAs(conv));
		}

		[Test]
		public void CanUseNhScopeAsIScope()
		{
			var conv = MockRepository.GenerateStub<IConversation>();
			IScope<IConversation> scope = new Scope<IConversation>(conv);
			Assert.That(scope.AssociatedConversation, Is.SameAs(conv));
		}

		[Test]
		public void CanQueryInvalidStatus()
		{
			var conv = MockRepository.GenerateStub<IConversation>();
			IScope<IConversation> scope = new Scope<IConversation>(conv);
			Assert.That(scope.IsValid, Is.True);
		}

		[Test]
		public void CanSetInvalidStatus()
		{
			var conv = MockRepository.GenerateStub<IConversation>();
			IScope<IConversation> scope = new Scope<IConversation>(conv);
			Assert.That(scope.IsValid, Is.True);
			scope.Invalidate();
			Assert.That(scope.IsValid, Is.False);
		}

	}
}
