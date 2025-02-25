using System.IO.Pipes;
using System.Text;
using YSocket.Messaging;

namespace YSocket.Ipc;

public class PipeBase : IDisposable
{
    public string Name
    {
        get;
        init;
    }

    private readonly Dictionary<string, Action<object>> handlers = [];

    protected PipeStream stream;
    protected BinaryWriter writer;
    protected CancellationToken cancellationToken;

    internal PipeBase(
        string name, 
        PipeStream pipeStream, 
        CancellationToken? cancellationToken)
    {
        this.Name = name;
        this.cancellationToken = cancellationToken ?? new CancellationTokenSource().Token;
        this.Init(pipeStream);
    }

    protected void Init(PipeStream pipeStream)
    {
        this.stream = pipeStream;
        this.writer = new BinaryWriter(this.stream);
    }

    protected async Task Read()
    {
        using var reader = new BinaryReader(this.stream);
        byte[] buffer = new byte[1024];
        var byteArray = new ByteArray();

        while (this.stream.IsConnected)
        {
            int bytesRead = await this.stream.ReadAsync(buffer, this.cancellationToken);

            byteArray.WriteBytes(buffer, 0, bytesRead);

            var length = 0;
            while (
                   byteArray.Position >= sizeof(Int32)
                && byteArray.Position >= (length = byteArray.PeekInt32(0)) + sizeof(Int32)
                )
            {
                var message = Encoding.UTF8.GetString(byteArray.Bytes, sizeof(Int32), length);

                this.Handle(message);

                byteArray.TrimStart(length + sizeof(Int32));
            }
        }
    }

    private void Handle(string message)
    {
        Task.Run(() =>
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                var messageWrapper = MessageWrapper.From<MessageWrapper>(message);
                this.handlers[messageWrapper.MessageQualifier](messageWrapper.Value);
            }
        });
    }

    public void Handle<T>(Action<T> handler)
    {
        this.handlers[typeof(T).AssemblyQualifiedName] = (obj) => handler((T)obj);
    }

    private async Task Write(string[] message)
    {
        var memoryStream = new MemoryStream();
        foreach (var item in message)
        {
            var bytes = Encoding.UTF8.GetBytes(item);
            memoryStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(Int32));
            memoryStream.Write(bytes, 0, bytes.Length);
        }

        this.writer.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
        this.writer.Flush();
        this.stream.WaitForPipeDrain();

        await Task.CompletedTask;
    }

    private void Write(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        this.writer.Write((Int32)bytes.Length);
        this.writer.Write(bytes);
        this.writer.Flush();
        this.stream.WaitForPipeDrain();
    }

    public void Send<T>(T obj)
        where T : class
    {
        this.Write(MessageWrapper.From(obj));
    }

    public void Dispose()
    {
        try { this.stream.Close(); } catch { }

        try { this.writer.Dispose(); } catch { }
    }
}
