using System;
using System.IO;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class SolvePartialProblemsStrategyTests
    {
        private static Stream stream = null;
        private Mock<ServerNetworkAdapter> networkAdapterMock = new Mock<ServerNetworkAdapter>(stream);

        [TestMethod]
        public void RegisterPartialProblems()
        {
            DvrpProblem.PartialProblems.Clear();
            SolvePartialProblems partialProblems = new SolvePartialProblems()
            {
                CommonData = new byte[1],
                Id = 1,
                PartialProblems =
                    new SolvePartialProblemsPartialProblem[]
                    {new SolvePartialProblemsPartialProblem() {Data = new byte[1]},}
            };
            string msg = MessageSerialization.Serialize(partialProblems);
            SolvePartialProblemsStrategy solvePartialProblemsStrategy = new SolvePartialProblemsStrategy();

            solvePartialProblemsStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.SolvePartialProblemsMessage, new TimeSpan(0,1,0));

            Assert.AreEqual(DvrpProblem.PartialProblems.Count, 1);
        }
    }
}
