using AwesomeAssertions;
using FunTranslationClient;
using NSubstitute;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Test.Strategies;

public class ShakespeareTranslationStrategyTest
{
    [Fact]
    public async Task ExecuteAsync_ShouldCallShakespeareTranslation_WithInputText()
    {
        const string inputText = "test.";
        var expectedResponse = new TranslationResponse(){ Contents = new Contents()
        {
            Translated = "translated"
        }};
        var client = Substitute.For<ITranslationClient>();
        var strategy = new ShakespeareTranslationStrategy();

        client.TranslateShakespeareAsync(
                Arg.Is<TranslationRequest>(request => request.Text == inputText),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var result = await strategy.ExecuteAsync(client, inputText);

        result.Should().BeSameAs(expectedResponse);
        
        await client.Received(1).TranslateShakespeareAsync(
            Arg.Is<TranslationRequest>(request => request.Text == inputText),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
        
        await client.DidNotReceive().TranslateYodaAsync(
            Arg.Any<TranslationRequest>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }
}
