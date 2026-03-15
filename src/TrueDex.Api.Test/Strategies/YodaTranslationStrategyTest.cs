using AwesomeAssertions;
using FunTranslationClient;
using NSubstitute;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Test.Strategies;

public class YodaTranslationStrategyTest
{
    [Fact]
    public async Task ExecuteAsync_ShouldCallYodaTranslation_WithInputText()
    {
        const string inputText = "Master Yoda will approve this.";
        var expectedResponse = new TranslationResponse();
        var client = Substitute.For<ITranslationClient>();
        var strategy = new YodaTranslationStrategy();

        client.TranslateYodaAsync(
                Arg.Is<TranslationRequest>(request => request.Text == inputText),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var result = await strategy.ExecuteAsync(client, inputText);

        result.Should().BeSameAs(expectedResponse);
        await client.Received(1).TranslateYodaAsync(
            Arg.Is<TranslationRequest>(request => request.Text == inputText),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
        await client.DidNotReceive().TranslateShakespeareAsync(
            Arg.Any<TranslationRequest>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }
}
