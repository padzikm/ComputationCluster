using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Common;

namespace ComputationalNode
{
    class ComputationalNode : NetworkAdapter
    {
        private Thread nodeThread;
        private Thread loopingThread;
        private DateTime time;
        private StatusThread[] threads;
        private bool working;
        private ulong id;
        private ulong problemId;

        public ComputationalNode(int _port)
        {
            port = _port;
            working = true;
        }

        public void Start(string server)
        {

            if (nodeThread != null)
                throw new InvalidOperationException("Node is already running!");

            try
            {
                nodeThread = new Thread(NodeWork);
                StartConnection(server, port);
                nodeThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in node start: {0}", e.Message);
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
                nodeThread.Abort();
                nodeThread = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in node stop: {0}", e.Message);
            }
            finally
            {
                CloseConnection();
            }

        }

        private void NodeWork()
        {
            try
            {
                Register();
                RegisterResponse();
                Status();
            }
            catch
            {

            }

            while (working)
            {
                try
                {
                    PartialProblems();
                    //TODO solve problem, new thread
                    Solution();
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
                        nodeThread.Abort();
                        nodeThread = null;
                        CloseConnection();
                    }
                }
            }
        }

        private void Register()
        {
            var registerMessage = new Register();
            registerMessage.Type = RegisterType.ComputationalNode;
            registerMessage.SolvableProblems = new string[] { "problem A", "problem B" };
            registerMessage.ParallelThreads = (byte)5;
            Send<Register>(registerMessage);
        }

        private void RegisterResponse()
        {           
            var registerResponseMessage = Recieve<RegisterResponse>();         
            id = registerResponseMessage.Id;
            time = registerResponseMessage.Timeout;
        }

        private void Status()
        {
            loopingThread = new Thread(() =>
                {
                    while (true)
                    {
                        var statusMessage = new Status();
                        statusMessage.Id = id;
                        statusMessage.Threads = threads;

                        if (!Send<Status>(statusMessage))
                            throw new Exception("StartKeepAlive");

                        Thread.Sleep(time.Millisecond);
                    }
                });
            loopingThread.Start();
        }

        private void PartialProblems()
        {
            try
            {
                var partialProblemsMaessage = Recieve<SolvePartialProblems>();
                problemId = partialProblemsMaessage.Id;
            }
            catch(Exception e)
            {

            }
        }

        private void Solution()
        {
            var solutionMessage = new Solutions();
            solutionMessage.ProblemType = "DVRT";
            solutionMessage.Id = problemId;
            //TODO fill message
            Send<Solutions>(solutionMessage);


        }




    }
}
