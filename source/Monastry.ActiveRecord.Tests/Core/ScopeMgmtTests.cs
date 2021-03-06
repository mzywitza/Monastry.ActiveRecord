﻿using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.Core
{
	[TestFixture]
	public class ScopeMgmtTests
	{
		[Test]
		public void CanAccessRegisteredScope()
		{
			var scope = MockRepository.GenerateStub<IScope>();
			scope.Stub(s => s.IsValid).Return(true);
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());
			var cc = new ConversationContext();

			cc.RegisterScope(scope);
			Assert.That(cc.CurrentScope, Is.SameAs(scope));
		}

		[Test]
		public void CanReleaseRegisteredScope()
		{
			var scope = MockRepository.GenerateStub<IScope>();
			scope.Stub(s => s.IsValid).Return(true);
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());
			var cc = new ConversationContext();

			cc.RegisterScope(scope);
			cc.ReleaseScope(scope);

			Assert.That(cc.CurrentScope, Is.Null);
		}

		[Test]
		public void ThrowsOnRegisteringNull()
		{
			var cc = new ConversationContext();
			var e = Assert.Throws<ArgumentNullException>(() => cc.RegisterScope(null));
			Assert.That(e.ParamName, Is.EqualTo("scope"));
		}

		[Test]
		public void ThrowsOnReleasingNull()
		{
			var cc = new ConversationContext();
			var e = Assert.Throws<ArgumentNullException>(() => cc.ReleaseScope(null));
			Assert.That(e.ParamName, Is.EqualTo("scope"));
		}

		[Test]
		public void ThrowsOnRegisteringInvalidScope()
		{
			var scope = MockRepository.GenerateStub<IScope>();
			var cc = new ConversationContext();

			scope.Stub(s => s.IsValid).Return(false);
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());

			var e = Assert.Throws<InvalidOperationException>(() => cc.RegisterScope(scope));
			Assert.That(e.Message, Contains.Substring("invalid"));
			Assert.That(e.Message, Contains.Substring("scope"));
			Assert.That(e.Message, Contains.Substring("internal"));
			Assert.That(e.Message, Contains.Substring("dispose"));
			Assert.That(e.Message, Contains.Substring("conversation"));
		}

		[Test]
		public void ThrowsOnAccessingInvalidScope()
		{
			var scope = MockRepository.GenerateMock<IScope>();
			var valid = true;
			scope.Stub(s => s.IsValid).Do((Func<bool>)(() => valid));
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());

			var cc = new ConversationContext();

			cc.RegisterScope(scope);
			valid = false;

			IScope x;
			var e = Assert.Throws<InvalidOperationException>(() => x = cc.CurrentScope);
			Assert.That(e.Message, Contains.Substring("invalid"));
			Assert.That(e.Message, Contains.Substring("scope"));
			Assert.That(e.Message, Contains.Substring("internal"));
			Assert.That(e.Message, Contains.Substring("dispose"));
			Assert.That(e.Message, Contains.Substring("conversation"));
		}

		[Test]
		public void ThrowsOnReleasingInvalidScope()
		{
			var scope = MockRepository.GenerateMock<IScope>();
			var valid = true;
			scope.Stub(s => s.IsValid).Do((Func<bool>)(() => valid));
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());

			var cc = new ConversationContext();

			cc.RegisterScope(scope);
			valid = false;
			var e = Assert.Throws<InvalidOperationException>(() => cc.ReleaseScope(scope));
			Assert.That(e.Message, Contains.Substring("invalid"));
			Assert.That(e.Message, Contains.Substring("scope"));
			Assert.That(e.Message, Contains.Substring("internal"));
			Assert.That(e.Message, Contains.Substring("dispose"));
			Assert.That(e.Message, Contains.Substring("conversation"));
		}

		[Test]
		public void ThrowsOnRegisteringScopeWithoutConversation()
		{
			var scope = MockRepository.GenerateStub<IScope>();
			var cc = new ConversationContext();

			scope.Stub(s => s.IsValid).Return(true);

			var e = Assert.Throws<InvalidOperationException>(() => cc.RegisterScope(scope));
			Assert.That(e.Message, Contains.Substring("conversation"));
			Assert.That(e.Message, Contains.Substring("null"));
			Assert.That(e.Message, Contains.Substring("internal"));
		}

		[Test]
		public void WhenAScopeIsRegisteredItIsUsedForCurrentConversation()
		{
			var defConv = MockRepository.GenerateMock<IConversation>();
			var conv = MockRepository.GenerateMock<IConversation>();

			var scope = MockRepository.GenerateMock<IScope>();
			scope.Stub(s => s.AssociatedConversation).Return(conv);
			scope.Stub(s => s.IsValid).Return(true);

			var cc = new ConversationContext();
			cc.SetDefaultConversation(defConv);

			Assert.That(cc.CurrentConversation, Is.SameAs(defConv));
			cc.RegisterScope(scope);
			Assert.That(cc.CurrentScope, Is.SameAs(scope));
			Assert.That(cc.CurrentConversation, Is.SameAs(conv));
			cc.ReleaseScope(scope);
			Assert.That(cc.CurrentConversation, Is.SameAs(defConv));
		}

		[Test]
		public void ScopesCanBeNested()
		{
			var outerConv = MockRepository.GenerateMock<IConversation>();
			var innerConv = MockRepository.GenerateMock<IConversation>();

			var outerScope = MockRepository.GenerateMock<IScope>();
			var innerScope = MockRepository.GenerateMock<IScope>();
			innerScope.Stub(s => s.AssociatedConversation).Return(innerConv);
			outerScope.Stub(s => s.AssociatedConversation).Return(outerConv);
			innerScope.Stub(s => s.IsValid).Return(true);
			outerScope.Stub(s => s.IsValid).Return(true);

			var cc = new ConversationContext();

			cc.RegisterScope(outerScope);
			Assert.That(cc.CurrentScope, Is.SameAs(outerScope));
			Assert.That(cc.CurrentConversation, Is.SameAs(outerConv));
			cc.RegisterScope(innerScope);
			Assert.That(cc.CurrentScope, Is.SameAs(innerScope));
			Assert.That(cc.CurrentConversation, Is.SameAs(innerConv));
			cc.ReleaseScope(innerScope);
			Assert.That(cc.CurrentScope, Is.SameAs(outerScope));
			Assert.That(cc.CurrentConversation, Is.SameAs(outerConv));
			cc.ReleaseScope(outerScope);
			Assert.That(cc.CurrentScope, Is.Null);
			Assert.That(cc.CurrentConversation, Is.Null);
		}

		[Test]
		public void ScopesMustBeReleasedInOrder()
		{
			var outerConv = MockRepository.GenerateMock<IConversation>();
			var innerConv = MockRepository.GenerateMock<IConversation>();

			var outerScope = MockRepository.GenerateMock<IScope>();
			var innerScope = MockRepository.GenerateMock<IScope>();
			innerScope.Stub(s => s.AssociatedConversation).Return(innerConv);
			outerScope.Stub(s => s.AssociatedConversation).Return(outerConv);
			innerScope.Stub(s => s.IsValid).Return(true);
			outerScope.Stub(s => s.IsValid).Return(true);

			var cc = new ConversationContext();

			cc.RegisterScope(outerScope);
			cc.RegisterScope(innerScope);
			var e = Assert.Throws<InvalidOperationException>(() => cc.ReleaseScope(outerScope));
			Assert.That(e.Message, Contains.Substring("release"));
			Assert.That(e.Message, Contains.Substring("order"));
			Assert.That(e.Message, Contains.Substring("nesting"));
		}

		[Test]
		public void ScopesMustBeRegisteredBeforeRelease()
		{
			var outerConv = MockRepository.GenerateMock<IConversation>();
			var innerConv = MockRepository.GenerateMock<IConversation>();

			var outerScope = MockRepository.GenerateMock<IScope>();
			var innerScope = MockRepository.GenerateMock<IScope>();
			innerScope.Stub(s => s.AssociatedConversation).Return(innerConv);
			outerScope.Stub(s => s.AssociatedConversation).Return(outerConv);
			innerScope.Stub(s => s.IsValid).Return(true);
			outerScope.Stub(s => s.IsValid).Return(true);

			var cc = new ConversationContext();

			cc.RegisterScope(outerScope);
			var e = Assert.Throws<InvalidOperationException>(() => cc.ReleaseScope(innerScope));
			Assert.That(e.Message, Contains.Substring("release"));
			Assert.That(e.Message, Contains.Substring("not"));
			Assert.That(e.Message, Contains.Substring("register"));
		}

		[Test]
		public void ScopesCannotBeRegisteredTwice()
		{
			var scope = MockRepository.GenerateStub<IScope>();
			var cc = new ConversationContext();

			scope.Stub(s => s.IsValid).Return(true);
			scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());

			cc.RegisterScope(scope);
			var e = Assert.Throws<InvalidOperationException>(() => cc.RegisterScope(scope));
			Assert.That(e.Message, Contains.Substring("scope"));
			Assert.That(e.Message, Contains.Substring("register"));
			Assert.That(e.Message, Contains.Substring("twice"));
		}

	}
}
