using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace SKIntroduction;

public static class CreatePromptPlugin
{
    public static KernelPlugin CreatePromptPluginWithFunctions(Kernel kernel)
    {
        var spellingFunction = KernelFunctionFactory.CreateFromPrompt(
             "Correct any misspelling or gramatical errors provided in input: {{$input}}",
              functionName: "spellChecker",
              description: "Correct the spelling for the user input.");

        var summarizeFunction = KernelFunctionFactory.CreateFromPrompt(
             "Summarize the provided input: {{$input}}",
              functionName: "summarizer",
              description: "summarize the user input.");

        var jokeFunction = KernelFunctionFactory.CreateFromPrompt(
             "Make a joke on the provided input: {{$input}}",
              functionName: "JokeMaker",
              description: "Make a joke on the user input.");

        var translatorFunction = KernelFunctionFactory.CreateFromPrompt(
             "Translate to english the provided input: {{$input}}",
              functionName: "translator",
              description: "translate to english the user input.");


        KernelPlugin simplePlugin =
            KernelPluginFactory.CreateFromFunctions(
                "QuizCreationPlugin",
                "Helps analyze scripts and create quizzes from them.",
                new[] {
                    spellingFunction,
                    summarizeFunction,
                    jokeFunction,
                    translatorFunction
                });

        return simplePlugin;
    }

}
