using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using NetworkAdapterConstructor = Common.NetworkAdapter;
using System.Net;
using CommunicationServer;

namespace UnitTests
{
    [TestClass]
    public class NetworkAdapter
    {
        Server server = new Server(IPAddress.Any, 12345, new TimeSpan(0, 0, 10));
        SolveRequest sr = new SolveRequest() { Data = null, ProblemType = "dvrp", SolvingTimeout = 1000, SolvingTimeoutSpecified = true };

        #region NetworkAdapter constructor tests
        [TestMethod]
        public void CreateNetworkAdapterWithStringFirstParamWithCorrectValue()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);

            // act

            // assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CreateNetworkAdapterWithIpAddressFirstParamWithCorrectValue()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor(IPAddress.Parse("192.168.0.1"), 12345);

            // act

            // assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNetworkAdapterWithIncorrectValues()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", -3);

            // act

            // assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNetworkAdapterWithIncorrectValues2()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor(IPAddress.Parse(null), 12345);

            // act

            // assert
        }
        #endregion

        #region Send and receive methods
        [TestMethod]
        public void SendGenericMethodForCorrectMessage()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);
            bool condition;

            // act
            server.Start();

            condition = na.Send<SolveRequest>(sr, true);
            na.CloseConnection();

            server.Stop();

            // assert
            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void SendAndReceiveGenericMethodForCorrectMessage()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);
            SolveRequestResponse msg, msg_correct;
            msg_correct = new SolveRequestResponse() { Id = 1 };

            // act
            server.Start();

            na.StartConnection();
            na.Send<SolveRequest>(sr, false);

            msg = na.Receive<SolveRequestResponse>(false);
            na.CloseConnection();

            server.Stop();

            // assert
            Assert.IsTrue(true);
            Assert.Equals(msg, msg_correct);
        }
        #endregion

        #region StartConnection and StopConnections tests
        [TestMethod]
        public void StartConnectionForWholeServerInstanceWorking()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);

            // act
            server.Start();

            na.StartConnection();

            server.Stop();

            // assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void StopConnectionForWholeServerInstanceWorking()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);

            // act
            server.Start();

            na.CloseConnection();

            server.Stop();

            // assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void StartAndStopConnectionForWholeServerInstanceWorking()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);

            // act
            server.Start();

            na.StartConnection();
            na.CloseConnection();

            server.Stop();

            // assert
            Assert.IsTrue(true);
        }
        #endregion
    }
}
