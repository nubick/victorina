using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Victorina.Test
{
    public class IpCodeSystemTest
    {
        [Test]
        public void GetCodeTest()
        {
            IpCodeSystem s = new IpCodeSystem();
            
            Assert.AreEqual(s.GetCode("0.0.0.0"), "AAAAAAAA");
            Assert.AreEqual(s.GetCode("255.255.255.255"), "VJVJVJVJ");
            Assert.AreEqual(s.GetCode("31.41.196.195"), "FBPBOHNH");
            Assert.AreEqual(s.GetCode("10.0.0.109"), "KAAAAAFE");
            Assert.AreEqual(s.GetCode("94.232.233.226"), "QDYIZISI");
            Assert.AreEqual(s.GetCode("127.0.0.1"), Static.LocalhostGameCode);
        }
        
        [Test]
        public void GetIpTest()
        {
            IpCodeSystem s = new IpCodeSystem();

            Assert.AreEqual(s.GetIp("AAAAAAAA"), "0.0.0.0");
            Assert.AreEqual(s.GetIp("VJVJVJVJ"), "255.255.255.255");
            Assert.AreEqual(s.GetIp("FBPBOHNH"), "31.41.196.195");
            Assert.AreEqual(s.GetIp("KAAAAAFE"), "10.0.0.109");
            Assert.AreEqual(s.GetIp("QDYIZISI"), "94.232.233.226");
        }

        [Test]
        public void IsGameCodeValidTest()
        {
            IpCodeSystem s = new IpCodeSystem();

            Assert.IsFalse(s.IsValidGameCode(null));
            Assert.IsFalse(s.IsValidGameCode(""));
            Assert.IsFalse(s.IsValidGameCode("12345678"));
            Assert.IsFalse(s.IsValidGameCode("ZZZZZZZZ"));
            Assert.IsFalse(s.IsValidGameCode("AaAaAaAa"));
            Assert.IsFalse(s.IsValidGameCode("ABABABABABAB"));
            Assert.IsFalse(s.IsValidGameCode("AZAZAZAZ"));
            
            Assert.IsTrue(s.IsValidGameCode("AAAAAAAA"));
            Assert.IsTrue(s.IsValidGameCode("QDYIZISI"));
            Assert.IsTrue(s.IsValidGameCode("ABABABAB"));
            Assert.IsTrue(s.IsValidGameCode("ABBBCBDB"));
            Assert.IsTrue(s.IsValidGameCode("VJVJVJVJ"));
        }
    }
}