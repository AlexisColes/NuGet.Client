﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGet.Common
{
    public class CollectorLogger : LoggerBase, ICollectorLogger
    {
        private readonly ILogger _innerLogger;
        private readonly ConcurrentQueue<ILogMessage> _errors;

        /// <summary>
        /// Initializes an instance of the <see cref="CollectorLogger"/>, while still
        /// delegating all log messages to the <param name="innerLogger" />.
        /// </summary>
        public CollectorLogger(ILogger innerLogger)
        {
            _innerLogger = innerLogger;
            _errors = new ConcurrentQueue<ILogMessage>();
        }

        public override void Log(ILogMessage message)
        {
            _errors.Enqueue(message);
            _innerLogger.Log(message);
        }

        public override Task LogAsync(ILogMessage message)
        {            
            _errors.Enqueue(message);
            return _innerLogger.LogAsync(message);
        }

        public override void Log(LogLevel level, string data)
        {
            //TODO clean this
            var message = new RestoreLogMessage(level, NuGetLogCode.Undefined, data, string.Empty, string.Empty);
            _errors.Enqueue(message);
            _innerLogger.Log(level, data);
        }

        public override Task LogAsync(LogLevel level, string data)
        {
            //TODO clean this
            var message = new RestoreLogMessage(level, NuGetLogCode.Undefined, data, string.Empty, string.Empty);
            _errors.Enqueue(message);
            return _innerLogger.LogAsync(level, data);
        }

        public IEnumerable<ILogMessage> Errors => _errors.ToArray();
    }
}
