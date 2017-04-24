using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Common
{
    public class RestoreLogMessage : IAssetsLogMessage
    {
        public LogLevel Level { get; set; }
        public NuGetLogCode Code { get; set; }
        public string Message { get; set; }
        public string ProjectPath { get; set; }
        public IReadOnlyList<string> TargetGraphs { get; set; }
        public DateTimeOffset Time { get; set; }

        public RestoreLogMessage(LogLevel logLevel, NuGetLogCode errorCode, 
            string errorString, string projectPath, string targetGraph)
        {
            Level = logLevel;
            Code = errorCode;
            Message = errorString;
            ProjectPath = projectPath;
            TargetGraphs = new List<string>
            {
                targetGraph
            };
        }

        public bool Equals(RestoreLogMessage other)
        {
            if(other == null)
            {
                return false;
            }
            else if(ReferenceEquals(this, other))
            {
                return true;
            }
            else if(Level == other.Level 
                && ProjectPath.Equals(other.ProjectPath) 
                && Level == other.Level
                && TargetGraphs.SequenceEqual(other.TargetGraphs))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            var errorDictionary = new Dictionary<string, object>
            {
                [LogMessageProperties.LOG_CODE_PROPERTY] = nameof(Code),
                [LogMessageProperties.LOG_LEVEL_PROPERTY] = $"{Level}"
            };

            if(Message != null)
            {
                errorDictionary[LogMessageProperties.MESSAGE_PROPERTY] = Message;
            }
            if(ProjectPath != null)
            {
                errorDictionary[LogMessageProperties.PROJECT_PATH_PROPERTY] = ProjectPath;
            }
            if(TargetGraphs != null && TargetGraphs.Any())
            {
                errorDictionary[LogMessageProperties.TARGET_GRAPH_PROPERTY] = TargetGraphs;
            }
            if(Time != null)
            {
                errorDictionary[LogMessageProperties.TIME_PROPERTY] = Time;
            }

            return errorDictionary;
        }

        public string FormatMessage()
        {
            var errorString = new StringBuilder();

            errorString.Append($"{nameof(Code)}:{Message}");

            return errorString.ToString();
        }
    }
}
