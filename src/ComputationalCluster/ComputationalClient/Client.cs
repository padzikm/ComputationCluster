using System;
using Common;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ComputationalClient
{
    public class Client
    {
        private NetworkAdapter networkAdapter;
        private Thread clientThread;
        private string problemType;
        private ulong solvingTimeout;
        private List<byte[]> solutionsList;
        private byte[] data;
        private bool working;
        private const int sleepTime = 30000;

        /// <summary>
        /// Specific constructor for ComputationalClient class. Allows to execute client's correctly. Also creates NetworkAdapter inside. Throws ArgumentNullException when incorrent inputs.
        /// </summary>
        /// <param name="serverName"> String value of a server address name. Cannot be empty. </param>
        /// <param name="port"> Port on which server is listening. Cannot be negative. </param>
        /// <param name="_problemType"> String value of name of a problem type. For example 'drvp'. Cannot be empty. </param>
        /// <param name="_solvingTimeout"> Time that client wait for solution. After it clients terminates. </param>
        /// <param name="_data"> Data that is sent to server. </param>
        public Client(string serverName, int port, string _problemType, ulong _solvingTimeout, byte[] _data)
        {
            networkAdapter = new NetworkAdapter(serverName, port);

            if(_problemType == null || serverName == null || port < 0)
                throw new ArgumentNullException();

            solutionsList = new List<byte[]>();
            problemType = _problemType;
            solvingTimeout = _solvingTimeout;
            data = _data;
            working = true;
        }
        
        /// <summary>
        /// This method starts whole Client work. A new thread is created due to not block console. 
        /// </summary>
        /// <returns> True if succed, false otherwise. </returns>
        public bool Start()
        {                   
            if (clientThread != null)
                throw new InvalidOperationException("Client is already running! Wait for partial or final solution.\n\n");

            try
            {
                clientThread = new Thread(ClientWork);
                clientThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client start: {0}\n\n", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Allow to terminate correctly program execution. Manages ClientWork thread.
        /// </summary>
        public bool Stop()
        {
            try
            {
                if (clientThread != null)
                    clientThread.Abort();
                clientThread = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client stop: {0}\n\n", ex.Message);
                return false;
            }
        }  

        private void ClientWork()
        {
            try
            {
                networkAdapter.StartConnection();

                Console.WriteLine("Sending solve request to server.\n\n");
                SendSolveRequest();

                Console.WriteLine("Solve request sent, waiting for solve request response...\n\n");
                SolveRequestResponse srp = networkAdapter.Receive<SolveRequestResponse>(false);

                networkAdapter.CloseConnection();                

                Console.WriteLine("Solve request appeared. ID of a task is: {0}\n\n", srp.Id);
                
                while (working)
                    SolutionRequestMessage(srp.Id);
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("Computation timeout reached. Closing connection. / {0}\n\n", te.Message);
                working = false;
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket exception appeared. Closing connection. / {0}\n\n", se.Message);
                working = false;
            }
            catch (Exception e)
            {
                if (e.Message == "Message not valid")
                {
                    Console.WriteLine(" 'Message not valid' occured - continue processing. / {0}\n\n", e.Message);
                }
                else if (e.Message == "NetworkStream unavaiable")
                {
                    Console.WriteLine(" 'NetworkStream unavaiable' occured - you cannot read from stream - continue processing. / {0}\n\n", e.Message);
                }
                else
                {
                    Console.WriteLine("Unexpected error. Closing connection. / {0}\n\n", e.Message);
                    working = false;
                }
            }
            finally
            {
                if (!working)
                    Stop(); 
            }
        }

        private void SendSolveRequest()
        {
            SolveRequest sr = new SolveRequest();
            sr.Data = data;
            sr.ProblemType = problemType;
            sr.SolvingTimeout = solvingTimeout;
            sr.SolvingTimeoutSpecified = true;

            networkAdapter.Send<SolveRequest>(sr, false);
        }

        private void SolutionRequestMessage(ulong id)
        {
            SolutionRequest solutionRequestMessage = new SolutionRequest() {Id = id};

            networkAdapter.StartConnection();

            networkAdapter.Send<SolutionRequest>(solutionRequestMessage, false);
            Solutions solutions = networkAdapter.Receive<Solutions>(false);

            networkAdapter.CloseConnection();

            foreach(var e in solutions.Solutions1)
            {
                if(e.Type == SolutionsSolutionType.Final)
                {
                    Console.WriteLine(
                        "Final Solutions in da hause - \n" +
                        "Id: {0} \n" +
                        "Problem type: {1} \n" +
                        "It's computations time: {2}\n" +
                        "Closing connection... \n\n",
                        solutions.Id, solutions.ProblemType, e.ComputationsTime);

                    working = false;
                }
                else if(e.Type == SolutionsSolutionType.Partial)
                { 
                    Console.WriteLine(
                        "Partial Solutions in da hause - \n" +
                        "Id: {0} \n" +
                        "Problem type: {1} \n" +
                        "It's computations time: {2}\n" +
                        "Continue to processing... \n\n",
                        solutions.Id, solutions.ProblemType, e.ComputationsTime);
                }
                else
                {
                    Console.WriteLine(
                        "Solutions in da hause - \n" +
                        "Id: {0} \n" +
                        "Problem type: {1} \n"+
                        "It's status: {2}, so still waiting for final or partial solutions... \n\n", 
                        solutions.Id, solutions.ProblemType, e.Type);
                }                

                if (!solutionsList.Contains(e.Data))
                    solutionsList.Add(e.Data);

                if (e.TimeoutOccured)
                    Console.WriteLine("Unfortunately timeout occured. Closing connection \n\n");   
            }
    
            Thread.Sleep(sleepTime);
        }
    }
}
