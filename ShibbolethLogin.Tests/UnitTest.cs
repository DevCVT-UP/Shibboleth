using ShibbolethLogin.Roles;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShibbolethLogin.ActiveDirectory;


namespace ShibbolethLogin.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonRoles()
        {
            ILogger logger = NullLogger.Instance;

            var jsonRoles = new JsonConfigRoleResolver(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "roles.json"),logger);           

            Assert.AreEqual(jsonRoles.Roles.Count(), 3);
            Assert.IsTrue(jsonRoles.Roles.Contains("employee"));
            Assert.AreEqual(jsonRoles.GetUserRoles("user1", new string[] { }).Count(),  2);
            Assert.IsTrue(jsonRoles.IsInRole("user4", "student", new string[] { }));
            Assert.IsFalse(jsonRoles.IsInRole("user5", "employee", new string[] { }));
        }

        [TestMethod]
        public void LinkedResolver()
        {
            ILogger logger = NullLogger.Instance;
            var jsonRoles = new JsonConfigRoleResolver(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "roles.json"),logger);
            var linkedResolver = new LinkedRoleResolver(jsonRoles, new ADRoleResolver(new ADConfig(logger)));
            Assert.AreEqual(linkedResolver.GetUserRoles("user1", new string[] { }).Count(), 2);
            Assert.IsTrue(linkedResolver.IsInRole("user4", "student", new string[] { }));
            Assert.IsFalse(linkedResolver.IsInRole("user5", "employee", new string[] { }));
            Assert.AreEqual(linkedResolver.GetUserRoles("user8", new string[] { "zamestnanec", "student"}).Count(), 2);
            Assert.IsNotNull(linkedResolver.GetUserRoles("user8", new string[] { }));
        }


        [TestMethod]
        public void TestUserDomainUtils()
        {

            var str1 = "user@domain";

            var split = str1.UserDomain();
            Assert.AreEqual(("user","domain"),split);
            Assert.AreEqual("user",split.Item1);
            Assert.AreEqual("domain",split.Item2);
            Assert.AreEqual((string.Empty,string.Empty),"".UserDomain());

            Assert.AreEqual("john@test","john".CanonizeUserDomainString("test"));
            Assert.AreEqual("alice@domain", "alice@domain".CanonizeUserDomainString("test"));


        }
    }
}
