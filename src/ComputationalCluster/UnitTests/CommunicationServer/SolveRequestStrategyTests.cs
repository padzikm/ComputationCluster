using System;
using System.IO;
using Common;
using CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.CommunicationServer
{
    [TestClass]
    public class SolveRequestStrategyTests
    {
        private static Stream stream = null;
        private Mock<ServerNetworkAdapter> networkAdapterMock = new Mock<ServerNetworkAdapter>(stream);

        [TestMethod]
        public void AcceptProblemIfAllFieldsAreDefined()
        {
            DvrpProblem.Problems.Clear();
            DvrpProblem.ProblemsDivideWaiting.Clear();
            SolveRequest request = new SolveRequest()
            {
                Data = new byte[1],
                ProblemType = "dvrp",
                SolvingTimeout = 1,
                SolvingTimeoutSpecified = true
            };
            string msg = MessageSerialization.Serialize(request);
            SolveRequestStrategy requestStrategy = new SolveRequestStrategy();
            requestStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.SolveRequestMessage, new TimeSpan(0,1,0));
            
            networkAdapterMock.Verify(p => p.Send(It.IsAny<SolveRequestResponse>()), Times.Once);
            Assert.AreEqual(DvrpProblem.Problems.Count, 1); 
            Assert.AreEqual(DvrpProblem.ProblemsDivideWaiting.Count, 1);
        }

        [TestMethod]
        public void RejectProblemIfDataIsNonDefined()
        {
            DvrpProblem.Problems.Clear();
            DvrpProblem.ProblemsDivideWaiting.Clear();
            SolveRequest request = new SolveRequest()
            {
                Data = null,
                ProblemType = "dvrp",
                SolvingTimeout = 1,
                SolvingTimeoutSpecified = true
            };
            string msg = MessageSerialization.Serialize(request);
            SolveRequestStrategy requestStrategy = new SolveRequestStrategy();
            requestStrategy.HandleMessage(networkAdapterMock.Object, msg, MessageType.SolveRequestMessage, new TimeSpan(0, 1, 0));

            networkAdapterMock.Verify(p => p.Send(It.IsAny<SolveRequestResponse>()), Times.Never);
            Assert.AreEqual(DvrpProblem.Problems.Count, 0);
            Assert.AreEqual(DvrpProblem.ProblemsDivideWaiting.Count, 0);
        }
    }
}
