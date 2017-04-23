using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGet.Common
{
    public class RestoreError : INuGetError
    {
        public LogLevel ErrorLevel { get; set; }
        public NuGetErrorCode ErrorCode { get; set; }
        public string ErrorString { get; set; }
        public string ProjectPath { get; set; }
        public List<string> TfmOrRidList { get; set; }

        public RestoreError(LogLevel level, NuGetErrorCode errorCode, 
            string errorString, string projectPath, string tfmOrRid)
        {
            ErrorLevel = level;
            ErrorCode = errorCode;
            ErrorString = errorString;
            ProjectPath = projectPath;
            TfmOrRidList = new List<string>
            {
                tfmOrRid
            };
        }

        public override string ToString()
        {
            var errorString = new StringBuilder();

            return errorString.ToString();
        }

        public bool Equals(RestoreError other)
        {
            if(other == null)
            {
                return false;
            }
            else if(ReferenceEquals(this, other))
            {
                return true;
            }
            else if(ErrorLevel == other.ErrorLevel 
                && ProjectPath.Equals(other.ProjectPath) 
                && ErrorLevel == other.ErrorLevel
                && TfmOrRidList.SequenceEqual(other.TfmOrRidList))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
