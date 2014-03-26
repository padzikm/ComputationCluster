using System;
using Common;
using System.Threading;
using System.Net.Sockets;

namespace ComputationalClient
{
    class ComputationalClient
    {
        private NetworkAdapter networkAdapter;
        private Thread askForSolutionThread;
        private Thread clientThread;
        private string problemType;
        private ulong solvingTimeout;
        private byte[] data;
        private bool working;
        private const int sleepTime = 30000;

        /// <summary>
        /// Specific constructor for ComputationalClient class. Allows to execute client's correctly.
        /// </summary>
        /// <param name="_problemType"> String value of name of a problem type. For example 'drvp'. </param>
        /// <param name="_solvingTimeout"> Time that client wait for solution. After it clients terminates. </param>
        /// <param name="_data"> Data that is sent to server. </param>
        public ComputationalClient(string _problemType, ulong _solvingTimeout, byte[] _data)
        {
            networkAdapter = new NetworkAdapter();
            if(_problemType == null)
                throw new ArgumentNullException();
            
            problemType = _problemType;
            solvingTimeout = _solvingTimeout;
            data = _data;
            working = true;
        }

        /// <summary>
        /// This method starts whole Client work. A new thread is created due to not block console. Uses StartConnection method 
        /// from NetworkAdapter class.
        /// </summary>
        /// <param name="server"> Specifies string value of IP which client use to connect to server. May be localhost. </param>
        /// <param name="port"> Port that server is listening to. </param>
        public void Start(string server, int port)
        {                   
            if (clientThread != null)
                throw new InvalidOperationException("Client is already running! Wait for partial or final solution.");

            try
            {
                clientThread = new Thread(ClientWork);
                networkAdapter.StartConnection(server, port);
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client start: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Allow to terminate correctly program execution. Manages ClientWork thread and askForSolution thread.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (askForSolutionThread != null)
                {
                    askForSolutionThread.Abort();
                    askForSolutionThread = null;
                }
                clientThread.Join();
                clientThread = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client stop: {0}", ex.Message);
            }
            finally
            {
                networkAdapter.CloseConnection();
            }
        }  

        // ClientWork działa tak, że  TYLKO RAZ wysyła wiadomość do server, że ma do policzenia problem, a w pętli oczekuje na kolejne rozwiązania. Jest to po to, żeby móc otrzymywać częściowe rozwiązania, oraz finalne. Finalne powinno zawierać jakąś informację, że jest finalne (a nie zawiera?), żeby móc skończyć kolejny obieg czekania na rozwiązania.
        private void ClientWork()
        {
            try
            {
                Console.WriteLine("Sending solve request to server.\n");
                SendSolveRequest();

                Console.WriteLine("Solve request sent, waiting for solve request response...\n");
                SolveRequestResponse srp = networkAdapter.Recieve<SolveRequestResponse>();

                Console.WriteLine("Solve request appeared. ID of a task is: {0}\n", srp.Id);
                SolutionRequestMessage(srp.Id);

                while (working)
                {
                    Console.WriteLine("Another thread is asking for solutions, waiting till some solutions show up...\n");
                    Solutions solutions = networkAdapter.Recieve<Solutions>();

                    Console.WriteLine("Solutions in da hause -\n id: {0}\n problem type: {1}\n Closing connection...", solutions.Id, solutions.ProblemType);
                }

                working = false;
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("Computation timeout reached. Closing connection. / {0}", te.Message);
                working = false;
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket exception appeared. Closing connection. / {0}", se.Message);
                working = false;
            }
            catch (Exception e)
            {
                if (e.Message == "Message not valid")
                {
                    Console.WriteLine(" 'Message not valid' occured - continue processing. / {0}", e.Message);
                }
                else
                {
                    Console.WriteLine("Unexpected error. Closing connection. / {0}", e.Message);
                    working = false;
                }
            }
            finally
            {
                if (!working)
                {
                    //loopingThread.Abort();
                    //loopingThread = null;
                    clientThread.Abort();
                    clientThread = null;
                    networkAdapter.CloseConnection();
                }
            }
        }

        private void SendSolveRequest()
        {
            SolveRequest sr = new SolveRequest();
            sr.Data = data;
            sr.ProblemType = problemType;
            sr.SolvingTimeout = solvingTimeout;
            sr.SolvingTimeoutSpecified = true;

            networkAdapter.Send<SolveRequest>(sr);
        }

        private void SolutionRequestMessage(ulong id)
        {
            askForSolutionThread = new Thread(() =>
            {
                while (true)
                {
                    SolutionRequest solutionRequestMessage = new SolutionRequest();
                    solutionRequestMessage.Id = id;

                    if (!networkAdapter.Send<SolutionRequest>(solutionRequestMessage))
                        throw new TimeoutException();

                    Thread.Sleep(sleepTime);
                }
            });
            askForSolutionThread.Start();
        }
    }
}
