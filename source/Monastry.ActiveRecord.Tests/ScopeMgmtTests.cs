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
    public class ScopeMgmtTests
    {
        [Test]
        public void CanRegisterAScope()
        {
            var scope = MockRepository.GenerateStub<IScope>();
            scope.Stub(s => s.IsValid).Return(true);
            scope.Stub(s => s.AssociatedConversation).Return(MockRepository.GenerateMock<IConversation>());
            var cc = new ConversationContext();

            cc.RegisterScope(scope);
        }

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
            var e = Assert.Throws<ArgumentNullException>(()=>cc.RegisterScope(null));
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

            var e = Assert.Throws<InvalidOperationException>(()=>cc.RegisterScope(scope));
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

    }
}
