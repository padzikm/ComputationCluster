using System;
using System.IO;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class SolutionRequestStrategyTests
    {
        private static Stream stream = null;
        private Mock<ServerNetworkAdapter> networkAdapterMock = new Mock<ServerNetworkAdapter>(stream);

        [TestMethod]
        public void ResponseIfProblemIsSolved()
        {
            DvrpProblem.Problems.Clear();
            DvrpProblem.ProblemSolutions.Clear();
            SolveRequest solveRequest = new SolveRequest() {Data = new byte[1]};
            DvrpProblem.Problems.Add(1, solveRequest);
            Solutions solution = new Solutions()
            {
                CommonData = new byte[1],
                Solutions1 = new SolutionsSolution[] {new SolutionsSolution() {Data = new byte[1]}}
            };
            SolutionRequest request = new SolutionRequest() {Id = 1};
            DvrpProblem.ProblemSolutions.Add(request.Id, solution);
            string msg = MessageSerialization.Serialize(request);
            SolutionRequestStrategy solutionRequestStrategy = new SolutionRequestStrategy();

            solutionRequestStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.SolutionRequestMessage, new TimeSpan(0,1,0));

            networkAdapterMock.Verify(p => p.Send(It.IsAny<Solutions>()), Times.Once);
            //Assert.AreEqual(DvrpProblem.ProblemSolutions.Count, 0);
        }
    }
}
