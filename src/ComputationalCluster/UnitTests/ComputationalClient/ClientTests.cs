using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using ComputationalClient;
using Common;

namespace UnitTests
{
    [TestClass]
    public class ClientTests
    {
        #region Starting and Stoping client's threads
        [TestMethod]
        public void StartWhileClientNotExist()
        {
            // arrange
            Client cc = new Client("localhost", 12345, "dvrp", 10000, null);
            bool condition;
            
            // act
            condition = cc.Start();

            // assert
            Assert.IsTrue(condition);
        }

        [TestMethod]
        
        public void StopWorkingOfClientThreadsWhileThreadNotExist()
        {
            // arrange
            Client cc = new Client("localhost", 12345, "dvrp", 10000, null);

            // act
            cc.Stop();

            // assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void StopWorkingOfClientThreadsWhileThreadExist()
        {
            // arrange
            Client cc = new Client("localhost", 12345, "dvrp", 10000, null);
            bool condition;

            // act
            cc.Start();
            condition = cc.Stop();

            // assert
            Assert.IsTrue(condition);
        }
        #endregion

        #region CreatingClient tests
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateClientWithIncorrectValueOfServerName()
        {
            // arrange
            Client cc = new Client(null, 12345, "dvrp", 10000, null);

            // act

            // assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateClientWithIncorrectValueOfPort()
        {
            // arrange
            Client cc = new Client("localhost", -3, "dvrp", 10000, null);

            // act

            // assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateClientWithIncorrectValueOfProblemName()
        {
            // arrange
            Client cc = new Client("localhost", 12345, null, 10000, null);

            // act

            // assert
        }

        [TestMethod]
        public void CreateClientWithCorrectValue()
        {
            // arrange
            Client cc = new Client("localhost", 12345, "dvrp", 10000, null);

            // act

            // assert
            Assert.IsTrue(true);
        }
        #endregion
    }
}
