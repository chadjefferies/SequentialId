using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SequentialId.Tests
{
    [TestClass]
    public class u_SequentialId
    {
        [TestMethod]
        public void Equals()
        {
            var newId = SequentialId.NewId();
            var newId2 = new SequentialId(newId.ToByteArray());
            Assert.AreEqual(newId, newId2);
        }

        [TestMethod]
        public void NotEquals()
        {
            var newId = SequentialId.NewId();
            var newId2 = SequentialId.NewId();
            Assert.AreNotEqual(newId, newId2);
        }

        [TestMethod]
        public void TimestampEqual()
        {
            var newId = SequentialId.NewId();
            var newId2 = new SequentialId(newId.ToByteArray());
            Assert.AreEqual(newId.ToTimestamp(), newId2.ToTimestamp());
        }

        [TestMethod]
        public void TimestampNotEqual()
        {
            var newId = SequentialId.NewId();
            var newId2 = new SequentialId(DateTime.UtcNow.AddSeconds(1));
            Assert.AreNotEqual(newId.ToTimestamp(), newId2.ToTimestamp());
        }
    }
}
