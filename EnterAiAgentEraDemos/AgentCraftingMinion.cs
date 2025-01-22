using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Experimental.Agents;


namespace EnterAiAgentEraDemos;


#pragma warning disable SKEXP0101
public class AgentCraftingMinion
{

    // Track agents for clean-up
    readonly List<IAgent> _agents = new();

    IAgentThread? _agentsThread = null;
    public async Task Execute()
    {
        var openAIFunctionEnabledModelId = "gpt-4o";
        var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_KEY_01");
        var userMessage = "";
        var pathToPlugin = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Agents", "MinionAgent.yaml");
        string agentDefinition = File.ReadAllText(pathToPlugin);
        var minionAgent = await new AgentBuilder()
            .WithOpenAIChatCompletion(openAIFunctionEnabledModelId, openAIApiKey)
            .FromTemplatePath(pathToPlugin)
            .BuildAsync();
        _agents.Add(minionAgent);
        _agentsThread = await minionAgent.NewThreadAsync();

        while (true)
        {
            Console.WriteLine("Enter a message to send to the agent or type 'exit' to quit:");
            userMessage = Console.ReadLine();
            if (userMessage == "exit")
            {
                Console.WriteLine($"Banana!!");
                break;
            }

            var responseMessages = await _agentsThread.InvokeAsync(minionAgent, userMessage).ToArrayAsync();
            DisplayMessages(responseMessages, minionAgent);
        }

        await CleanUpAsync();
        Console.WriteLine($"=============================================================================");
    }
    private IAgent Track(IAgent agent)
    {
        _agents.Add(agent);

        return agent;
    }

    private void DisplayMessages(IEnumerable<IChatMessage> messages, IAgent? agent = null)
    {
        foreach (var message in messages)
        {
            DisplayMessage(message, agent);
        }
    }

    private void DisplayMessage(IChatMessage message, IAgent? agent = null)
    {
        //Console.WriteLine($"[{message.Id}]");
        if (agent != null)
        {
            if (message.Role != "user")
                Console.WriteLine($"# {message.Role}: ({agent.Name}) {message.Content}");
        }
        else
        {
            Console.WriteLine($"# {message.Role}: {message.Content}");
        }
    }

    private async Task CleanUpAsync()
    {
        Console.WriteLine("🧽 Cleaning up ...");

        if (_agentsThread != null)
        {
            Console.WriteLine("Thread going away ...");
            _agentsThread.DeleteAsync();
            _agentsThread = null;
        }

        if (_agents.Any())
        {
            Console.WriteLine("Agents going away ...");
            await Task.WhenAll(_agents.Select(agent => agent.DeleteAsync()));
            _agents.Clear();
        }
    }
}
