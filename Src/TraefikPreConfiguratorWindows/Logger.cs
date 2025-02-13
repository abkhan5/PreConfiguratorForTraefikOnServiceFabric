﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TraefikPreConfiguratorWindows
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;

    /// <summary>
    /// Trace Logger implementations.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The telemetry client used to log to Application insights.
        /// </summary>
        private static TelemetryClient telemetryClient = new TelemetryClient();

        /// <summary>
        /// Starts the Application Insights module.
        /// </summary>
        /// <param name="instrumentationKey">Application insights instrumentation key.</param>
        public static void ConfigureLogger(string instrumentationKey)
        {
            TelemetryConfiguration telemetryConfiguration = new TelemetryConfiguration(instrumentationKey);
            new DiagnosticsTelemetryModule().Initialize(telemetryConfiguration);
            Logger.telemetryClient = new TelemetryClient(telemetryConfiguration);
            var traceSource = new TraceSource("TraceLogging");
            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            traceSource.Switch.Level = SourceLevels.All;
        }

        /// <summary>
        /// Logs the verbose message.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogVerbose(CallInfo callInfo, string messageFormat, params object[] arguments)
        {
            Log(callInfo, TraceLevel.Verbose, GetMessage(messageFormat, arguments));
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogInfo(CallInfo callInfo, string messageFormat, params object[] arguments)
        {
            Log(callInfo, TraceLevel.Info, GetMessage(messageFormat, arguments));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogWarning(CallInfo callInfo, string messageFormat, params object[] arguments)
        {
            Log(callInfo, TraceLevel.Warning, GetMessage(messageFormat, arguments));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogError(CallInfo callInfo, string messageFormat, params object[] arguments)
        {
            Log(callInfo, TraceLevel.Error, GetMessage(messageFormat, arguments));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="exp">The exception.</param>
        public static void LogError(CallInfo callInfo, Exception exp)
        {
            Log(callInfo, TraceLevel.Error, $"Exception: {exp}");
        }

        /// <summary>
        /// Logs the error
        /// .</summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="exp">The exception.</param>
        /// <param name="customMessageFormat">The custom message format.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogError(CallInfo callInfo, Exception exp, string customMessageFormat, params object[] arguments)
        {
            Log(
                callInfo,
                TraceLevel.Error,
                $"CustomMessage {GetMessage(customMessageFormat, arguments)} \n Exception {exp}");
        }

        /// <summary>
        /// Flushes the logs to the backend diagnostics engine.
        /// </summary>
        public static void Flush()
        {
            Logger.telemetryClient.Flush();
        }

        /// <summary>
        /// Logs message to underlying loggers.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        /// <param name="traceLevel">The trace level.</param>
        /// <param name="message">The message.</param>
        private static void Log(CallInfo callInfo, TraceLevel traceLevel, string message)
        {
            string messageToLog = callInfo.ToString() + " -- " + traceLevel.ToString() + " -- " + message;
            Console.WriteLine(messageToLog);
            Logger.telemetryClient.TrackTrace(messageToLog);
        }

        /// <summary>
        /// Creates message from format.
        /// </summary>
        /// <param name="messageFormat">Message format.</param>
        /// <param name="arguments">Message arguments.</param>
        /// <returns>String message.</returns>
        private static string GetMessage(string messageFormat, params object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return messageFormat;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, messageFormat, arguments);
            }
        }
    }
}
