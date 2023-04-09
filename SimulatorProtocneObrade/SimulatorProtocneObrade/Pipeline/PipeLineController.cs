using SimulatorProtocneObrade.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorProtocneObrade.Pipeline
{
    public class PipeLineController
    {
        bool operandForwarding;
        public List<Instruction> listOfAllInstructions = new();
        public Queue<Instruction> fetchBuffer = new();
        public Queue<Instruction> decodeBuffer = new();
        public Queue<Instruction> executeBuffer = new();
        public Queue<Instruction> memBuffer = new();
        public Queue<Instruction> wbBuffer = new();
        List<List<string>> table = new List<List<string>>();
        public PipeLineController(bool operandForwarding)
        {
            this.operandForwarding = operandForwarding;
            
        }
        public void addEmpty(int currCycle)
        {
            foreach (var arr in this.table)
                arr.Insert(currCycle, "");
        }
        public void printTable(int currCycle)
        {
            Console.Write(String.Format("{0,-10}", "Cycle"));
            for (int i = 1; i < currCycle+1; i++)
            {
                Console.Write(String.Format("{0,-5}", i));
            }
            Console.WriteLine();
            foreach (var arr in table)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    if (i == 0)
                    {
                        Console.Write(String.Format("{0,-10}", arr[i]));
                    }
                    else
                    {
                        Console.Write(String.Format("{0,-5}", arr[i]));
                    }
                }
                Console.WriteLine();
            }
        }
        public void registerInstruction(Instruction instruction)
        {
            listOfAllInstructions.Add(instruction);
            fetchBuffer.Enqueue(instruction);
            //this.operandForwarding = operandForwarding;
            int index = int.Parse(instruction.getName());
            table.Add(new List<string>());
            table[index].Add(instruction.getName());

        }
        public void fetchInstruction(int cycle)
        {
            Instruction nextInstruction; bool status = fetchBuffer.TryPeek(out nextInstruction);
            if (status)
            {
                //pm.putInput(nextInstruction.getName(),"IF",cycleNumber);
                Console.WriteLine("Instruction fetched:" + nextInstruction.getName());
                nextInstruction.finishCycle();
                int phaseCycle = nextInstruction.getPhaseCycle();
                if (phaseCycle == 0)
                {
                    fetchBuffer.Dequeue();
                    nextInstruction.changePhase("ID");
                    decodeBuffer.Enqueue(nextInstruction);
                }
                int index = int.Parse(nextInstruction.getName());
                table[index].Insert(cycle, "IF");
            }

        }
        public void decodeInstruction(int cycleNumber)
        {

            //this.fetchInstruction();
            Instruction nextInstruction; bool status = decodeBuffer.TryPeek(out nextInstruction);
            if (status)
            {
                bool canExecute = nextInstruction.canExecute();
                if (canExecute)
                {
                    //pm.putInput(nextInstruction.getName(), "ID", currentCycleNumber);
                    Console.WriteLine("Instruction decoded:" + nextInstruction.getName());
                    nextInstruction.takeAllResources();
                    nextInstruction.finishCycle();
                    int phaseCycle = nextInstruction.getPhaseCycle();
                    if (phaseCycle == 0)
                    {
                        decodeBuffer.Dequeue();
                        nextInstruction.changePhase("EX");
                        executeBuffer.Enqueue(nextInstruction);
                    }
                    int index = int.Parse(nextInstruction.getName());
                    table[index].Insert(cycleNumber, "ID");
                }
                

            }
            this.fetchInstruction(cycleNumber);

        }
        public void executeInstruction(int cycleNumber)
        {
            bool canRelease = false;
            // this.decodeInstruction();
            Instruction nextInstruction; bool status = executeBuffer.TryPeek(out nextInstruction);
            if (status)
            {
                //pm.putInput(nextInstruction.getName(), "EX", currentCycleNumber);
                Console.WriteLine("Instruction executing:" + nextInstruction.getName());
                nextInstruction.finishCycle();
                int phaseCycle = nextInstruction.getPhaseCycle();
                if (phaseCycle == 0)
                {
                    executeBuffer.Dequeue();
                    nextInstruction.changePhase("MEM");
                    memBuffer.Enqueue(nextInstruction);

                    canRelease = true;

                }
                int index = int.Parse(nextInstruction.getName());
                table[index].Insert(cycleNumber, "EX");
            }
            this.decodeInstruction(cycleNumber);
            if (operandForwarding && nextInstruction != null && canRelease)
            {

                nextInstruction.releaseAllResources();
                Instruction nextInstruction2; bool status2 = decodeBuffer.TryPeek(out nextInstruction2);
                if (status2)
                {
                    //One that is using
                    var list1 = nextInstruction.getResources();
                    //One that is waiting
                    var list2 = nextInstruction2.getResources();
                    var intersection = list1.Intersect(list2);
                    Resource r = intersection.ElementAt(0);
                    bool executingStatus = nextInstruction.getTypeOfResource(r);
                    bool waitingStatus = nextInstruction2.getTypeOfResource(r);
                    if (executingStatus && !waitingStatus)
                    {
                        Console.WriteLine("Prevented potential RAW Hazard");
                    }
                    else if (!executingStatus && waitingStatus)
                    {
                        Console.WriteLine("Prevented potential WAR Hazard");
                    }
                    else if (executingStatus && waitingStatus)
                    {
                        Console.WriteLine("Prevented potential WAW Hazard");
                    }
                    Console.WriteLine("Instruction " + nextInstruction.getName() + " released resources for instruction:" + nextInstruction2.getName());

                }
            }
        }
        public void memInstruction(int cycleNumber)
        {
            Instruction nextInstruction; bool status = memBuffer.TryPeek(out nextInstruction);
            if (status)
            {
                //pm.putInput(nextInstruction.getName(), "MEM", currentCycleNumber);
                Console.WriteLine("Instruction mem-accessing:" + nextInstruction.getName());
                nextInstruction.finishCycle();
                int phaseCycle = nextInstruction.getPhaseCycle();
                if (phaseCycle == 0)
                {
                    memBuffer.Dequeue();
                    nextInstruction.changePhase("WB");
                    wbBuffer.Enqueue(nextInstruction);
                }
                int index = int.Parse(nextInstruction.getName());
                table[index].Insert(cycleNumber, "MEM");
            }
            this.executeInstruction(cycleNumber);
        }
        public void writeBackInstruction(int cycleNumber)
        {
            // this.memInstruction();
            Instruction nextInstruction; bool status = wbBuffer.TryPeek(out nextInstruction);
            if (status)
            {
                //pm.putInput(nextInstruction.getName(), "WB", currentCycleNumber);
                Console.WriteLine("Instruction writeB:" + nextInstruction.getName());
                nextInstruction.finishCycle();
                int phaseCycle = nextInstruction.getPhaseCycle();
                if (phaseCycle == 0)
                {
                    wbBuffer.Dequeue();
                    this.listOfAllInstructions.Remove(nextInstruction);

                }
                int index = int.Parse(nextInstruction.getName());
                table[index].Insert(cycleNumber, "WB");
            }
            this.memInstruction(cycleNumber);
            if (!operandForwarding && nextInstruction != null)
            {
                nextInstruction.releaseAllResources();
                Instruction nextInstruction2; bool status2 = decodeBuffer.TryPeek(out nextInstruction2);
                if (status2)
                {
                    //One that is using
                    var list1 = nextInstruction.getResources();
                    //One that is waiting
                    var list2 = nextInstruction2.getResources();
                    var intersection = list1.Intersect(list2);
                    Resource r = intersection.ElementAt(0);
                    bool executingStatus = nextInstruction.getTypeOfResource(r);
                    bool waitingStatus = nextInstruction2.getTypeOfResource(r);
                    if (executingStatus && !waitingStatus)
                    {
                        Console.WriteLine("Prevented potential RAW Hazard");
                    }
                    else if (!executingStatus && waitingStatus)
                    {
                        Console.WriteLine("Prevented potential WAR Hazard");
                    }
                    else if (!executingStatus && !waitingStatus)
                    {
                        Console.WriteLine("Prevented potential WAW Hazard");
                    }
                    //Console.WriteLine("Instruction " + nextInstruction.getName() + " released resources for instruction:" + nextInstruction2.getName());

                }
            }

        }
        public void pipelineActivate()
        {
            addEmpty(1);
            Console.WriteLine("CIKLUS 1:");
            this.fetchInstruction(1);
            addEmpty(2);
            Console.WriteLine("CIKLUS 2:");
            this.decodeInstruction(2);
            addEmpty(3);
            Console.WriteLine("CIKLUS 3:");
            this.executeInstruction(3);
            addEmpty(4);
            Console.WriteLine("CIKLUS 4:");
            this.memInstruction(4);
            addEmpty(5);
            Console.WriteLine("CIKLUS 5:");
            this.writeBackInstruction(5);
            int i = 5;
            while (this.listOfAllInstructions.Count != 0)
            {
                
                Console.WriteLine("CIKLUS " + i++);
                addEmpty(i);
                this.writeBackInstruction(i);
            }
            Console.WriteLine();
            this.printTable(i);
            //this.pm.printMatrix();

        }
    }
}
