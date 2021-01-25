using System;
using System.IO;
using ListSerializationCore;

namespace ListSerialization
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var node1 = new ListNode
            {
                Data = "(1) Данные первого элемента"
            };
            var node2 = new ListNode
            {
                Data = "(2) Данные второго элемента"
            };
            var node3 = new ListNode
            {
                Data = "(3) Данные третьего и последнего элемента"
            };

            node1.Next = node2;
            node2.Previous = node1;

            node2.Next = node3;
            node3.Previous = node2;

            node1.Random = node3;
            node2.Random = node1;
            node3.Random = node1;

            var list = new ListRandom
            {
                Count = 3,
                Head = node1,
                Tail = node3
            };

            ShowList(list);

            using (var stream = new FileStream("test.dat", FileMode.Create))
            {
                list.Serialize(stream);
            }

            using (var stream = new FileStream("test.dat", FileMode.Open))
            {
                list.Deserialize(stream);
            }

            ShowList(list);
        }

        private static void ShowList(ListRandom list)
        {
            Console.WriteLine("Showing list");

            var elem = list.Head;
            while (elem != null)
            {
                Console.WriteLine($"Data = {elem.Data}");
                Console.WriteLine($"Prev = {elem.Previous?.Data ?? "NULL"}");
                Console.WriteLine($"Next = {elem.Next?.Data ?? "NULL"}");
                Console.WriteLine($"Random = {elem.Random?.Data ?? "NULL"}");
                Console.WriteLine();
                elem = elem.Next;
            }
        }
    }
}
