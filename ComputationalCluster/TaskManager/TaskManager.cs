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
        public void Register()
        {
            //TODO fill Register with data
            var registerMessage = new Register();
            Send<Register>(registerMessage);
        }

        public 

    }
}
