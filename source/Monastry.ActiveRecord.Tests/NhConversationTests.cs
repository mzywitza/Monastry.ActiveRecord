﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Monastry.ActiveRecord;
using Rhino.Mocks;
using NHibernate;

namespace Monastry.ActiveRecord.Tests
{
	[TestFixture]
	public class NhConversationTests
	{
		[Test]
		public void Exists()
		{
			NhConversation c = null;
			Assert.That(c == null); // Getting rid of CS0219: c is assigned but never used.
		}

		[Test]
		public void ImplementsINhConversation()
		{
			NhConversation c = null;
			INhConversation i = c;
		}

		[Test]
		public void NeedsSessionFactory()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			Assert.That(c != null);
		}

		[Test]
		public void RejectsNullAsSessionFactory()
		{
			var e = Assert.Throws<ArgumentNullException>(() => new NhConversation(null));
			Assert.That(e.ParamName, Is.EqualTo("sessionFactory"));
		}

		[Test]
		public void DefaultCommitModeIsAutomatic()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			var cm = c.CommitMode;
			Assert.That(cm, Is.EqualTo(ConversationCommitMode.Automatic));
		}

		[Test]
		public void CanSetDifferentCommitMode()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cm = ConversationCommitMode.Explicit;
			var c = new NhConversation(sf) { CommitMode = cm };
			Assert.That(c.CommitMode, Is.EqualTo(cm));
		}

		[Test]
		public void CannotSetCommitModeAfterSessionStarted()
		{
			var t = MockRepository.GenerateStub<ITransaction>();
			var s = MockRepository.GenerateStub<ISession>();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var cm = ConversationCommitMode.Explicit;
			var c = new NhConversation(sf);
			c.Execute(x => { }); // Call doesn't matter, it's a stub
			var e = Assert.Throws<NotSupportedException>(() => c.CommitMode = cm);
			Assert.That(e.Message, Contains.Substring("CommitMode"));
			Assert.That(e.Message, Contains.Substring("started"));
		}

		[Test]
		public void CancelingRaisesEvent()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			ConversationCanceledEventArgs eventRaised = null;
			object eventRaiser = null;
			c.Canceled += (o, a) =>
			{
				eventRaiser = o;
				eventRaised = a;
			};
			c.Cancel();
			Assert.That(eventRaised, Is.Not.Null);
			Assert.That(eventRaised.CanceledByUser);
			Assert.That(eventRaised.Exception, Is.Null);
			Assert.That(eventRaiser, Is.SameAs(c));
		}

		[Test]
		public void ExceptionsCancelTheConversation()
		{
			var t = MockRepository.GenerateStub<ITransaction>();
			var s = MockRepository.GenerateStub<ISession>();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var c = new NhConversation(sf);
			ConversationCanceledEventArgs eventRaised = null;
			object eventRaiser = null;
			c.Canceled += (o, a) =>
			{
				eventRaiser = o;
				eventRaised = a;
			};
			var e = Assert.Throws<Exception>(() => c.Execute(x => { throw new Exception("foo"); }));
			Assert.That(e.Message, Is.EqualTo("foo"));
			Assert.That(eventRaised, Is.Not.Null);
			Assert.That(eventRaised.CanceledByUser, Is.False);
			Assert.That(eventRaised.Exception, Is.SameAs(e));
			Assert.That(eventRaiser, Is.SameAs(c));
		}

		[Test]
		public void CannotCallIntoCanceledConversation()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			c.Cancel();
			var e = Assert.Throws<InvalidOperationException>(() => c.Execute(s => { })); // Call doesn't matter, it's a stub
			Assert.That(e.Message, Contains.Substring("canceled"));
		}

		[Test]
		public void RestartingTheConversationResetsCancelState()
		{
			var t = MockRepository.GenerateStub<ITransaction>();
			var s = MockRepository.GenerateStub<ISession>();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var c = new NhConversation(sf);
			c.Cancel();
			c.Restart();
			bool called = false;
			c.Execute(x => called = true); // Call doesn't matter, it's a stub
			Assert.That(called);
		}

		[Test]
		public void CancelStateCanBeQueried()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			c.Cancel();
			Assert.That(c.IsCanceled);
		}

		[Test]
		public void CantCommitAfterCanceling()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var c = new NhConversation(sf);
			c.Cancel();
			Assert.Throws<InvalidOperationException>(() => c.Commit());
		}
	}
}
