// See https://aka.ms/new-console-template for more information
using SimulatorProtocneObrade;
using SimulatorProtocneObrade.Instructions;
using SimulatorProtocneObrade.Pipeline;
using System.Resources;
using System.Runtime.CompilerServices;

PipeLineController ppp = new PipeLineController(true);
List<Resource> resources = new List<Resource>();
resources.Add(new Resource("prvi"));
resources.Add(new Resource("drugi"));
AddInstruction s0 = new AddInstruction("0");
MulInstruction s1 = new MulInstruction("1");
AddInstruction s2 = new AddInstruction("2");
s0.addResource(resources[0], true);
s1.addResource(resources[0], false);
s2.addResource(resources[0], false);
//AddInstruction s3 = new AddInstruction("3");
ppp.registerInstruction(s0);
ppp.registerInstruction(s1);
ppp.registerInstruction(s2);
//ppp.registerInstruction(s3, true);
ppp.pipelineActivate();
