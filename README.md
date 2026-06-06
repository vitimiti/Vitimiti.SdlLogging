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

## Automated NuGet Publishing on GitHub

This repository includes a GitHub Actions workflow in `.github/workflows/nuget.yml`.

### What it does

- Builds the solution on every push to `main`, every pull request, and every `v*` tag.
- Packs the NuGet package on tags matching `v*`.
- Publishes the `.nupkg` and `.snupkg` files to NuGet.org.
- Creates a GitHub Release for each `v*` tag and attaches the generated packages.

### Release flow

The workflow derives the NuGet version from the git tag name, so a tag like `v1.2.3` publishes version `1.2.3`.

When the tag build succeeds, GitHub also gets a matching Release page with the generated NuGet package files attached as downloadable assets.

```bash
git tag v1.0.0
git push origin v1.0.0
```

### Notes

- The project file still keeps a default `Version` for local packing, but tagged CI releases override it.
- The NuGet API key is only stored as the `NUGET_API_KEY` GitHub Actions secret. Do not commit it, add it to the workflow file directly, or store it in source control.
- You can use the same API key locally with `dotnet nuget push`, but prefer a dedicated key for CI so you can revoke it without affecting your local setup.
