using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKIntroduction;

Console.WriteLine("Hello, Semantic Kernel!");

await BasicSK.Execute();

//await BasicSKChat.Execute();

//Personas agentsExample = new();
//await agentsExample.Execute();

//MinionBusyLight minionBusyLight = new MinionBusyLight();
//await minionBusyLight.Execute();    

//CriticWorkflow agentsWorkflowExample = new();
//await agentsWorkflowExample.Execute();

Console.WriteLine("Type enter to finish");
Console.ReadLine();


