﻿using System.IO;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace CodeConverters.Core.Diagnostics
{
    public static class Log4NetAppenderFactory
    {
        private static readonly PatternLayout PatternLayout = new PatternLayout("[%-5level][%date][%thread][%logger]: %message%newline");
        private const long TenMb = (10 * 1024 * 1024);
     
        public static IAppender CreateNewRelicAgentAppender()
        {
            return new NewRelicAgentErrorAppender { Threshold = Level.Error };
        }
    
        public static TraceAppender CreateTraceAppender()
        {
            var tracer = new TraceAppender { Layout = PatternLayout };
            tracer.ActivateOptions();
            PatternLayout.ActivateOptions();
            return tracer;
        }

        /// <summary>
        /// Creates a size based rolling file appender that is configured to fill up 31 x 10Mb log files before deleting old logs.
        /// Each time the log file roll overs it appends an incrementing counter onto the end of the filename.
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="loggingPath"></param>
        /// <param name="desiredLoggingLevel">Log threshold level to use, defaults to ALL if not supplied.</param>
        /// <returns></returns>
        public static RollingFileAppender CreateRollingFileAppender(string processName, string loggingPath, Level desiredLoggingLevel = null)
        {
            var levelToUse = desiredLoggingLevel ?? Level.All;
            var fileAppender = new RollingFileAppender
            {
                File = Path.Combine(loggingPath, processName + ".log"),
                AppendToFile = true,
                ImmediateFlush = true,
                Layout = PatternLayout,
                LockingModel = new FileAppender.MinimalLock(),
                Threshold = levelToUse,
                // use size base options so log4net will auto remove old logs
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 30, // gives us 31 files - the original plus 30 backups.  multiply that by 10Mb then we need 310Mb of storage
                MaxFileSize = TenMb,
                StaticLogFileName = false,
                PreserveLogFileNameExtension = false,
                CountDirection = 1
            };
            fileAppender.ActivateOptions();
            return fileAppender;
        }
    }
}