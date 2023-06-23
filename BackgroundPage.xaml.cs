using Azure.AI.Vision.Common.Options;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Maui.Controls;
using Azure.AI.Vision.Common.Input;

using Azure.AI.TextAnalytics;
using System.Net;

namespace AzureDay23_CognitiveServices;

public partial class BackgroundPage : ContentPage
{
    int count = 0;

    public BackgroundPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {

        RemoveBackground();

    }



    private async void RemoveBackground()
    {
        var resultFile = await FilePicker.Default.PickAsync();
        if (resultFile != null)
        {
            if (resultFile.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                resultFile.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                //using var stream = await resultFile.OpenReadAsync();
                //var image = ImageSource.FromStream(() => stream);

                image1.Source = ImageSource.FromFile(resultFile.FullPath);
            }
        }
        else
        {
            return;
        }

        var serviceOptions = new VisionServiceOptions(
    "https://azureday23cogser.cognitiveservices.azure.com",
    new AzureKeyCredential("76d79cf0f3d849f78718b2e8470c8f80"));

        using var imageSource = VisionSource.FromFile(
    resultFile.FullPath);

        ///////// segmentation
        var analysisOptions = new ImageAnalysisOptions()
        {
            SegmentationMode = ImageSegmentationMode.BackgroundRemoval,


        };

        using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

        var result = analyzer.Analyze();

        if (result.Reason == ImageAnalysisResultReason.Analyzed)
        {
            using var segmentationResult = result.SegmentationResult;

            var imageBuffer = segmentationResult.ImageBuffer;
            Console.WriteLine($" Segmentation result:");
            Console.WriteLine($"   Output image buffer size (bytes) = {imageBuffer.Length}");
            Console.WriteLine($"   Output image height = {segmentationResult.ImageHeight}");
            Console.WriteLine($"   Output image width = {segmentationResult.ImageWidth}");
            var bytes = imageBuffer.ToArray();
            image2.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            /*string outputImageFile = "output.png";
            using (var fs = new FileStream(outputImageFile, FileMode.Create))
            {
                fs.Write(imageBuffer.Span);
            }*/
            //Console.WriteLine($"   File {outputImageFile} written to disk");
        }
        else
        {
            var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
            Console.WriteLine(" Analysis failed.");
            Console.WriteLine($"   Error reason : {errorDetails.Reason}");
            Console.WriteLine($"   Error code : {errorDetails.ErrorCode}");
            Console.WriteLine($"   Error message: {errorDetails.Message}");
            Console.WriteLine(" Did you set the computer vision endpoint and key?");
        }

        ///////// text
        using var imageSourceFeatures = VisionSource.FromFile(
    resultFile.FullPath);
        var analysisOptionsFeatures = new ImageAnalysisOptions()
        {
            Features = ImageAnalysisFeature.Tags | ImageAnalysisFeature.Caption,

        };

        using var analyzerFeatures = new ImageAnalyzer(serviceOptions, imageSourceFeatures, analysisOptionsFeatures);

        var resultFeatures = analyzerFeatures.Analyze();

        if (resultFeatures.Reason == ImageAnalysisResultReason.Analyzed)
        {
            title.Text = resultFeatures.Caption.Content;
            tags.Text = "";
            foreach (var tag in resultFeatures.Tags)
            {
                tags.Text += tag.Confidence.ToString("F3") + " - " + tag.Name + "\r\n";
            }

        }
        else
        {
            var errorDetails = ImageAnalysisErrorDetails.FromResult(resultFeatures);
            Console.WriteLine(" Analysis failed.");
            Console.WriteLine($"   Error reason : {errorDetails.Reason}");
            Console.WriteLine($"   Error code : {errorDetails.ErrorCode}");
            Console.WriteLine($"   Error message: {errorDetails.Message}");
            Console.WriteLine(" Did you set the computer vision endpoint and key?");
        }
    }
}

