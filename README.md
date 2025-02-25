# YSocket

YSocket is a C# Library to provide communication in the following categories

- Interprocess
  - Named Pipes
- Microservices (in Work...)
  - WebSockets
- Gaming (in Work...)
  - Small packaged Udp

# Interprocess Communication (IPC)

### Named Pipe Server
```csharp
using YSocket.Ipc;

//- Setup Named Pipe Server
//- Register Command/Query Handlers

class ServerPipeHandler : IDisposable
{
    private readonly PipeServer pipeServer;
    
    public ServerPipeHandler(
        CancellationToken? cancellationToken = null)
    {
        var pipeSecurity = new PipeSecurity();
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
            PipeAccessRights.ReadWrite,
            AccessControlType.Allow));

        this.pipeServer = new(Names.PIPE_SERVER_GUID, pipeSecurity, cancellationToken);
        
        this.RegisterPipeHandlers();
        
        this.pipeServer.Start();
    }

    private void RegisterPipeHandlers()
    {
        this.pipeServer.Handle<ExitCommand>(exitCommand =>
        {
            //exitcommand received - stop application
        });

        this.pipeServer.Handle<StatusInformationQuery>(statusInformationQuery =>
        {
            //statusinformationquery received - respond
            this.pipeServer.Send(new StatusInformation { ... });
        });
    }

    public void Dispose()
    {
        this.pipeServer.Dispose();
    }
}
```

### Named Pipe Client
```csharp
using YSocket.Ipc;

//- Setup Named Pipe Client
//- Register Command/Query Handlers

class ClientPipeHandler : IDisposable
{
    private readonly PipeClient pipeClient;
    
    internal ClientPipeHandler(
        CancellationToken? cancellationToken)
    {
        this.pipeClient = new(Names.PIPE_SERVER_GUID, 
            cancellationToken: cancellationToken);
    }

    internal void RegisterPipeHandlers()
    {
        this.pipeClient.Handle<StatusInformation>(statusInformation =>
        {
            //process status information
        });
    }

    internal async Task<bool> Connect()
    {
        var retry = 0;
        var success = false;

        do
        {
            success = await this.pipeClient.Connect(2500);
            if (success == true)
            {
                break;
            }
        } while (retry++ < 3);

        return await Task.FromResult(success);
    }

    internal bool QueryStatusInformation()
    {
        try
        {
            this.pipeClient.Send(new StatusInformationQuery());
        }
        catch (Exception ex)
        {
            return false;
        }
    
        return true;
    }

    internal bool TerminateServer()
    {
        try
        {
            this.pipeClient.Send(new ExitCommand());
        }
        catch (Exception ex)
        {
            return false;
        }
    
        return true;
    }

    public void Dispose()
    {
        this.pipeClient.Dispose();
    }
}
```
