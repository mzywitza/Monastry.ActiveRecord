using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using NHibernate;


namespace Monastry.ActiveRecord.Tests
{
    [TestFixture]
    public class NhDaoTests
    {
        [Test]
        public void InterfaceIDaoExists()
        {
            IDao<Customer> dao = null;
            Assert.That(dao, Is.Null);
        }

        [Test]
        public void INhDaoIsAnIDao()
        {
            INhDao<Customer> nhDao = null;
            IDao<Customer> dao = nhDao;
        }

        [Test]
        public void NhDaoExists()
        {
            INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
            INhDao<Customer> dao = new NhDao<Customer>(context);
        }

        [Test]
        public void IDaoHasAConversationProperty()
        {
            INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
            IDao<Customer> dao = new NhDao<Customer>(context);
            IConversation c = dao.CurrentConversation;
        }

        [Test]
        public void INhDaoHasASeparateConversationProperty()
        {
            INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
            INhDao<Customer> dao = new NhDao<Customer>(context);
            INhConversation c = dao.CurrentConversation;
        }

        [Test]
        public void NhDaoRequiresConversationContext()
        {
            INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
            INhDao<Customer> dao = new NhDao<Customer>(context);
        }

        [Test]
        public void NhDaoDoesntAcceptNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new NhDao<Customer>(null));
        }

        [Test]
        public void NhDaoUsesContextToDetermineConversation()
        {
            INhConversation conv = MockRepository.GenerateMock<INhConversation>();
            INhConversationContext context = MockRepository.GenerateStub<INhConversationContext>();
            context.Stub(c => c.CurrentConversation).Return(conv);
            
            INhDao<Customer> nhDao = new NhDao<Customer>(context);
            IDao<Customer> dao = nhDao;

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

    class Customer
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }
}
