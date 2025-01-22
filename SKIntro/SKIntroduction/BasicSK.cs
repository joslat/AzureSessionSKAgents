using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;

namespace SKIntroduction;

public static class BasicSK
{
    public static async Task Execute()
    {

        var modelDeploymentName = "gpt-4o";
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZUREOPENAI_ENDPOINT");
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_APIKEY");

        Kernel kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                modelDeploymentName,
                azureOpenAIEndpoint,
                azureOpenAIApiKey)
            .Build();

        //kernel.ImportPluginFromType<WhatDateIsIt>();

        string userPrompt = "I would like to know what date is it and 5 significative" +
            "things that happened on the past on this day.";

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var result = await kernel.InvokePromptAsync(
            userPrompt,
            new(openAIPromptExecutionSettings));

        Console.WriteLine($"Result: {result}");
        Console.WriteLine();
    }
}
