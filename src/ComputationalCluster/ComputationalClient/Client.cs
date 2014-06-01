using System;
using Common;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using DvrpUtils;

namespace ComputationalClient
{

    public class Client
    {
        private readonly NetworkAdapter _networkAdapter;
        private Thread _clientThread;
        private readonly string _problemType;
        private readonly ulong _solvingTimeout;
        private readonly List<byte[]> _solutionsList;
        private readonly byte[] _data;
        private bool _working;
        private const int SleepTime = 30000;

        /// <summary>
        /// Specific constructor for ComputationalClient class. Allows to execute client's correctly. Also creates NetworkAdapter inside. Throws ArgumentNullException when incorrent inputs.
        /// </summary>
        /// <param name="serverName"> String value of a server address name. Cannot be empty. </param>
        /// <param name="port"> Port on which server is listening. Cannot be negative. </param>
        /// <param name="problemType"> String value of name of a problem type. For example 'drvp'. Cannot be empty. </param>
        /// <param name="solvingTimeout"> Time that client wait for solution. After it clients terminates. </param>
        /// <param name="data"> Data that is sent to server. </param>
        public Client(string serverName, int port, string problemType, ulong solvingTimeout, byte[] data)
        {
            _networkAdapter = new NetworkAdapter(serverName, port);

            if(problemType == null || serverName == null || port < 0)
                throw new ArgumentNullException();

            _solutionsList = new List<byte[]>();
            _problemType = problemType;
            _solvingTimeout = solvingTimeout;
            _data = data;
            _working = true;
        }
        
        /// <summary>
        /// This method starts whole Client work. A new thread is created due to not block console. 
        /// </summary>
        /// <returns> True if succed, false otherwise. </returns>
        public bool Start()
        {                   
            if (_clientThread != null)
                throw new InvalidOperationException("Client is already running! Wait for partial or final solution.\n\n");

            try
            {
                _clientThread = new Thread(ClientWork);
                _clientThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Error in client start: {0}", ex.Message);
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
                if (_clientThread != null)
                    _clientThread.Abort();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Error in client stop: {0}", ex.Message);
                return false;
            }
        }  

        private void ClientWork()
        {
            try
            {
                _networkAdapter.StartConnection();

                Console.WriteLine(@"Sending solve request to server.");
                SendSolveRequest();

                Console.WriteLine(@"Solve request sent, waiting for solve request response...");
                var srp = _networkAdapter.Receive<SolveRequestResponse>(false);

                _networkAdapter.CloseConnection();                

                Console.WriteLine(@"Solve request appeared. ID of a task is: {0}", srp.Id);
                
                while (_working)
                    SolutionRequestMessage(srp.Id);
            }
            catch (TimeoutException te)
            {
                Console.WriteLine(@"Computation timeout reached. Closing connection. / {0}", te.Message);
                _working = false;
            }
            catch (SocketException se)
            {
                Console.WriteLine(@"Socket exception appeared. Closing connection. / {0}", se.Message);
                _working = false;
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "Message not valid":
                        Console.WriteLine(@" 'Message not valid' occured - continue processing. / {0}", e.Message);
                        break;
                    case "NetworkStream unavaiable":
                        Console.WriteLine(@" 'NetworkStream unavaiable' occured - you cannot read from stream - continue processing. / {0}", e.Message);
                        break;
                    default:
                        Console.WriteLine(@"Unexpected error. Closing connection. / {0}", e.Message);
                        _working = false;
                        break;
                }
            }
            finally
            {
                if (!_working)
                    Stop(); 
            }
        }

        private void SendSolveRequest()
        {
            var sr = new SolveRequest
            {
                Data = _data,
                ProblemType = _problemType,
                SolvingTimeout = _solvingTimeout,
                SolvingTimeoutSpecified = true
            };

            _networkAdapter.Send(sr, false);
        }

        private void SolutionRequestMessage(ulong id)
        {
            var solutionRequestMessage = new SolutionRequest() {Id = id};

            _networkAdapter.StartConnection();

            _networkAdapter.Send(solutionRequestMessage, false);
            var solutions = _networkAdapter.Receive<Solutions>(false);

            _networkAdapter.CloseConnection();

            if (solutions == null || solutions.Solutions1 == null)
            {
                Thread.Sleep(SleepTime);
                return;
            }

            try
            {
                foreach (var e in solutions.Solutions1)
                {
                    switch (e.Type)
                    {
                        case SolutionsSolutionType.Final:
                            Console.WriteLine(
                                "Final Solutions in da hause - \n" +
                                "Id: {0} \n" +
                                "Problem type: {1} \n" +
                                "It's computations time: {2}\n" +
                                "\nSolution: {3}\n\n" +
                                "Closing connection... \n\n",
                                solutions.Id, solutions.ProblemType, e.ComputationsTime, DataSerialization.BinaryDeserializeObject<double>(e.Data));

                            _working = false;
                            break;
                        case SolutionsSolutionType.Partial:
                            Console.WriteLine(
                                "Partial Solutions in da hause - \n" +
                                "Id: {0} \n" +
                                "Problem type: {1} \n" +
                                "It's computations time: {2}\n" +
                                "Continue to processing... \n\n",
                                solutions.Id, solutions.ProblemType, e.ComputationsTime);

                            Console.WriteLine(@"Partial solution for problem {0} is {1}", id, DataSerialization.BinaryDeserializeObject<double>(e.Data));
                            break;
                        default:
                            Console.WriteLine(   
                                "Id: {0} \n" +
                                "Problem type: {1} \n" +
                                "It's status: {2}, so still waiting for final or partial solutions... \n\n",
                                solutions.Id, solutions.ProblemType, e.Type);
                            break;
                    }

                    if (!_solutionsList.Contains(e.Data))
                        _solutionsList.Add(e.Data);

                    if (e.TimeoutOccured)
                        throw new TimeoutException();
                }
            }
            catch (Exception)
            {
               Console.WriteLine(@"Exception thrown while reading solutions.");
            } 
    
            Thread.Sleep(SleepTime);
        }
    }
}
