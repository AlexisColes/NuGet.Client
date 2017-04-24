using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Common
{
    public abstract class LoggerBase : ILogger
    {
        protected void Write(string data, LogLevel level)
        {
            WriteMessage(data, level);
        }
        protected Task WriteAsync(string data, LogLevel level)
        {
            return new Task(() => WriteMessage(data, level));
        }

        private void WriteMessage(string data, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;

                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;

                case LogLevel.Information:
                    LogInformation(data);
                    break;

                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;

                case LogLevel.Warning:
                    LogWarning(data);
                    break;

                case LogLevel.Error:
                    LogError(data);
                    break;
            }
        }
        public void Log(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string data)
        {
            throw new NotImplementedException();
        }

        public void LogError(string data)
        {
            throw new NotImplementedException();
        }

        public void LogErrorSummary(string data)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string data)
        {
            throw new NotImplementedException();
        }

        public void LogInformationSummary(string data)
        {
            throw new NotImplementedException();
        }

        public void LogMinimal(string data)
        {
            throw new NotImplementedException();
        }

        public void LogVerbose(string data)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string data)
        {
            throw new NotImplementedException();
        }
    }
}
