using System.Collections;

namespace ListSerialization.Core;

public class ListRandom
{
    public int Count;
    public ListNode Head;
    public ListNode Tail;

    public void Serialize(Stream stream)
    {
        var serializer = new Serializer(this, stream);
        serializer.Serialize();
    }

    public void Deserialize(Stream stream)
    {
        var serializer = new Serializer(this, stream);
        serializer.Deserialize();
    }

    private readonly struct Enumerable : IEnumerable<ListNode>
    {
        private readonly ListRandom _list;

        public Enumerable(ListRandom list)
        {
            _list = list;
        }

        public IEnumerator<ListNode> GetEnumerator()
        {
            var current = _list.Head;
            while (current is not null)
            {
                yield return current;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private readonly struct Serializer
    {
        private readonly ListRandom _list;
        private readonly Stream _stream;

        public Serializer(ListRandom list, Stream stream)
        {
            _list = list;
            _stream = stream;
        }

        public void Serialize()
        {
            _stream.Write(BitConverter.GetBytes(_list.Count));

            var nodes = new Dictionary<ListNode, int>(_list.Count);
            var idx = 0;
            foreach (var node in new Enumerable(_list)) nodes.Add(node, idx++);

            foreach (var (node, _) in nodes)
            {
                _stream.Write(node.Data);
                var random = node.Random is not null
                    ? nodes[node.Random]
                    : -1;
                _stream.Write(random);
            }
        }

        public void Deserialize()
        {
            _list.Count = _stream.ReadInt();
            if (_list.Count == 0) return;

            using var nodes = new Array<Node>(_list.Count);
            for (var i = 0; i < _list.Count; i++)
            {
                var node = DeserializeNode();
                nodes[i] = node;

                if (_list.Head is null)
                {
                    _list.Head = node.ListNode;
                    _list.Tail = _list.Head;
                }

                node.ListNode.Previous = _list.Tail;
                _list.Tail.Next = node.ListNode;
                _list.Tail = node.ListNode;
            }

            for (var i = 0; i < _list.Count; i++)
            {
                var (node, random) = nodes[i];
                node.Random = nodes[random].ListNode;
            }
        }

        private Node DeserializeNode()
        {
            var data = _stream.ReadString();
            var random = _stream.ReadInt();

            return new(new() { Data = data }, random);
        }
    }

    private record struct Node(ListNode ListNode, int Random);
}