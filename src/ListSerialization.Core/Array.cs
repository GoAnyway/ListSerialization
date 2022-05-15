using System.Buffers;

namespace ListSerialization.Core;

internal readonly ref struct Array<T>
{
    private readonly T[] _array;
    private readonly bool _rented;

    public Span<T> Span { get; }
    public int Size { get; }

    public Array(int size)
    {
        _array = ArrayPool<T>.Shared.Rent(size);
        _rented = true;
        Span = _array.AsSpan(..size);
        Size = size;
    }

    public Array(Span<T> span)
    {
        _array = null;
        _rented = false;
        Span = span;
        Size = span.Length;
    }

    public static implicit operator T[](Array<T> array) =>
        array._array ?? throw new InvalidOperationException();

    public static implicit operator Span<T>(Array<T> array) => array.Span;
    public static implicit operator ReadOnlySpan<T>(Array<T> array) => array.Span;

    public void Dispose()
    {
        if (!_rented) return;
        ArrayPool<T>.Shared.Return(_array);
    }

    public T this[int i]
    {
        get => IsInRange(i)
            ? Span[i]
            : throw new ArgumentOutOfRangeException(nameof(i));
        set => Span[i] = IsInRange(i)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(i));
    }

    public Span<T> this[Range range] =>
        Span[range];

    private bool IsInRange(int index) =>
        index >= 0 && index < Size;
}