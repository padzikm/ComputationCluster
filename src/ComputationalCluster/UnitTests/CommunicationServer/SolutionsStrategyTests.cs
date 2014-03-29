using System;
using System.IO;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class SolutionsStrategyTests
    {
        private static Stream stream = null;
        private Mock<ServerNetworkAdapter> networkAdapterMock = new Mock<ServerNetworkAdapter>(stream);

        [TestMethod]
        public void RegisterFinalSolution()
        {
            DvrpProblem.ProblemSolutions.Clear();
            Solutions request = new Solutions()
            {
                Id = 1,
                CommonData = new byte[1],
                Solutions1 = new SolutionsSolution[] {new SolutionsSolution() {Type = SolutionsSolutionType.Final},}
            };
            SolutionsStrategy solutionsStrategy = new SolutionsStrategy();
            string msg = MessageSerialization.Serialize(request);

            solutionsStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.RegisterMessage, new TimeSpan(0, 1, 0));
            
            Assert.AreEqual(DvrpProblem.ProblemSolutions.Count, 1);       
        }
    }
}
