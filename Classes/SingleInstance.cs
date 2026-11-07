using System.IO.Pipes;
using System.Text;

namespace Drums4me.Classes;

/// <summary>
/// Sorgt dafür, dass nur ein Fenster läuft und nie zwei Metronome gleichzeitig spielen.
/// Ein weiterer Start übergibt seine Datei über eine Pipe, die nur dem eigenen Benutzer offensteht, und beendet sich.
/// </summary>
internal sealed class SingleInstance : IDisposable
{
    private const string Name = "drums4me.SingleInstance";
    private const int ConnectTimeoutMs = 3000;

    private readonly Mutex _mutex;
    private readonly CancellationTokenSource _stop = new();

    public SingleInstance()
    {
        _mutex = new Mutex(true, $@"Local\{Name}", out var createdNew);
        IsFirst = createdNew;
    }

    public bool IsFirst { get; }

    /// <summary>Übergibt die Datei (oder null zum reinen Aktivieren) an die laufende Instanz. Liefert false, wenn sie nicht antwortet.</summary>
    public static bool TrySendToFirst(string? path)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", Name, PipeDirection.Out, PipeOptions.CurrentUserOnly);
            client.Connect(ConnectTimeoutMs);

            // Windows lässt ein Hintergrundprogramm sonst nicht in den Vordergrund.
            if (NativeMethods.GetNamedPipeServerProcessId(client.SafePipeHandle, out var serverProcessId))
            {
                NativeMethods.AllowSetForegroundWindow(serverProcessId);
            }

            var bytes = Encoding.UTF8.GetBytes(path ?? string.Empty);
            client.Write(bytes, 0, bytes.Length);
            client.Flush();
            return true;
        }
        catch (Exception ex) when (ex is TimeoutException or IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Nimmt Dateien weiterer Starts entgegen. Der Aufruf von <paramref name="onMessage"/> erfolgt im Hintergrund.</summary>
    public void Listen(Action<string?> onMessage) => _ = ListenAsync(onMessage, _stop.Token);

    private static async Task ListenAsync(Action<string?> onMessage, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(Name, PipeDirection.In, 1, PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                await server.WaitForConnectionAsync(token).ConfigureAwait(false);

                using var reader = new StreamReader(server, Encoding.UTF8);
                var text = await reader.ReadToEndAsync(token).ConfigureAwait(false);
                onMessage(text.Length == 0 ? null : text);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Eine abgebrochene Übergabe betrifft nur diesen einen Start, die nächste läuft wieder normal.
                await Task.Delay(200, CancellationToken.None).ConfigureAwait(false);
            }
        }
    }

    public void Dispose()
    {
        _stop.Cancel();
        _stop.Dispose();
        if (IsFirst)
        {
            _mutex.ReleaseMutex();
        }
        _mutex.Dispose();
    }
}
