using System.Collections.Generic;
namespace MUtility
{
    public static class EnumerateUtility
    {
        public static LinkedListNode<T> AppendLast<T>(this LinkedList<T> list, T[] array)
        {
            var head = list.AddLast( array[0] );
            for (int i = 1; i < array.Length; i++)
            {
                list.AddLast( array[i] );
            }
            return head;
        }
    }
}
