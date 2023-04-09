using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorProtocneObrade
{
    public class Resource
    {
        bool taken = false;
        string Name;
        public Resource(string Name)
        {
            this.Name = Name;
        }
        public void takeResource()
        {
            this.taken = true;
        }
        public void releaseResource()
        {
            this.taken = false;
        }
        public bool isTaken()
        {
            return taken;
        }
    }
}
