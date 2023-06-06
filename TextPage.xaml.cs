﻿using Azure.AI.Vision.Common.Options;
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
        string document = @"L'arma è stata indicata, è stata repertata ,sapremo tutto quanto all'esito.Lo ha detto l'avvocato della famiglia di Giulia Tramontano, Giovanni Cacciapuoti uscendo dal sopralluogo durante il quale è stato trovato il coltello che Alessandro Impagnatiello avrebbe usato per uccidere la fidanzata Giulia Tramontano nella loro casa di Senago.Si tratta dell'arma che il barman 30enne ha detto di aver lavato e riposto dopo l'omicidio in un ceppo portacoltelli sopra il frigorifero della cucina. Poco prima avevano lasciato la casa senza rilasciare dichiarazioni anche il pm Alessia Menegazzo e il procuratore aggiunto Letizia Mannella. La procura di Milano ha poi delegato ai carabinieri gli accertamenti scientifici sugli altri coltelli infilati nel ceppo. Le analisi serviranno per individuare, in base anche alla compatibilità della lama con le ferite inferte, quale sia il coltello che è stato utilizzato per uccidere la 29enne.Il sopralluogo degli investigatori nell'abitazione era cominciato stamani a Senago in via Novella. Davanti alla casa tutte le tv per le dirette e una piccola folla di curiosi. L'abitazione si trova a poche centinaia di metri di distanza da via Monte Rosa, dove è stato trovato il corpo di Giulia. Il punto è diventato in questi giorni un luogo di pellegrinaggio e proprio ieri il sindaco Magda Beretta ha invitato i cittadini - 'in accordo con la famiglia Tramontano' - a portare i loro omaggi e i loro messaggi attorno alla panchina rossa contro la violenza sulle donne, nel parco Falcone e Borsellino. Dopo l'arrivo dei carabinieri che hanno tolto i sigilli al l'appartamento, a mezzogiorno è arrivato il furgone della sezione scientifica che ha imboccato la rampa dei box. Quindi sono entrati nella casa il procuratore aggiunto Letizia Mannella e la pm Alessia Menegazzo. Poco prima era arrivato in via Novella anche l'avvocato della famiglia Tramontano, Giovanni Cacciapuoti.";

        // Prepare analyze operation input. You can add multiple documents to this list and perform the same
        // operation to all of them.
        var batchInput = new List<string>
            {
                document
            };

        TextAnalyticsActions actions = new TextAnalyticsActions()
        {
            ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction() }
        };

        // Start analysis process.
        AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions,"IT-it");
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

                    foreach (SummarySentence sentence in documentResults.Sentences)
                    {
                        Console.WriteLine($"  Sentence: {sentence.Text}");
                        Console.WriteLine();
                    }
                }
            }
        }
    }

    
}
