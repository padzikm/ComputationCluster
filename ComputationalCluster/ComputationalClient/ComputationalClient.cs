using System;
using Common;
using System.Threading;
using System.Net.Sockets;

namespace ComputationalClient
{
    class ComputationalClient : NetworkAdapter
    {
        private Thread loopingThread;
        private Thread clientThread;
        private string problemType;
        private ulong solvingTimeout;
        private byte[] data;
        private bool working;
        private const int sleepTime = 3000;

        public ComputationalClient(string _problemType, ulong _solvingTimeout, byte[] _data, int _port)
        {
            problemType = _problemType;
            solvingTimeout = _solvingTimeout;
            data = _data;
            port = _port;
            working = true;
        }

        public void Start(string server)
        {                   
            if (clientThread != null)
                throw new InvalidOperationException("Client is already running! Wait for partial or final solution.");

            try
            {
                clientThread = new Thread(ClientWork);
                StartConnection(server, port);
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client start: {0}", ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (loopingThread != null)
                {
                    loopingThread.Abort();
                    loopingThread = null;
                }
                clientThread.Abort();
                clientThread = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in client stop: {0}", ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void SendSolveRequest()
        {
            SolveRequest sr = new SolveRequest();
            sr.Data = data;
            sr.ProblemType = problemType;
            sr.SolvingTimeout = solvingTimeout;
            sr.SolvingTimeoutSpecified = true;

            Send<SolveRequest>(sr);
        }

        private void ClientWork()
        {
            while (working)
            {
                try
                {
                    Console.WriteLine("1\n");
                    SendSolveRequest();
                    Console.WriteLine("2\n");
                    SolveRequestResponse srp = Recieve<SolveRequestResponse>();
                    Console.WriteLine("3\n");
                    SolutionRequestMessage(srp.Id);
                    Console.WriteLine("4\n");
                    Solutions solutions = Recieve<Solutions>();
                    Console.WriteLine("5\n");
                    Console.WriteLine("Solutions appeared. Result in a particulary folder. Closing connection.");

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
                        CloseConnection();
                    }
                }
            }
        }

        private void SolutionRequestMessage(ulong id)
        {
            loopingThread = new Thread(() =>
            {
                while (true)
                {
                    SolutionRequest solutionRequestMessage = new SolutionRequest();
                    solutionRequestMessage.Id = id;

                    if (!Send<SolutionRequest>(solutionRequestMessage))
                        throw new TimeoutException();

                    Thread.Sleep(sleepTime);
                }
            });
            loopingThread.Start();
        }
    }
}
