using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Common;
using NuGet.Common.Errors;

namespace NuGet.Commands
{
    class RestoreError : INuGetErrors<RestoreErrorCodes>
    {
        public LogLevel ErrorLevel { get; set; }
        public RestoreErrorCodes ErrorCode { get; set; }
        public string ErrorString { get; set; }
        public string ProjectPath { get; set; }
        public List<string> TfmOrRid { get; set; }
    }
}
