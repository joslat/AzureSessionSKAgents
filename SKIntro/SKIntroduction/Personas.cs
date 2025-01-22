using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace SKIntroduction;

#pragma warning disable SKEXP0101

public class Personas
{
    // Track agents for clean-up
    readonly List<IAgent> _agents = new();

    IAgentThread? _agentsThread = null;
    public async Task Execute()
    {
        var modelDeploymentName = "gpt-4o";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");
        var openAIFunctionEnabledModelId = "gpt-4o";
        var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_APIKEY");
        var userMessage = "";
        var pathToPlugin = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "MinionAgent.yaml");
        string agentDefinition = File.ReadAllText(pathToPlugin);

        // Azure instead of OpenAI
        var minionAgent = await new AgentBuilder()
            .WithAzureOpenAIChatCompletion(
               model: modelDeploymentName,
               endpoint: azureOpenAIEndpoint,
               apiKey: azureOpenAIApiKey)
            .FromTemplatePath(pathToPlugin)
            .BuildAsync();

        // Uncomment the following code to use OpenAI instead of Azure
        //var minionAgent = await new AgentBuilder()
        //    .WithOpenAIChatCompletion(openAIFunctionEnabledModelId, openAIApiKey)
        //    .FromTemplatePath(pathToPlugin)
        //    .BuildAsync();

        _agents.Add(minionAgent);
        _agentsThread = await minionAgent.NewThreadAsync();
        Console.WriteLine("Enter a message to send to the agent or type 'exit' to quit:");

        while (true)
        {
            Console.Write("User: ");
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
            {
                if (message.Role == "assistant")
                {
                    Console.WriteLine($"Minion: {message.Content}");
                }
                else 
                    Console.WriteLine($"# {message.Role}: ({agent.Name}) {message.Content}");
            }               
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
