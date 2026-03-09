using System;
using Xunit;

namespace Kawayi.Demystifier.Test;

public class ParameterParamTests
{
    [Fact]
    public void DemistifiesMethodWithParams()
    {
        Exception dex = null;
        try
        {
            MethodWithParams(1, 2, 3);
        }
        catch (Exception e)
        {
            dex = e.Demystify(StyleOptions.NoColorOption);
        }

        // Assert
        var stackTrace = dex.ToString();
        stackTrace = LineEndingsHelper.RemoveLineEndings(stackTrace);
        var trace = string.Join(string.Empty, stackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));

        var expected = string.Join(string.Empty, new[] {
            "System.ArgumentException: Value does not fall within the expected range.",
            "   at bool Kawayi.Demystifier.Test.ParameterParamTests.MethodWithParams(params int[] numbers)",
            "   at void Kawayi.Demystifier.Test.ParameterParamTests.DemistifiesMethodWithParams()"});

        Assert.Equal(expected, trace);
    }

    private bool MethodWithParams(params int[] numbers)
    {
        throw new ArgumentException();
    }
}
