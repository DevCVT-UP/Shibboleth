using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShibbolethLogin.ActiveDirectory.ActiveDirectory;
using ShibbolethLogin.Roles;
using System;
using System.IO;
using System.Linq;

namespace ShibbolethLogin.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonRoles()
        {
            var jsonRoles = new JsonConfigRoleResolver(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "roles.json"));           

            Assert.AreEqual(jsonRoles.Roles.Count(), 3);
            Assert.IsTrue(jsonRoles.Roles.Contains("employee"));
            Assert.AreEqual(jsonRoles.GetUserRoles("user1", new string[] { }).Count(),  2);
            Assert.IsTrue(jsonRoles.IsInRole("user4", "student", new string[] { }));
            Assert.IsFalse(jsonRoles.IsInRole("user5", "employee", new string[] { }));
        }

        [TestMethod]
        public void LinkedResolver()
        {
            var jsonRoles = new JsonConfigRoleResolver(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "roles.json"));
            var linkedResolver = new LinkedRoleResolver(jsonRoles, new ADRoleResolver(new ADConfig()));
            Assert.AreEqual(linkedResolver.GetUserRoles("user1", new string[] { }).Count(), 2);
            Assert.IsTrue(linkedResolver.IsInRole("user4", "student", new string[] { }));
            Assert.IsFalse(linkedResolver.IsInRole("user5", "employee", new string[] { }));
            Assert.AreEqual(linkedResolver.GetUserRoles("user8", new string[] { "zamestnanec", "student"}).Count(), 2);
            Assert.IsNotNull(linkedResolver.GetUserRoles("user8", new string[] { }));
        }
    }
}
