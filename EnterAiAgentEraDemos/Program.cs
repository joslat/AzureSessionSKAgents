using EnterAiAgentEraDemos;;

Console.WriteLine("Agents rule!!");


AgentCraftingMinion agentsExample = new();
//AgentCraftingHistory agentsExample = new();
//AgentCreatorCritic agentsExample = new();

await agentsExample.Execute();

Console.WriteLine("Type enter to finish");
Console.ReadLine();