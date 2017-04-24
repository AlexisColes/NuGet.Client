using System;
using System.Collections.Generic;
using System.Text;

namespace NuGet.Common
{
    interface IAssetsLogMessage : ILogMessage
    {
        /// <summary>
        /// Used to convert the Log Message object into a dictionary that can be written into the assets file.
        /// The Dictionary is converted into a JObject by the LokFileFormat.
        /// </summary>
        /// <returns>Dictionary of the properties representing the Log Message.</returns>
        Dictionary<string, object> ToDictionary();
    }
}
