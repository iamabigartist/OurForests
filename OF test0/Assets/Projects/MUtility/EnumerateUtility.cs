using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace MUtility
{
    public static class EnumerateUtility
    {

        public static T[] Copy<T>(this T[] array)
        {
            var new_array = new T[array.Length];
            Array.Copy( array, new_array, 0 );
            return new_array;
        }

        public static string ToMString<T>(this IEnumerable<T> array)
        {
            return string.Join( ",", array );
        }

    #region SQL

        public static int[] AllIndexOf<T>(this T[] source, T value)
        {
            return (..source.Length).ToArray().Where( i => source[i].Equals( value ) ).ToArray();
        }

    #endregion

    #region Range

        public static int[] ToArray(this Range r)
        {
            return Enumerable.Range( r.Start.Value, r.End.Value - r.Start.Value ).ToArray();
        }

    #endregion

    #region LinkedList

        public static LinkedListNode<T> AppendLast<T>(this LinkedList<T> list, T[] array)
        {
            var head = list.AddLast( array[0] );
            for (int i = 1; i < array.Length; i++)
            {
                list.AddLast( array[i] );
            }
            return head;
        }

    #endregion

    #region Flatten

        public static int FlattenedCount<T>(this IEnumerable<IEnumerable<T>> lists)
        {
            return lists.Sum( list => list.Count() );
        }

        public static T[] ToFlattenedArray<T>(this LinkedList<List<T>> lists)
        {
            var array = new T[lists.FlattenedCount()];
            int start_index = 0;
            for (var i = lists.First; i != null; i = i.Next)
            {
                var cur_list = i.Value;
                cur_list.CopyTo( array, start_index );
                start_index = +cur_list.Count;
            }
            return array;
        }

        public static T[] ToFlattenedArrayParallel<T>(this List<List<T>> lists, int[] start_indices)
        {
            var array = new T[lists.FlattenedCount()];

            Parallel.For( 0, lists.Count, i =>
            {
                var cur_list = lists[i];
                cur_list.CopyTo( array, start_indices[i] );
            } );
            return array;
        }

        public static T[] ToFlattenedArrayParallel<T>(this List<T[]> lists, int[] start_indices)
        {
            var array = new T[lists.FlattenedCount()];

            Parallel.For( 0, lists.Count, i =>
            {
                var cur_list = lists[i];
                cur_list.CopyTo( array, start_indices[i] );
            } );

            return array;
        }

        public static int[] GetFlattenCopyPositions<T>(this IEnumerable<IEnumerable<T>> arrays)
        {
            var copy_positions = new int[arrays.Count()];
            int i = 0;
            int sum = 0;
            foreach (var array in arrays)
            {
                copy_positions[i] = sum;
                i++;
                sum += array.Count();
            }
            return copy_positions;
        }

        // public static T[] ToFlattenedArrayParallel<T>(
        //     this IEnumerable<IEnumerable<T>> arrays,
        //     int[] write_positions)
        // {
        //     var flatten_array = new T[write_positions.Last()];
        //
        //     Parallel.ForEach( arrays, (array, state, i) =>
        //     {
        //         array.ToArray().CopyTo( flatten_array, write_positions[i] );
        //     } );
        //
        //     return flatten_array;
        // }

        public static T[] ToFlattenedArray<T>(
            this IEnumerable<IEnumerable<T>> arrays,
            int[] write_positions)
        {
            var flatten_array = new T[write_positions.Last()];

            int i = 0;
            foreach (var array in arrays)
            {
                array.ToArray().CopyTo( flatten_array, write_positions[i] );
                i++;
            }

            return flatten_array;
        }

    #endregion
    }
}
