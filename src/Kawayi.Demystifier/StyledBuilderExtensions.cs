using Kawayi.Demystifier.Enumerable;

namespace Kawayi.Demystifier;

public static class StyledBuilderExtensions
{
    public static StyledStringBuilder AppendDemystified(this StyledStringBuilder stringBuilder, Exception exception, StyleOptions option)
    {
        try
        {
            var stackTrace = new EnhancedStackTrace(exception);

            stringBuilder.Append(exception.GetType().ToString());
            if (!string.IsNullOrEmpty(exception.Message))
            {
                stringBuilder.Append(": ").Append(option.MessageStyle, exception.Message);
            }
            stringBuilder.Append("\n");

            if (stackTrace.FrameCount > 0)
            {
                stackTrace.Append(stringBuilder, option);
            }

            if (exception is AggregateException aggEx)
            {
                foreach (var ex in EnumerableIList.Create(aggEx.InnerExceptions))
                {
                    stringBuilder.AppendInnerException(ex, option);
                }
            }

            if (exception.InnerException != null)
            {
                stringBuilder.AppendInnerException(exception.InnerException, option);
            }
        }
        catch (Exception e)
        {
            // Processing exceptions shouldn't throw exceptions; if it fails
            throw new AggregateException("inner exception", e, exception);
        }

        return stringBuilder;
    }

    private static void AppendInnerException(
        this StyledStringBuilder stringBuilder,
        Exception exception,
        StyleOptions option)
        => stringBuilder
            .Append(option.InnerExceptionOpenStyle, "   --->")
            .AppendLine()
            .AppendDemystified(exception, option)
            .AppendLine()
            .Append("   ")
            .Append(option.InnerExceptionEndStyle, "--- End of inner exception stack trace ---");
}
