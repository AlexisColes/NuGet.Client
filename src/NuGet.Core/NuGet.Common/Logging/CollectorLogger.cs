// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NuGet.Common
{
    public class CollectorLogger : ICollectorLogger
    {
        private readonly ILogger _innerLogger;
        private readonly ConcurrentQueue<INuGetError> _errors;

        /// <summary>
        /// Initializes an instance of the <see cref="CollectorLogger"/>, while still
        /// delegating all log messages to the <param name="innerLogger" />.
        /// </summary>
        public CollectorLogger(ILogger innerLogger)
        {
            _innerLogger = innerLogger;
            _errors = new ConcurrentQueue<INuGetError>();
        }
        
        public void LogDebug(string data)
        {
            _innerLogger.LogDebug(data);
        }

        public void LogVerbose(string data)
        {
            _innerLogger.LogVerbose(data);
        }

        public void LogInformation(string data)
        {
            _innerLogger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            _innerLogger.LogMinimal(data);
        }

        public void LogWarning(string data)
        {
            _innerLogger.LogWarning(data);
        }
        public void LogError(string data)
        {
            _innerLogger.LogError(data);
        }

        public void LogInformationSummary(string data)
        {
            _innerLogger.LogInformationSummary(data);
        }

        public void LogErrorSummary(string data)
        {
            _innerLogger.LogErrorSummary(data);
        }

        public void LogError(INuGetError error)
        {
            _innerLogger.LogError(error.ToString());
            _errors.Enqueue(error);
        }

        public void LogWarning(INuGetError warning)
        {
            _innerLogger.LogWarning(warning.ToString());
            _errors.Enqueue(warning);
        }

        public IEnumerable<INuGetError> Errors => _errors.ToArray();
    }
}
