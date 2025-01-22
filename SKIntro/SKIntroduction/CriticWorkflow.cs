using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace SKIntroduction;

#pragma warning disable SKEXP0101

public class CriticWorkflow
{
    // Track agents for clean-up
    readonly List<IAgent> _agents = new();

    IAgentThread? _agentsThread = null;
    public async Task Execute()
    {
        var modelDeploymentName = "gpt-4o";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        //var openAIFunctionEnabledModelId = "gpt-4o";
        //var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_APIKEY");
        var userMessage = "";

        var creatorAgent =
            Track(
                await new AgentBuilder()
//                  .WithOpenAIChatCompletion(openAIFunctionEnabledModelId, openAIApiKey)
                    .WithAzureOpenAIChatCompletion(
                       model: modelDeploymentName,
                       endpoint: azureOpenAIEndpoint,
                       apiKey: azureOpenAIApiKey)
                    .WithInstructions("You are a marketing writer, creator, agent.  you are tasked with generating a marketing brief based on the provided product, service or event details. You will also tailor the marketing brief to the target audience (or audiences). You like to play with words and rhymes and like to state the truth and be consistent. You also like to produce creative and engaging text. For the brief you will provide a title, some short catchy phrases and descriptions of the product, service or event. You are laser focused with the goal at hand and do not waste time with chit chat. Your goal is to produce this marketing brief and refine it accepting any feedback and suggestions when refining an idea. You will aways respond by providing the marketing brief preceded by the iteration number which you will receive and add one. An example of the output is: " +
                    "---" +
                    "ITERATION: 1" +
                    "Marketing Brief: name of the marketing brief" +
                    "...contents of the marketing brief..." +
                    "---"
                    )
                    .WithName("CreatorAgent")
                    .WithDescription("Marketing Brief Creator Agent")
                    .BuildAsync());

        var criticAgent =
            Track(
                await new AgentBuilder()
                    //.WithOpenAIChatCompletion(openAIFunctionEnabledModelId, openAIApiKey)
                    .WithAzureOpenAIChatCompletion(
                       model: modelDeploymentName,
                       endpoint: azureOpenAIEndpoint,
                       apiKey: azureOpenAIApiKey)
                    .WithInstructions("You are a Critic agent with years of experience and a love of well written (concise, true, catchy and tailored to the audience) marketing briefs. You will evaluates the provided marketing brief for clarity, persuasiveness, brand and audience alignment. Always respond to the most recent message by evaluating and providing critique without example. Always provide the iteration number and the marketing brief, which you will provide at the beginning. After this you will provide your expert critique with the things you that are wrong first, and then, suggestions to improve. If the iteration is less than 5, provide at least one suggestion for improvement, and maximum 6. If after 5 or more iterations the marketing brief is acceptable and meets your criteria, say: DONE. " +
                    "An example of the output is: " +
                    "---" +
                    "ITERATION: 1" +
                    "Marketing Brief: name of the marketing brief" +
                    "...contents of the marketing brief..." +
                    "CRITIC:" +
                    "...contents of the critic..." +
                    "---" +
                    "Remember to add the Iteration number and the marketing brief at the beginning as shown in the example.")
                    .WithName("CriticAgent")
                    .WithDescription("Marketing Brief Critic Agent")
                    .BuildAsync());


        _agentsThread = await creatorAgent.NewThreadAsync();
        string marketingBriefIdeaToElaborate = "Create me a marketing brief for the OST NET and the Future event: .NET and the future\r\nThis event shows which expanded options with.NET 9 exist and how .NET projects can be upgraded with AI.\r\n\r\nprogram\r\n\r\n6:00 p.m. Welcome\r\n \r\n18:05 Input «What's new in.NET 9 »\r\nFind out more about the exciting new features in the upcoming version of.NET. This version focuses on improving the development and performance of cloud-native applications. We will highlight some selected examples from the extensive list of updates, improvements and optimizations.NET 9 has to offer.\r\nManuel Bauer, lecturer and team lead at Swiss Life\r\n \r\n18:40 Input «Infusing AI into your.NET Application with Semantic Kernel »\r\nDiscover how to spice up your.NET applications with Generative by using the Semantic Kernel SDK. Learn why Generative AI is crucial, and how to do it through some live-coding demos showing how to progressively get it done. We will also give a peek to the power of AI agents - practically. By the end of the talk, you'll know how to orchestrate Generative AI within your.NET projects and get a glimpse into the future of AI-driven development.\r\nJose Luis Latorre Microsoft MVP and Leader of Zurich.NET User Group Zurich\r\n \r\n19:15 aperitif\r\n \r\nThe event takes place on site Rapperswil-Jona campus instead of. Registration is required for organizational reasons.\r\n\r\nContact and information\r\nOST - Eastern Switzerland University of Applied Sciences\r\nManuel Bauer\r\n+41 (0) 58 257 45 99\r\nmanuel.bauer@ost.ch\r\n\r\nRapperswil-Jona campus, building 5, room 5.003.";

        try
        {
            // Add the user message
            var messageUser = await _agentsThread.AddUserMessageAsync(marketingBriefIdeaToElaborate);
            DisplayMessage(messageUser);

            bool isComplete = false;
            do
            {
                // Initiate copy-writer input
                var agentMessages = await _agentsThread.InvokeAsync(creatorAgent).ToArrayAsync();
                DisplayMessages(agentMessages, creatorAgent);

                // Initiate art-director input
                agentMessages = await _agentsThread.InvokeAsync(criticAgent).ToArrayAsync();
                DisplayMessages(agentMessages, criticAgent);

                // Evaluate if goal is met.
                if (agentMessages.First().Content.Contains("DONE", StringComparison.OrdinalIgnoreCase))
                {
                    isComplete = true;
                }
            }
            while (!isComplete);
        }
        finally
        {
            await CleanUpAsync();
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
                Console.WriteLine($"# {message.Role}: ({agent.Name}) {message.Content}");
                Console.WriteLine($"=============================================================================");
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
