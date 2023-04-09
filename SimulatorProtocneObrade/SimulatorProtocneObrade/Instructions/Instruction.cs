using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorProtocneObrade.Instructions
{
    public abstract class Instruction
    {
        protected string Name;
        protected string currentPhase = "IF";
        protected int phaseCycle = 1;
        protected List<Resource> resources = new List<Resource>();
        protected Dictionary<Resource, bool> advancedResources = new();

        public abstract void Execute();
        public void finishCycle()
        {
            phaseCycle--;
        }
        public List<Resource> getResources()
        {
            return resources;
        }
        public Instruction(string name)
        {
            Name = name;
        }
        public void addResource(Resource r1, bool operandType)
        {
            resources.Add(r1);
            advancedResources.Add(r1, operandType);
        }
        public bool getTypeOfResource(Resource r)
        {
            return advancedResources[r];
        }
        public bool canExecute()
        {
            bool canExecute = true;
            foreach (Resource resource in resources)
            {
                if (resource.isTaken())
                {
                    canExecute = false;
                    break;
                }
            }
            return canExecute;
        }
        public int getPhaseCycle()
        {
            return phaseCycle;
        }
        public abstract void changePhase(string phase);
        public string getName()
        {
            return this.Name;
        }
        public void takeAllResources()
        {
            foreach (Resource resource in resources)
            {
                resource.takeResource();
            }
        }
        public void releaseAllResources()
        {
            foreach (Resource resource in resources)
            {
                resource.releaseResource();
            }
        }
    }
}
