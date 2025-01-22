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
using EnterAiAgentEraDemos;

namespace EnterAiAgentEraDemos;

#pragma warning disable SKEXP0101
public class AgentCraftingHistory
{
    readonly List<IAgent> _agents = new();
    IAgentThread? _agentsThread = null;

    public async Task Execute()
    {
        var openAIFunctionEnabledModelId = "gpt-4o";
        var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        //var builder = Kernel.CreateBuilder();
        //builder.Services.AddOpenAIChatCompletion(
        //    openAIFunctionEnabledModelId,
        //    openAIApiKey);
        //var kernel = builder.Build();

        var userMessage = "";
        var todaysDatePlugin = KernelPluginFactory.CreateFromType<WhatDateIsIt>();
        var historianAgent = await new AgentBuilder()
                        .WithOpenAIChatCompletion(openAIFunctionEnabledModelId, openAIApiKey)
                        .WithInstructions("You are an historian, you know about the past and all that has happened." +
                        "you will respond using factual figures which are based in real events." +
                        "you will not invent any event which is not true and has not happened in reality." +
                        "If you don't know how to respond just say that you have absolutely no idea.")
                        .WithName("HistorianAgent")
                        .WithDescription("A knowledgeable agent for digging into past events.")
                        .WithPlugin(todaysDatePlugin)
                        .BuildAsync();
        _agents.Add(historianAgent);
        _agentsThread = await historianAgent.NewThreadAsync();
        Console.WriteLine("Enter a message to send to the agent or type 'exit' to quit:");

        while (true)
        {
            userMessage = Console.ReadLine();
            if (userMessage == "exit")
            {
                Console.WriteLine($"Banana!!");
                break;
            }

            var responseMessages = 
                await _agentsThread.InvokeAsync(historianAgent, userMessage).ToArrayAsync();
            DisplayMessages(responseMessages, historianAgent);
        }

        await CleanUpAsync();

        Console.WriteLine($"=============================================================================");
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
