using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ListSerialization.Core
{
    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream stream)
        {
            var elem = Head;
            var list = new List<ListNode>();

            var n = Count;
            while (n-- > 0)
            {
                list.Add(elem);
                elem = elem.Next;
            }

            WriteBytes(stream, BitConverter.GetBytes(Count));

            foreach (var node in list)
            {
                WriteBytes(stream, BitConverter.GetBytes(list.IndexOf(node.Random)));
                var bytes = Encoding.UTF8.GetBytes(node.Data);
                WriteBytes(stream, BitConverter.GetBytes(bytes.Length));
                WriteBytes(stream, bytes);
            }
        }

        public void Deserialize(Stream stream)
        {
            Head = null;
            Tail = null;

            var buffer = new byte[sizeof(int)];
            stream.Read(buffer);
            Count = BitConverter.ToInt32(buffer);
            var list = new List<ListNode>();
            var randoms = new List<int>();

            var n = Count;
            while (n-- > 0)
            {
                buffer = new byte[sizeof(int)];
                stream.Read(buffer);
                var idx = BitConverter.ToInt32(buffer);
                randoms.Add(idx);

                stream.Read(buffer);
                var size = BitConverter.ToInt32(buffer);

                buffer = new byte[size];
                stream.Read(buffer);
                list.Add(new ListNode
                {
                    Data = Encoding.UTF8.GetString(buffer)
                });
            }

            if (Count == 0) return;

            Head = list[0];
            Tail = list[Count - 1];

            var elem = Head;
            for (var idx = 0; idx < Count; ++idx)
            {
                elem.Random = list[randoms[idx]];
                elem.Previous = idx > 0 ? list[idx - 1] : null;
                elem.Next = idx < Count - 1 ? list[idx + 1] : null;
                elem = elem.Next;
            }
        }

        private void WriteBytes(Stream stream, byte[] bytes)
        {
            foreach (var @byte in bytes)
            {
                stream.WriteByte(@byte);
            }
        }
    }
}