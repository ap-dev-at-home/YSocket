using System.IO.Pipes;

namespace YSocket.Ipc;

public class PipeClient : PipeBase
{
    public string ServerName
    {
        get;
        init;
    }

    private NamedPipeClientStream ClientStream => (NamedPipeClientStream)base.stream;

    public PipeClient(
        string name, 
        string serverName = ".",
        CancellationToken? cancellationToken = null)
        : base(
            name,
            new NamedPipeClientStream(
                serverName,
                name,
                PipeDirection.InOut,
                PipeOptions.Asynchronous),
            cancellationToken)
    {
        this.ServerName = serverName;
    }

    public async Task<bool> Connect(int timeout)
    {
        try
        {
            await this.ClientStream.ConnectAsync(timeout, base.cancellationToken);
        }
        catch
        {
            return await Task.FromResult(false);
        }

        try
        {
            _ = base.Read();
        }
        catch (Exception ex)
        {
            base.Error?.Invoke(ex);
        }

        return await Task.FromResult(true);
    }
}
