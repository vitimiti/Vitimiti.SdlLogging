# Vitimiti's SDL3 Logging

A small library to help with the logging system in SDL3 by using the `ILogger` interface.

## How to Use

This is a safe, disposable object that manages SDL's logging system and takes Microsoft's `ILogger` interface to integrate it into regular dotnet logging systems.

To use it, in your application, you may do something such as:

```csharp
using Vitimiti.SdlLogging;

internal interface INativeContext : IDisposable
{
    void Initialize();
}

internal sealed class SdlNativeContext(ILoggerFactory loggerFactory) : INativeContext
{
    private SafeLogObject? _sdlLogObject;
    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            // Release SDL3 stuff
            _sdlLogObject?.Dispose();
        }

        _sdlObject = null;
        // Quit SDL3
        _disposedValue = true;
    }

    ~SdlNativeContext()
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

    public void Initialize()
    {
        _sdlLogObject = new SafeLogObject(loggerFactory.CreateLogger<SafeLogObject>());
        // Initialize SDL3 stuff
    }
}
```

This will automatically modify SDL's log level to the same level as your `ILogger` so that logging can happen in a controlled, expected manner.
