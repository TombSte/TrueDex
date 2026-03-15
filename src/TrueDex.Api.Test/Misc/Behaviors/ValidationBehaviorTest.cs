using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using TrueDex.Api.Misc.Behaviors;

namespace TrueDex.Api.Test.Misc.Behaviors;

public class ValidationBehaviorTest
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsAreRegistered()
    {
        var behavior = new ValidationBehavior<TestMessage, string>([]);
        var message = new TestMessage("pikachu");
        var cancellationTokenSource = new CancellationTokenSource();
   
        var nextWasCalled = false;

        MessageHandlerDelegate<TestMessage, string> next = (request, cancellationToken) =>
        {
            nextWasCalled = true;
            request.Should().BeSameAs(message);
            cancellationToken.Should().Be(cancellationTokenSource.Token);
            return ValueTask.FromResult("handled");
        };

        var result = await behavior.Handle(message, next, cancellationTokenSource.Token);

        result.Should().Be("handled");
        nextWasCalled.Should().BeTrue();
        await cancellationTokenSource.CancelAsync();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenAllValidatorsSucceed()
    {
        var validator = new PassingValidator();

        var behavior = new ValidationBehavior<TestMessage, string>([validator]);
        var message = new TestMessage("charizard");
        var cancellationTokenSource = new CancellationTokenSource();
        var nextWasCalled = false;

        MessageHandlerDelegate<TestMessage, string> next = (request, cancellationToken) =>
        {
            nextWasCalled = true;
            request.Should().BeSameAs(message);
            cancellationToken.Should().Be(cancellationTokenSource.Token);
            return ValueTask.FromResult("validated");
        };

        var result = await behavior.Handle(message, next, cancellationTokenSource.Token);

        result.Should().Be("validated");
        nextWasCalled.Should().BeTrue();
        validator.CapturedCancellationToken.Should().Be(cancellationTokenSource.Token);
        await cancellationTokenSource.CancelAsync();
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenAnyValidatorFails()
    {
        var validators = new IValidator<TestMessage>[]
        {
            new FailingValidator("Name", "Name is required"),
            new FailingValidator("Name", "Name is too short")
        };

        var behavior = new ValidationBehavior<TestMessage, string>(validators);
        var message = new TestMessage(string.Empty);
        var nextWasCalled = false;

        MessageHandlerDelegate<TestMessage, string> next = (_, _) =>
        {
            nextWasCalled = true;
            return ValueTask.FromResult("should-not-run");
        };

        var action = async () => await behavior.Handle(message, next, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Select(static x => x.ErrorMessage)
            .Should()
            .BeEquivalentTo(["Name is required", "Name is too short"]);
        nextWasCalled.Should().BeFalse();
    }

    private sealed record TestMessage(string Name) : IRequest<string>;

    private sealed class PassingValidator : AbstractValidator<TestMessage>
    {
        public CancellationToken CapturedCancellationToken { get; private set; }

        public override Task<ValidationResult> ValidateAsync(
            ValidationContext<TestMessage> context,
            CancellationToken cancellation = default)
        {
            CapturedCancellationToken = cancellation;
            return Task.FromResult(new ValidationResult());
        }
    }

    private sealed class FailingValidator(string propertyName, string errorMessage) : AbstractValidator<TestMessage>
    {
        public override Task<ValidationResult> ValidateAsync(
            ValidationContext<TestMessage> context,
            CancellationToken cancellation = default)
        {
            return Task.FromResult(new ValidationResult([
                new ValidationFailure(propertyName, errorMessage)
            ]));
        }
    }
}
