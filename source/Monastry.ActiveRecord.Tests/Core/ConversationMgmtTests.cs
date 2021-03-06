﻿using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Monastry.ActiveRecord.Tests.Core
{
	[TestFixture]
	public class ConversationMgmtTests
	{
		[Test]
		public void DefaultConversationMustNotBeNull()
		{
			IConversationContext cc = new ConversationContext();
			Assert.Throws<ArgumentNullException>(() => cc.SetDefaultConversation(null));
		}

		[Test]
		public void UsefulExceptionOnCallingEndDefaultConversationContextWithoutDefaultConversation()
		{
			IConversationContext cc = new ConversationContext();
			var e = Assert.Throws<InvalidOperationException>(() => cc.UnsetDefaultConversation());

			Assert.That(e.Message, Contains.Substring("default"));
			Assert.That(e.Message, Contains.Substring("conversation"));
		}

		[Test]
		public void TwiceEndDefaultConversationThrows()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
			IConversationContext cc = new ConversationContext();

			cc.SetDefaultConversation(conv);

			cc.UnsetDefaultConversation();

			Assert.Throws<InvalidOperationException>(() => cc.UnsetDefaultConversation());
		}

		[Test]
		public void CannotSetDefaultConversationTwice()
		{
			IConversation conv1 = MockRepository.GenerateStub<IConversation>();
			IConversation conv2 = MockRepository.GenerateStub<IConversation>();
			IConversationContext cc = new ConversationContext();

			cc.SetDefaultConversation(conv1);
			Assert.Throws<InvalidOperationException>(() => cc.SetDefaultConversation(conv2));
		}

		[Test]
		public void CanSetDefaultConversationAfterAPreviousOneHasEnded()
		{
			IConversation conv1 = MockRepository.GenerateStub<IConversation>();
			IConversation conv2 = MockRepository.GenerateStub<IConversation>();
			IConversationContext cc = new ConversationContext();

			cc.SetDefaultConversation(conv1);
			cc.UnsetDefaultConversation();

			cc.SetDefaultConversation(conv2);
		}

		[Test]
		public void DefaultConversationIsUsedWhenNoScopeIsActive()
		{
			IConversation conv = MockRepository.GenerateStub<IConversation>();
			IConversationContext cc = new ConversationContext();

			cc.SetDefaultConversation(conv);
			Assert.That(cc.CurrentConversation, Is.SameAs(conv));
		}

	}
}
