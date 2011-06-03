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
	public class ConversationMgmtTests
	{
		[Test]
		public void ConversationContextIsIConversationContext()
		{
			IConversationContext<IConversation,IScope<IConversation>> cc = 
                new ConversationContext<IConversation,IScope<IConversation>>();
		}

		[Test]
		public void CanSetDefaultScope()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv);
		}

		[Test]
		public void DefaultConversationMustNotBeNull()
		{
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();
            Assert.Throws<ArgumentNullException>(() => cc.SetDefaultConversation(null));
		}

		[Test]
		public void DefaultConversationIsDiposedOnEndDefaultConversation()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv);
			cc.EndDefaultConversation();

			conv.AssertWasCalled(c => c.Dispose());
		}

		[Test]
		public void UsefulExceptionOnCallingEndDefaultConversationContextWithoutDefaultConversation()
		{
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();
            var e = Assert.Throws<InvalidOperationException>(() => cc.EndDefaultConversation());

			Assert.That(e.Message, Contains.Substring("default"));
			Assert.That(e.Message, Contains.Substring("conversation"));
		}

		[Test]
		public void TwiceEndDefaultConversationThrows()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv);

			cc.EndDefaultConversation();

			Assert.Throws<InvalidOperationException>(() => cc.EndDefaultConversation());
		}

		[Test]
		public void CannotSetDefaultConversationTwice()
		{
			IConversation conv1 = MockRepository.GenerateStub<IConversation>();
			IConversation conv2 = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv1);
			Assert.Throws<InvalidOperationException>(() => cc.SetDefaultConversation(conv2));
		}

		[Test]
		public void CanSetDefaultConversationAfterAPreviousOneHasEnded()
		{
			IConversation conv1 = MockRepository.GenerateStub<IConversation>();
			IConversation conv2 = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv1);
			cc.EndDefaultConversation();

			cc.SetDefaultConversation(conv2);
		}

		[Test]
		public void DefaultConversationIsUsedWhenNoScopeIsActive()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
            IConversationContext<IConversation, IScope<IConversation>> cc =
                new ConversationContext<IConversation, IScope<IConversation>>();

			cc.SetDefaultConversation(conv);
			Assert.That(cc.CurrentConversation, Is.SameAs(conv));
		}

	}
}
