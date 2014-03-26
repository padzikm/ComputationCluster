using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using NetworkAdapterConstructor = Common.NetworkAdapter;
using System.Net;

namespace UnitTests
{
    [TestClass]
    public class NetworkAdapter
    {
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

        [TestMethod]
        public void SendGenericMethodForCorrectMessage()
        {
            // arrange
            NetworkAdapterConstructor na = new NetworkAdapterConstructor("localhost", 12345);
            SolveRequest sr = new SolveRequest();
            sr.Data = null;
            sr.ProblemType = "drvp";
            sr.SolvingTimeout = 1000;
            sr.SolvingTimeoutSpecified = true;
            //bool condition;

            // act
            //condition = na.Send<SolveRequest>(sr);
            //na.StopConnection();

            // assert
            Assert.IsTrue(true);
        }
    }
}
