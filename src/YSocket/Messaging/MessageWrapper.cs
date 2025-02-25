using System.Text.Json;

namespace YSocket.Messaging;

public class MessageWrapper
{
    public string MessageQualifier
    {
        get;
        init;
    }

    public string Message
    {
        get;
        init;
    }

    public object Value
    {
        get
        {
            return JsonSerializer.Deserialize(this.Message, Type.GetType(this.MessageQualifier));
        }
    }

    public static MessageWrapper From<T>(string message)
    {
        return JsonSerializer.Deserialize<MessageWrapper>(message);
    }

    public static string From<M>(M obj)
    {
        return JsonSerializer.Serialize(new MessageWrapper
        {
            MessageQualifier = obj.GetType().AssemblyQualifiedName,
            Message = JsonSerializer.Serialize(obj)
        });
    }
}
