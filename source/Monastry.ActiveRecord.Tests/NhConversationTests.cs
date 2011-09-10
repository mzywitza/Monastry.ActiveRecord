using System;
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
		public void NeedsSessionFactoryAndContext()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf,cc);
			Assert.That(c != null);
		}

		[Test]
		public void RejectsNullAsSessionFactory()
		{
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var e = Assert.Throws<ArgumentNullException>(() => new NhConversation(null,cc));
			Assert.That(e.ParamName, Is.EqualTo("sessionFactory"));
		}

		[Test]
		public void RejectsNullAsContext()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var e = Assert.Throws<ArgumentNullException>(() => new NhConversation(sf, null));
			Assert.That(e.ParamName, Is.EqualTo("context"));
		}

		[Test]
		public void DefaultCommitModeIsAutomatic()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			var cm = c.CommitMode;
			Assert.That(cm, Is.EqualTo(ConversationCommitMode.Automatic));
		}

		[Test]
		public void CanSetDifferentCommitMode()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var cm = ConversationCommitMode.Explicit;
			var c = new NhConversation(sf,cc) { CommitMode = cm };
			Assert.That(c.CommitMode, Is.EqualTo(cm));
		}

		[Test]
		public void CannotSetCommitModeAfterSessionStarted()
		{
			var t = MockRepository.GenerateStub<ITransaction>();
			var s = MockRepository.GenerateStub<ISession>();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var cm = ConversationCommitMode.Explicit;
			var c = new NhConversation(sf, cc);
			c.Execute(x => { }); // Call doesn't matter, it's a stub
			var e = Assert.Throws<NotSupportedException>(() => c.CommitMode = cm);
			Assert.That(e.Message, Contains.Substring("CommitMode"));
			Assert.That(e.Message, Contains.Substring("started"));
		}

		[Test]
		public void CancelingRaisesEvent()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf,cc);
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
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var c = new NhConversation(sf,cc);
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
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
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
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			sf.Stub(x => x.OpenSession()).Return(s);
			s.Stub(x => x.BeginTransaction()).IgnoreArguments().Return(t);
			var c = new NhConversation(sf,cc);
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
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			c.Cancel();
			Assert.That(c.IsCanceled);
		}

		[Test]
		public void CantCommitAfterCanceling()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			c.Cancel();
			Assert.Throws<InvalidOperationException>(() => c.Commit());
		}

		[Test]
		public void CanCreateScope()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			using (c.Scope()) { }
		}

		[Test]
		public void ScopeIsRegistered()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateMock<INhConversationContext>();
			cc.Expect(m => m.RegisterScope(null)).IgnoreArguments();
			var c = new NhConversation(sf, cc);
			using (c.Scope()) { }
			cc.VerifyAllExpectations();
		}

		[Test]
		public void ScopeIsDeregistered()
		{
			INhScope registeredScope = null;
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateMock<INhConversationContext>();
			cc.Expect(m => m.RegisterScope(null)).IgnoreArguments().WhenCalled(i=>registeredScope = (INhScope)i.Arguments[0]);
			var c = new NhConversation(sf, cc);
			using (c.Scope()) {
				cc.Expect(m => m.ReleaseScope(registeredScope));            
			}
			cc.VerifyAllExpectations();
		}

		[Test]
		public void CanSwitchBetweenConversations()
		{
			NhConversationContext context = new NhConversationContext();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var conv1 = new NhConversation(sf, context);
			var conv2 = new NhConversation(sf, context);

			Assert.That(context.CurrentConversation, Is.Null);
			using (conv1.Scope())
			{
				Assert.That(context.CurrentConversation, Is.SameAs(conv1));
				using (conv2.Scope())
				{
					Assert.That(context.CurrentConversation, Is.SameAs(conv2));
				}
				Assert.That(context.CurrentConversation, Is.SameAs(conv1));
			}
			Assert.That(context.CurrentConversation, Is.Null);
		}

		[Test]
		public void CanUseExecuteToSwitchConversation()
		{
			NhConversationContext context = new NhConversationContext();
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var conv1 = new NhConversation(sf, context);
			var conv2 = new NhConversation(sf, context);

			Assert.That(context.CurrentConversation, Is.Null);
			conv1.Execute(()=>Assert.That(context.CurrentConversation, Is.SameAs(conv1)));
			conv2.Execute(()=>Assert.That(context.CurrentConversation, Is.SameAs(conv2)));
			Assert.That(context.CurrentConversation, Is.Null);
		}

		[Test]
		public void CannotCallExecuteWhenConversationIsCanceled()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			c.Cancel();
			var e = Assert.Throws<InvalidOperationException>(() => c.Execute(() => { ; })); // Call doesn't matter, it's a stub
			Assert.That(e.Message, Contains.Substring("canceled"));
		}

		[Test]
		public void ExceptionsInExecuteCancelTheConversation()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			ConversationCanceledEventArgs eventRaised = null;
			object eventRaiser = null;
			c.Canceled += (o, a) =>
			{
				eventRaiser = o;
				eventRaised = a;
			};
			var e = Assert.Throws<Exception>(() => c.Execute(() => { throw new Exception("foo"); }));
			Assert.That(e.Message, Is.EqualTo("foo"));
			Assert.That(eventRaised, Is.Not.Null);
			Assert.That(eventRaised.CanceledByUser, Is.False);
			Assert.That(eventRaised.Exception, Is.SameAs(e));
			Assert.That(eventRaiser, Is.SameAs(c));
		}
		
		[Test]
		public void ExceptionsInExecuteSilentlyCancelTheConversationWithoutThrowing()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			var e = new Exception("foo");
			ConversationCanceledEventArgs eventRaised = null;
			object eventRaiser = null;
			c.Canceled += (o, a) =>
			{
				eventRaiser = o;
				eventRaised = a;
			};
			c.ExecuteSilently(() => { throw e; }); // Doesn't throw.
			Assert.That(eventRaised, Is.Not.Null);
			Assert.That(eventRaised.CanceledByUser, Is.False);
			Assert.That(eventRaised.Exception, Is.SameAs(e));
			Assert.That(eventRaiser, Is.SameAs(c));
		}
		
		[Test]
		public void ScopesAreInvalidatedOnConversationDisposal()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = MockRepository.GenerateStub<INhConversationContext>();
			var c = new NhConversation(sf, cc);
			INhScope s = (INhScope)c.Scope();
			Assert.That(s.IsValid);
			c.Dispose();
			Assert.That(s.IsValid, Is.False);
			s.Dispose(); // Must not throw
		}

		[Test]
		public void ScopesAreNotLeaking()
		{
			var sf = MockRepository.GenerateStub<ISessionFactory>();
			var cc = new NhConversationContext();
			var c = new NhConversation(sf, cc);
			WeakReference weakRef = CreateScope(c);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Assert.That(weakRef.Target, Is.Null);
		}

		private WeakReference CreateScope(IConversation c)
		{
			using (var scope = c.Scope())
			{
				return new WeakReference(scope);
			}
		}
	}

}
