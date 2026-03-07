using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kawayi.Demystifier.Test
{
    public class NonThrownException
    {
        [Fact]
        public async Task DoesNotPreventThrowStackTrace()
        {
            // Arrange
            Exception innerException = null;
            try
            {
                await Task.Run(() => throw new Exception()).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                innerException = ex;
            }

            // Act
            var demystifiedException = new Exception(innerException.Message, innerException)
                .Demystify(StyleOptions.NoColorOption);

            // Assert
            var stackTrace = demystifiedException.ToString();
            stackTrace = LineEndingsHelper.RemoveLineEndings(stackTrace);
            var trace = stackTrace.Split(new[]{Environment.NewLine}, StringSplitOptions.None);

#if NETCOREAPP3_0_OR_GREATER
            Assert.Equal(
                new[] {
                    "System.Exception: Exception of type 'System.Exception' was thrown.",
                    " ---> System.Exception: Exception of type 'System.Exception' was thrown.",
                    "   at Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()+() => { }",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()",
                    "   --- End of inner exception stack trace ---"},
                trace);
#else
            Assert.Equal(
                new[] {
                    "System.Exception: Exception of type 'System.Exception' was thrown. ---> System.Exception: Exception of type 'System.Exception' was thrown.",
                    "   at Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()+() => { }",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()",
                    "   --- End of inner exception stack trace ---"},
                trace);
#endif

            // Act
            try
            {
                throw demystifiedException;
            }
            catch (Exception ex)
            {
                demystifiedException = ex.Demystify(StyleOptions.NoColorOption);
            }

            // Assert
            stackTrace = demystifiedException.ToString();
            stackTrace = LineEndingsHelper.RemoveLineEndings(stackTrace);
            trace = stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

#if NETCOREAPP3_0_OR_GREATER
            Assert.Equal(
                new[] {
                    "System.Exception: Exception of type 'System.Exception' was thrown.",
                    " ---> System.Exception: Exception of type 'System.Exception' was thrown.",
                    "   at Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()+() => { }",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()",
                    "   --- End of inner exception stack trace ---",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()"
                },
                trace);
#else
            Assert.Equal(
                new[] {
                    "System.Exception: Exception of type 'System.Exception' was thrown. ---> System.Exception: Exception of type 'System.Exception' was thrown.",
                    "   at Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()+() => { }",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()",
                    "   --- End of inner exception stack trace ---",
                    "   at async Task Kawayi.Demystifier.Test.NonThrownException.DoesNotPreventThrowStackTrace()"
                },
                trace);
#endif
        }

        [Fact]
        public async Task Current()
        {
            // Arrange
            EnhancedStackTrace est = null;

            // Act
            await Task.Run(() => est = EnhancedStackTrace.Current()).ConfigureAwait(false);

            // Assert
            var stackTrace = est.ToString();
            stackTrace = LineEndingsHelper.RemoveLineEndings(stackTrace);
            var trace = stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                // Remove Full framework entries
                .Where(s => !s.StartsWith("   at bool System.Threading._ThreadPoolWaitCallbac") &&
                            !s.StartsWith("   at void System.Threading.Tasks.Task.System.Thre"))
                .ToArray();

            Assert.Equal("   at Task Kawayi.Demystifier.Test.NonThrownException.Current()+() => { }", trace[0]);
            Assert.Equal("   at bool System.Threading.ThreadPoolWorkQueue.Dispatch()", trace[^3]);
            Assert.Equal("   at void System.Threading.PortableThreadPool+WorkerThread.WorkerThreadStart()", trace[^2]);
            Assert.Equal("   at void System.Threading.Thread.StartCallback()", trace[^1]);
        }
    }
}
