using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TaskManager
{
    public class TaskManager : NetworkAdapter
    {
        public void RegisterResponse()
        {
            //TODO fill Register with data
            var registerResponseMessage = new RegisterResponse();
            Send<RegisterResponse>(registerResponseMessage);
        }

        public void DivideProblem()
        {

        }

    }
}
