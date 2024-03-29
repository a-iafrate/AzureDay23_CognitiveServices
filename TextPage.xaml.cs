﻿using Azure.AI.Vision.Common.Options;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Maui.Controls;
using Azure.AI.Vision.Common.Input;

using Azure.AI.TextAnalytics;
using System.Net;
using Azure.Core;
using System.Text.Json;
using Azure.AI.Language.Conversations;

namespace AzureDay23_CognitiveServices;

public partial class TextPage : ContentPage
{
    int count = 0;

    public TextPage()
    {
        InitializeComponent();
        picker.SelectedIndex = 0;
        picker2.SelectedIndex = 0;
    }

    private void OnTextClicked(object sender, EventArgs e)
    {
        TextSummarization();
    }

    private void OnConversationClicked(object sender, EventArgs e)
    {

        ConversationSummarization();

    }

    private async void TextSummarization()
    {
        //https://portal.azure.com/#create/Microsoft.CognitiveServicesTextAnalytics

        AzureKeyCredential credentials = new AzureKeyCredential("9a12a08ba613461ba4ca2b1e3c9d0c60");
        Uri endpoint = new Uri("https://azureday23text.cognitiveservices.azure.com/");
        var client = new TextAnalyticsClient(endpoint, credentials);

        // Prepare analyze operation input. You can add multiple documents to this list and perform the same
        // operation to all of them.
        var batchInput = new List<string>
            {
                editor1.Text
            };

        TextAnalyticsActions actions = new TextAnalyticsActions()
        {

            ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new ExtractiveSummarizeAction(new ExtractiveSummarizeOptions() {
            MaxSentenceCount=(int)picker2.SelectedItem}) }
        };

        // Start analysis process.
        AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions, (string)picker.SelectedItem);
        await operation.WaitForCompletionAsync();
        // View operation status.
        Console.WriteLine($"AnalyzeActions operation has completed");
        Console.WriteLine();

        Console.WriteLine($"Created On   : {operation.CreatedOn}");
        Console.WriteLine($"Expires On   : {operation.ExpiresOn}");
        Console.WriteLine($"Id           : {operation.Id}");
        Console.WriteLine($"Status       : {operation.Status}");

        Console.WriteLine();
        // View operation results.
        await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
        {
            IReadOnlyCollection<ExtractiveSummarizeActionResult> summaryResults = documentsInPage.ExtractiveSummarizeResults;

            foreach (ExtractiveSummarizeActionResult summaryActionResults in summaryResults)
            {
                if (summaryActionResults.HasError)
                {
                    Console.WriteLine($"  Error!");
                    Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
                    Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
                    continue;
                }

                foreach (ExtractiveSummarizeResult documentResults in summaryActionResults.DocumentsResults)
                {
                    if (documentResults.HasError)
                    {
                        Console.WriteLine($"  Error!");
                        Console.WriteLine($"  Document error code: {documentResults.Error.ErrorCode}.");
                        Console.WriteLine($"  Message: {documentResults.Error.Message}");
                        continue;
                    }

                    Console.WriteLine($"  Extracted the following {documentResults.Sentences.Count} sentence(s):");
                    Console.WriteLine();

                    string result = string.Empty;
                    foreach (var sentence in documentResults.Sentences)
                    {
                        result += sentence.Text + "\r\n";
                        Console.WriteLine($"  Sentence: {sentence.Text}");
                        Console.WriteLine();
                    }
                    editor2.Text = result;
                }
            }
        }
    }


    private async void ConversationSummarization()
    {
        //https://portal.azure.com/#create/Microsoft.CognitiveServicesTextAnalytics

        AzureKeyCredential credentials = new AzureKeyCredential("9f445925ccab44e3baf6fd84c25faa16");
        Uri endpoint = new Uri("https://azureday23cogserus.cognitiveservices.azure.com/");
        var client = new ConversationAnalysisClient(endpoint, credentials);

        var data = new
        {
            analysisInput = new
            {
                conversations = new[]
            {
            new
            {
                conversationItems = new[]
                {
                    new
                    {
                        text = "Hello, you’re chatting with Rene. How may I help you?",
                        id = "1",
                        role = "Agent",
                    },
                    new
                    {
                        text = "Hi, I tried to set up wifi connection for Smart Brew 300 coffee machine, but it didn’t work.",
                        id = "2",
                        role = "Customer",
                    },
                    new
                    {
                        text = "I’m sorry to hear that. Let’s see what we can do to fix this issue. Could you please try the following steps for me? First, could you push the wifi connection button, hold for 3 seconds, then let me know if the power light is slowly blinking on and off every second?",
                        id = "3",
                        role = "Agent",
                    },
                    new
                    {
                        text = "Yes, I pushed the wifi connection button, and now the power light is slowly blinking?",
                        id = "4",
                        role = "Customer",
                    },
                    new
                    {
                        text = "Great. Thank you! Now, please check in your Contoso Coffee app. Does it prompt to ask you to connect with the machine?",
                        id = "5",
                        role = "Agent",
                    },
                    new
                    {
                        text = "No. Nothing happened.",
                        id = "6",
                        role = "Customer",
                    },
                    new
                    {
                        text = "I’m very sorry to hear that. Let me see if there’s another way to fix the issue. Please hold on for a minute.",
                        id = "7",
                        role = "Agent",
                    }
                },
                id = "1",
                language = "en",
                modality = "text",
            },
        }
            },
            tasks = new[]
        {
        new
        {
            parameters = new
            {
                summaryAspects = new[]
                {
                    "issue",
                    "resolution",
                }
            },
            kind = "ConversationalSummarizationTask",
            taskName = "1",
        },
    },
        };

        Operation<BinaryData> analyzeConversationOperation = client.AnalyzeConversations(WaitUntil.Started, RequestContent.Create(data));
        analyzeConversationOperation.WaitForCompletion();

        using JsonDocument result = JsonDocument.Parse(analyzeConversationOperation.Value.ToStream());
        JsonElement jobResults = result.RootElement;
        foreach (JsonElement task in jobResults.GetProperty("tasks").GetProperty("items").EnumerateArray())
        {
            JsonElement results = task.GetProperty("results");

            string text = ("Conversations:\r\n");
            foreach (JsonElement conversation in results.GetProperty("conversations").EnumerateArray())
            {

                text += ($"Conversation: #{conversation.GetProperty("id").GetString()}\r\n");
                text += ("Summaries:\r\n");
                foreach (JsonElement summary in conversation.GetProperty("summaries").EnumerateArray())
                {
                    text += ($"Text: {summary.GetProperty("text").GetString()} - ");
                    text += ($"Aspect: {summary.GetProperty("aspect").GetString()}\r\n");
                }

            }
            editorConversation.Text = text;
        }
    }
}


