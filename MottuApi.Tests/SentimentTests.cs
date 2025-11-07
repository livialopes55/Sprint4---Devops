using FluentAssertions;
using MottuApi.ML;
using Xunit;

namespace MottuApi.Tests;

public class SentimentTests
{
    [Fact]
    public void Sentiment_Positive_Text_Should_Be_Positive()
    {
        var model = new SentimentModel();
        model.Predict("serviço excelente").Should().BeTrue();
    }
}
