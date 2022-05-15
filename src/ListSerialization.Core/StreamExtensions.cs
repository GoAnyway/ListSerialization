using System.Text;

namespace ListSerialization.Core;

internal static class StreamExtensions
{
    public static int ReadInt(this Stream stream)
    {
        using var buffer = new Array<byte>(stackalloc byte[sizeof(int)]);
        var read = stream.Read(buffer);
        if (read != sizeof(int)) throw new InvalidOperationException();

        return BitConverter.ToInt32(buffer);
    }

    public static string ReadString(this Stream stream)
    {
        var size = stream.ReadInt();
        using var buffer = new Array<byte>(size);
        var read = stream.Read(buffer, 0, size);
        if (read != size) throw new InvalidOperationException();

        return Encoding.UTF8.GetString(buffer, 0, size);
    }

    public static void Write(this Stream stream, int value) => 
        stream.Write(BitConverter.GetBytes(value));

    public static void Write(this Stream stream, string value)
    {
        Write(stream, value.Length);
        stream.Write(Encoding.UTF8.GetBytes(value));
    }
}