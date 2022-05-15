using ListSerialization.Core;
using static System.Console;

namespace ListSerialization.Console
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

            using (var stream = File.Open("test.dat", FileMode.OpenOrCreate))
            {
                list.Serialize(stream);
            }

            using (var stream = File.OpenRead("test.dat"))
            {
                list.Deserialize(stream);
            }

            ShowList(list);
        }

        private static void ShowList(ListRandom list)
        {
            WriteLine("Showing list");

            var current = list.Head;
            while (current is not null)
            {
                WriteLine($"Data = {current.Data}");
                WriteLine($"Prev = {current.Previous?.Data ?? "NULL"}");
                WriteLine($"Next = {current.Next?.Data ?? "NULL"}");
                WriteLine($"Random = {current.Random?.Data ?? "NULL"}");
                WriteLine();
                current = current.Next;
            }
        }
    }
}