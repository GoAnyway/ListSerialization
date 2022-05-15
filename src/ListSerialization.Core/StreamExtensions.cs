using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ListSerialization.Core;

internal static class StreamExtensions
{
    public static int ReadInt(this Stream stream)
    {
        var buffer = new Array<byte>(stackalloc byte[sizeof(int)]);
        var _ = stream.Read(buffer);

        return BitConverter.ToInt32(buffer);
    }

    public static string ReadString(this Stream stream)
    {
        const int stackallocThreshold = 1024;
        var size = stream.ReadInt();

        using var buffer = size <= stackallocThreshold
            ? new Array<byte>(stackalloc byte[size])
            : new(size);
        var _ = size <= stackallocThreshold
            ? stream.Read(buffer)
            : stream.Read(buffer, 0, size);

        var chars = MemoryMarshal.Cast<byte, char>(buffer);
        return new(chars);
    }

    public static void Write(this Stream stream, int value)
    {
        var buffer = new Array<byte>(stackalloc byte[sizeof(int)]);
        Unsafe.As<byte, int>(ref buffer.Span[0]) = value;
        stream.Write(buffer);
    }

    public static void Write(this Stream stream, string value)
    {
        var bytes = MemoryMarshal.Cast<char, byte>(value.AsSpan());
        Write(stream, bytes.Length);
        stream.Write(bytes);
    }
}