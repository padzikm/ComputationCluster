using System;
using System.IO;
using System.Linq;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class RegisterStrategyTests
    {
        private static Stream stream = null;
        private Mock<ServerNetworkAdapter> networkAdapterMock = new Mock<ServerNetworkAdapter>(stream);

        [TestMethod]
        public void RegisterComponentIfAllFieldsAreDefined()
        {
            DvrpProblem.Tasks.Clear();
            Register request = new Register()
            {
                ParallelThreads = 1,
                SolvableProblems = new string[1],
                Type = RegisterType.TaskManager
            };
            RegisterStrategy registerStrategy = new RegisterStrategy();
            string msg = MessageSerialization.Serialize(request);
            
            registerStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.RegisterMessage, new TimeSpan(0, 1, 0));

            networkAdapterMock.Verify(p => p.Send(It.IsAny<RegisterResponse>()), Times.Once);
            Assert.AreEqual(DvrpProblem.Tasks.Count, 1);            
        }

        [TestMethod]
        public void RejectComponentIfSolvableProblemsAreNonDefined()
        {
            DvrpProblem.Tasks.Clear();
            Register request = new Register()
            {
                ParallelThreads = 1,
                SolvableProblems = null,
                Type = RegisterType.TaskManager
            };
            RegisterStrategy registerStrategy = new RegisterStrategy();
            string msg = MessageSerialization.Serialize(request);

            registerStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.RegisterMessage, new TimeSpan(0, 1, 0));
            
            networkAdapterMock.Verify(p => p.Send(It.IsAny<RegisterResponse>()), Times.Never);
            Assert.AreEqual(DvrpProblem.Tasks.Count, 0);                      
        }
    }
}
