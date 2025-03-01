using System.IO.Pipes;

namespace YSocket.Ipc;

public class PipeServer : PipeBase
{
    private NamedPipeServerStream ServerStream => (NamedPipeServerStream)base.stream;
    private readonly PipeSecurity? pipeSecurity;

    public PipeServer(
        string name, 
        PipeSecurity? pipeSecurity = null,
        CancellationToken? cancellationToken = default)
        : base(
            name,
            PipeServer.From(name, pipeSecurity),
            cancellationToken)
    {
        this.pipeSecurity = pipeSecurity;
    }

    private static NamedPipeServerStream From(string name, PipeSecurity? pipeSecurity)
    {
        return NamedPipeServerStreamAcl.Create(
            name,
            PipeDirection.InOut,
            maxNumberOfServerInstances: 1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous,
            inBufferSize: 0,
            outBufferSize: 0,
            pipeSecurity);
    }

    public void Start()
    {
        _ = this.Wait();
    }

    private async Task Wait()
    {
        while (base.cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                await this.ServerStream.WaitForConnectionAsync(base.cancellationToken);
                await base.Read();
            }
            catch (Exception ex)
            {
                base.Error?.Invoke(ex);
            }
            finally
            {
                try { this.ServerStream.Disconnect(); } catch { }
            }

            try { this.ServerStream.Dispose(); } catch { }

            base.Init(PipeServer.From(base.Name, this.pipeSecurity));
        }
    }
}
