using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;
using System.Net.Sockets;

namespace ComputationalClient
{
    class ComputationalClient: NetworkAdapter
    {
        private Thread loopingThread;
        private string problemType;
        private ulong solvingTimeout;
        private byte[] data;
        private const int sleepTime = 3000;

        public ComputationalClient(string _problemType, ulong _solvingTimeout, byte[] _data)
        {
            problemType = _problemType;
            solvingTimeout = _solvingTimeout;
            data = _data;
        }

        public void ClientWork(string server)
        {
            StartConnection(server);            
            try
            {
                SolveRequest sr = new SolveRequest();
                sr.Data = data;
                sr.ProblemType = problemType;
                sr.SolvingTimeout = solvingTimeout;
                sr.SolvingTimeoutSpecified = true;

                Send<SolveRequest>(sr);

                SolveRequestResponse srp;
                do
                {
                    srp = Recieve<SolveRequestResponse>();
                }
                while (srp == null);

                SolutionRequestMessage(srp.Id);

                Solutions solutions;
                do
                {
                    solutions = Recieve<Solutions>();
                }
                while (solutions == null);

                Console.WriteLine("Solutions appeared. Result in a particulary folder. Closing connection");
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("Computation timeout reached. Closing connection. / {0}", te.Message);
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket exception appeared. Closing connection. / {0}", se.Message);
            }
            catch (Exception e)
            {
                if (e.Message == "Message not valid")
                {
                    Console.WriteLine(" 'Message not valid' occured - continue processing. / {0}", e.Message); // TODO: continue processing
                }
            }
            finally
            {
                loopingThread.Join();
                CloseConnection();
            }
        }

        public void SolutionRequestMessage(ulong id)
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
