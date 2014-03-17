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
        private ulong id;
        private DateTime time;
        private StatusThread[] threads;
        private bool recieve = false;
        private Thread sender;
        private bool stop = false;

        public ulong ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public DateTime Time
        {
            get { return this.time; }
            set { this.time = value; }
        }

        public StatusThread[] Threads
        {
            get { return this.threads; }
            set { this.threads = value; }
        }

        public void Satrt()
        {
            // bool recieve=false;

            Register();

            while (!recieve)
            {
                RegisterResponse();
            }

            try
            {

                sender = new Thread(sendStatus);
                sender.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in node status: {0}", ex.Message);
            }

            //TODO work

        }

        public void sendStatus()
        {
            while (!stop)
            {
                Status();

                // TODO time to sleep numbers of threads

            }
        }

        public void Register()
        {
            var registerMessage = new Register();
            registerMessage.Type = RegisterType.ComputationalNode;
            registerMessage.SolvableProblems = new string[] { "problem A", "problem B" };
            registerMessage.ParallelThreads = (byte)5;
            Send<Register>(registerMessage);
        }

        public void RegisterResponse()
        {
            
            var registerResponseMessage = Recieve<RegisterResponse>();
            if (registerResponseMessage != null)
                recieve = true;
            ID = registerResponseMessage.Id;
            Time = registerResponseMessage.Timeout;

        }

        public void Status()
        {
            var statusMessage = new Status();
            statusMessage.Id = ID;
            statusMessage.Threads = Threads;
            Send<Status>(statusMessage);
        }

        public void PartialProblems()
        {
            
            var partialProblemsMaessage = Recieve<SolvePartialProblems>();
            //TODO take message

        }

        public void Solution()
        {
            var solutionMessage = new Solutions();
            //TODO fill message
            Send<Solutions>(solutionMessage);


        }




    }
}
