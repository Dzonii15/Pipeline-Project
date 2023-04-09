using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorProtocneObrade.Instructions
{
    public class StoreInstruction : Instruction
    {
        public StoreInstruction(string name) : base(name)
        {
        }
        public override void changePhase(string phase)
        {
            switch (phase)
            {
                case "ID":
                    this.currentPhase = "ID";
                    this.phaseCycle = 1;
                    break;
                case "EX":
                    this.currentPhase = "EX";
                    this.phaseCycle = 1;
                    break;
                case "MEM":
                    this.currentPhase = "MEM";
                    this.phaseCycle = 3;
                    break;
                case "WB":
                    this.currentPhase = "WB";
                    this.phaseCycle = 1;
                    break;
            }
        }

        public override void Execute()
        {
            Console.WriteLine("ID:" + this.Name + " Phase:" + this.currentPhase);
        }

    }
}
