// ==============================================================================
// The MIT License (MIT)
//
// Copyright (c) 2026 Victor Matia (vitimiti)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ==============================================================================

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Vitimiti.SdlLogging.Ffi;

internal static unsafe partial class Sdl
{
    public readonly record struct LogCategory(int Value)
    {
        [SuppressMessage(
            "Style",
            "IDE0046:Convert to conditional expression",
            Justification = "Nested conditional expressions are difficult to read."
        )]
        public override string ToString()
        {
            if (this == LogCategoryApplication)
            {
                return "Application";
            }
            if (this == LogCategoryError)
            {
                return "Error";
            }
            if (this == LogCategoryAssert)
            {
                return "Assert";
            }
            if (this == LogCategorySystem)
            {
                return "System";
            }
            if (this == LogCategoryAudio)
            {
                return "Audio";
            }
            if (this == LogCategoryVideo)
            {
                return "Video";
            }
            if (this == LogCategoryRender)
            {
                return "Render";
            }
            if (this == LogCategoryInput)
            {
                return "Input";
            }
            if (this == LogCategoryTest)
            {
                return "Test";
            }
            if (this == LogCategoryGpu)
            {
                return "Gpu";
            }

            return $"Unknown({Value})";
        }
    }

    public static LogCategory LogCategoryApplication => new(0);
    public static LogCategory LogCategoryError => new(1);
    public static LogCategory LogCategoryAssert => new(2);
    public static LogCategory LogCategorySystem => new(3);
    public static LogCategory LogCategoryAudio => new(4);
    public static LogCategory LogCategoryVideo => new(5);
    public static LogCategory LogCategoryRender => new(6);
    public static LogCategory LogCategoryInput => new(7);
    public static LogCategory LogCategoryTest => new(8);
    public static LogCategory LogCategoryGpu => new(9);

    public readonly record struct LogPriority(int Value)
    {
        [SuppressMessage(
            "Style",
            "IDE0046:Convert to conditional expression",
            Justification = "Nested conditional expressions are difficult to read."
        )]
        public static LogPriority FromLogger(ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                return LogPriorityTrace;
            }
            if (logger.IsEnabled(LogLevel.Debug))
            {
                return LogPriorityDebug;
            }
            if (logger.IsEnabled(LogLevel.Information))
            {
                return LogPriorityInfo;
            }
            if (logger.IsEnabled(LogLevel.Warning))
            {
                return LogPriorityWarn;
            }
            if (logger.IsEnabled(LogLevel.Error))
            {
                return LogPriorityError;
            }
            if (logger.IsEnabled(LogLevel.Critical))
            {
                return LogPriorityCritical;
            }

            return LogPriorityInvalid;
        }
    }

    public static LogPriority LogPriorityInvalid => new(0);
    public static LogPriority LogPriorityTrace => new(1);
    public static LogPriority LogPriorityVerbose => new(2);
    public static LogPriority LogPriorityDebug => new(3);
    public static LogPriority LogPriorityInfo => new(4);
    public static LogPriority LogPriorityWarn => new(5);
    public static LogPriority LogPriorityError => new(6);
    public static LogPriority LogPriorityCritical => new(7);

    [LibraryImport(DllName, EntryPoint = "SDL_SetLogPriorities")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    public static partial void SetLogPriorities(LogPriority priority);

    public static GCHandle LogOutputFunctionHandle { get; set; }

    [LibraryImport(DllName, EntryPoint = "SDL_SetLogOutputFunction")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    public static partial void SetLogOutputFunction(
        delegate* unmanaged[Cdecl]<void*, LogCategory, LogPriority, byte*, void> logOutputFunction,
        void* userData
    );
}
