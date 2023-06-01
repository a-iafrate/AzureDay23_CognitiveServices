using Azure.AI.Vision.Common.Options;
using Azure;

namespace AzureDay23_CognitiveServices;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private void RemoveBackground()
	{

        var serviceOptions = new VisionServiceOptions(
    Environment.GetEnvironmentVariable("VISION_ENDPOINT"),
    new AzureKeyCredential(Environment.GetEnvironmentVariable("VISION_KEY")));
    }
}

