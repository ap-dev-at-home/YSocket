using System.Net;

namespace YSocket.Messaging;

public class ByteArray : IDisposable
{
    private byte[] bytes;
    private int position = 0;

    public int Position
    {
        get => this.position;
        set => this.position = value;
    }

    public byte[] Bytes => this.bytes;

    public ByteArray(int capacity = 32)
    {
        this.bytes = new byte[capacity];
    }

    public ByteArray(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public ByteArray(byte[] bytes, int position)
    {
        this.bytes = bytes;
        this.position = position;
    }

    public void WriteIPAddress(IPAddress ipAdress)
    {
        byte[] addr = ipAdress.GetAddressBytes();

        this.WriteBytes(addr);
    }

    public unsafe void WriteUInt16(UInt16 v)
    {
        int diff = this.bytes.Length - sizeof(UInt16) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(UInt16*)b = v;
        }

        this.position += sizeof(UInt16);
    }

    public unsafe void WriteUInt32(UInt32 v)
    {
        int diff = this.bytes.Length - sizeof(UInt32) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(UInt32*)b = v;
        }

        this.position += sizeof(UInt32);
    }

    public unsafe void WriteFloat(float v)
    {
        int diff = this.bytes.Length - sizeof(float) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(float*)b = v;
        }

        this.position += sizeof(UInt32);
    }

    public unsafe void WriteInt32(int v)
    {
        int diff = this.bytes.Length - sizeof(int) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(int*)b = v;
        }

        this.position += sizeof(int);
    }

    public unsafe void WriteUInt64(ulong v)
    {
        int diff = this.bytes.Length - sizeof(ulong) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(ulong*)b = v;
        }

        this.position += sizeof(ulong);
    }

    public unsafe void WriteInt64(long v)
    {
        int diff = this.bytes.Length - sizeof(long) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            *(long*)b = v;
        }

        this.position += sizeof(long);
    }

    public void WriteByte(byte b)
    {
        int diff = this.bytes.Length - sizeof(byte) - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        unchecked
        {
            this.bytes[this.position] = b;
        }

        this.position += 1;
    }

    public void WriteBytes(byte[] src)
    {
        int l = (sizeof(byte) * src.Length);
        int diff = this.bytes.Length - l - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        Array.Copy(src, 0, this.bytes, this.position, src.Length);

        this.position += src.Length;
    }

    public void WriteBytes(Memory<byte> src)
    {
        int l = (sizeof(byte) * src.Length);
        int diff = this.bytes.Length - l - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        src.CopyTo(new Memory<byte>(this.bytes, this.position, src.Length));

        this.position += src.Length;
    }

    public void WriteBytes(byte[] src, int offset, int length)
    {
        int l = (sizeof(byte) * length);
        int diff = this.bytes.Length - l - this.position;
        if (diff < 0)
        {
            Array.Resize<byte>(ref this.bytes, this.bytes.Length + (diff * -1));
        }

        Array.Copy(src, offset, this.bytes, this.position, length);
        this.position += length;
    }

    public IPAddress ReadIPv4Address()
    {
        if (this.position > this.bytes.Length - 4)
        {
            throw new IndexOutOfRangeException();
        }

        var ipAdress = new IPAddress([
            this.bytes[this.position + 0],
            this.bytes[this.position + 1],
            this.bytes[this.position + 2],
            this.bytes[this.position + 3]
        ]);

        this.position += 4;

        return ipAdress;
    }

    public byte ReadByte()
    {
        if (this.position > this.bytes.Length - 1)
        {
            throw new IndexOutOfRangeException();
        }

        byte result = this.bytes[this.position];
        this.position += 1;

        return result;
    }

    public unsafe UInt16 ReadUInt16()
    {
        if (this.position > this.bytes.Length - sizeof(UInt16))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(UInt16);
            return *(UInt16*)b;
        }
    }

    public unsafe UInt32 ReadUInt32()
    {
        if (this.position > this.bytes.Length - sizeof(UInt32))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(UInt32);
            return *(UInt32*)b;
        }
    }

    public unsafe float ReadFloat()
    {
        if (this.position > this.bytes.Length - sizeof(float))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(float);
            return *(float*)b;
        }
    }

    public unsafe int ReadInt32()
    {
        if (this.position > this.bytes.Length - sizeof(Int32))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(int);
            return *(int*)b;
        }
    }

    public unsafe int PeekInt32(int? position)
    {
        int p = position ?? this.position;
        if (p > this.bytes.Length - sizeof(Int32))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[p])
        {
            return *(int*)b;
        }
    }

    public unsafe UInt64 ReadUInt64()
    {
        if (this.position > this.bytes.Length - sizeof(UInt64))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(UInt64);
            return *(UInt64*)b;
        }
    }

    public unsafe Int64 ReadInt64()
    {
        if (this.position > this.bytes.Length - sizeof(Int64))
        {
            throw new IndexOutOfRangeException();
        }

        fixed (byte* b = &this.bytes[this.position])
        {
            this.position += sizeof(Int64);
            return *(Int64*)b;
        }
    }

    public byte[] ReadBytes(int count)
    {
        if (this.position > this.bytes.Length - count)
        {
            throw new IndexOutOfRangeException();
        }

        byte[] result = new byte[count];

        Array.Copy(this.bytes, this.Position, result, 0, count);

        this.position += count;

        return result;
    }

    public void TrimStart(int count)
    {
        int diff = this.bytes.Length - count;
        byte[] newArray = new byte[diff];
        Buffer.BlockCopy(this.bytes, count, newArray, 0, newArray.Length);
        this.bytes = newArray;
        this.position -= count;
    }

    public void Reset()
        => this.position = 0;
    
    public void Clear()
        => this.bytes = [];
    
    public void Dispose() 
        => this.bytes = [];
}

