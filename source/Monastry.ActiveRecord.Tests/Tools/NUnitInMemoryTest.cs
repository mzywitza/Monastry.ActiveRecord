using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monastry.ActiveRecord.Tests.Tools
{
    public abstract class NUnitInMemoryTest : InMemoryTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();
        }
    }
}
