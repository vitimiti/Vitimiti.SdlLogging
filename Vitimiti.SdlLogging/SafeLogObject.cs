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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Extensions.Logging;
using Vitimiti.SdlLogging.Ffi;

namespace Vitimiti.SdlLogging;

public sealed class SafeLogObject : IDisposable
{
    private delegate void LogOutputFunction(
        Sdl.LogCategory category,
        Sdl.LogPriority priority,
        string message
    );

    private readonly ILogger<SafeLogObject> _logger;

    private bool _disposedValue;

    public SafeLogObject(ILogger<SafeLogObject> logger)
    {
        _logger = logger;
        Sdl.SetLogPriorities(Sdl.LogPriority.FromLogger(_logger));
        Sdl.LogOutputFunctionHandle = GCHandle.Alloc(
            (Action<Sdl.LogCategory, Sdl.LogPriority, string>)LogOutput
        );

        unsafe
        {
            Sdl.SetLogOutputFunction(
                &UnmanagedLogOutput,
                (void*)GCHandle.ToIntPtr(Sdl.LogOutputFunctionHandle)
            );
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            // No managed resources to dispose.
        }

        if (Sdl.LogOutputFunctionHandle.IsAllocated)
        {
            Sdl.LogOutputFunctionHandle.Free();
        }

        _disposedValue = true;
    }

    ~SafeLogObject()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void UnmanagedLogOutput(
        void* userData,
        Sdl.LogCategory category,
        Sdl.LogPriority priority,
        byte* message
    )
    {
        if (userData is null)
        {
            return;
        }

        var handle = GCHandle.FromIntPtr((nint)userData);
        if (!handle.IsAllocated || handle.Target is not LogOutputFunction callback)
        {
            return;
        }

        var messageString = Utf8StringMarshaller.ConvertToManaged(message) ?? string.Empty;
        callback(category, priority, messageString);
    }

    private void LogOutput(Sdl.LogCategory category, Sdl.LogPriority priority, string message)
    {
        if (priority == Sdl.LogPriorityTrace)
        {
            LoggerMessages.SdlTrace(_logger, category, message);
        }
        else if (priority == Sdl.LogPriorityDebug)
        {
            LoggerMessages.SdlDebug(_logger, category, message);
        }
        else if (priority == Sdl.LogPriorityInfo)
        {
            LoggerMessages.SdlInformation(_logger, category, message);
        }
        else if (priority == Sdl.LogPriorityWarn)
        {
            LoggerMessages.SdlWarning(_logger, category, message);
        }
        else if (priority == Sdl.LogPriorityError)
        {
            LoggerMessages.SdlError(_logger, category, message);
        }
        else if (priority == Sdl.LogPriorityCritical)
        {
            LoggerMessages.SdlCritical(_logger, category, message);
        }
    }
}
