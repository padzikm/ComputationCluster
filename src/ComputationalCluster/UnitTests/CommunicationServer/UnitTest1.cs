using System;
using System.IO;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Register register = new Register()
            {
                ParallelThreads = 1,
                SolvableProblems = null,
                Type = RegisterType.ComputationalNode
            };
            string msg = MessageSerialization.Serialize(register);
            Stream stream = null;
            Mock<ServerNetworkAdapter> mockAdapter = new Mock<ServerNetworkAdapter>(stream);

            RegisterStrategy registerStrategy = new RegisterStrategy();
            registerStrategy.HandleMessage(mockAdapter.Object, msg, MessageType.RegisterMessage, new TimeSpan(0,1,0));

            Assert.AreEqual(DvrpProblem.Nodes.Count, 1);
            mockAdapter.Verify(m => m.Send(register), Times.Never);            
        }

        [TestMethod]
        public void TestMethod2()
        {
            Register register = new Register()
            {
                ParallelThreads = 1,
                SolvableProblems = null,
                Type = RegisterType.ComputationalNode
            };
            string msg = MessageSerialization.Serialize(register);
            Stream stream = null;
            Mock<ServerNetworkAdapter> mockAdapter = new Mock<ServerNetworkAdapter>(stream);

            RegisterStrategy registerStrategy = new RegisterStrategy();
            registerStrategy.HandleMessage(mockAdapter.Object, msg, MessageType.RegisterMessage, new TimeSpan(0, 1, 0));

            Assert.AreEqual(DvrpProblem.Nodes.Count, 1);
            mockAdapter.Verify(m => m.Send(register), Times.Never);
        }
    }
}
