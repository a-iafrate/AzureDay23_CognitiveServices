using Azure.AI.Vision.Common.Options;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Maui.Controls;
using Azure.AI.Vision.Common.Input;

using Azure.AI.TextAnalytics;
using System.Net;

namespace AzureDay23_CognitiveServices;

public partial class TextPage : ContentPage
{
	int count = 0;

	public TextPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		

        TextSummarization();

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
            ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction(new ExtractSummaryOptions() { 
            MaxSentenceCount=(int)picker2.SelectedItem}) }
        };

        // Start analysis process.
        AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions,(string)picker.SelectedItem);
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
            IReadOnlyCollection<ExtractSummaryActionResult> summaryResults = documentsInPage.ExtractSummaryResults;

            foreach (ExtractSummaryActionResult summaryActionResults in summaryResults)
            {
                if (summaryActionResults.HasError)
                {
                    Console.WriteLine($"  Error!");
                    Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
                    Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
                    continue;
                }

                foreach (ExtractSummaryResult documentResults in summaryActionResults.DocumentsResults)
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
                    foreach (SummarySentence sentence in documentResults.Sentences)
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

    
}

