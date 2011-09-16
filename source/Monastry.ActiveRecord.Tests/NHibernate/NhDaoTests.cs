using System;
using Monastry.ActiveRecord.Tests.Model;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;


namespace Monastry.ActiveRecord.Tests.NHibernate
{
	[TestFixture]
	public class NhDaoTests
	{
		[Test]
		public void NhDaoDoesntAcceptNullContext()
		{
			Assert.Throws<ArgumentNullException>(() => new NhDao<Software>(null));
		}

		[Test]
		public void NhDaoUsesContextToDetermineConversation()
		{
			INhConversation conv = MockRepository.GenerateMock<INhConversation>();
			INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
			context.Stub(c => c.CurrentConversation).Return(conv);

			INhDao<Software> nhDao = new NhDao<Software>(context);
			IDao<Software> dao = nhDao;

			Assert.That(nhDao.CurrentConversation, Is.SameAs(conv));
			Assert.That(dao.CurrentConversation, Is.SameAs(conv));
		}

		private object CreateMock()
		{
			ISession session = MockRepository.GenerateStub<ISession>();
			INhConversation conversation = MockRepository.GenerateStub<INhConversation>();
			INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
			context.Stub(c => c.CurrentConversation).Return(conversation);
			conversation.Stub(c => c.Execute(Arg<Action<ISession>>.Is.Anything));
			return null;
		}
	}
}
