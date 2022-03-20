using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace VolumeMegaStructure.Util
{
    public class FixedSegmentList<T> : List<T>
    {
        //TODO clean code and practice usage//
        //TODO finish all the management//
        public readonly int SegmentLength;
        public int SegmentCount => Count * SegmentLength;

        public int SegmentCapacity
        {
            get => Capacity / SegmentLength;
            set => Capacity = value * SegmentLength;
        }

        public int SegmentStartIndex(int segment_index) { return SegmentLength * segment_index; }


        public FixedSegmentList(int segment_length)
        {
            SegmentLength = segment_length;
        }

        public FixedSegmentList(int segment_length, IEnumerable<T> source) : base( source )
        {
            SegmentLength = segment_length;
            if (Count % SegmentLength != 0)
            {
                Capacity += SegmentLength - Count % SegmentLength;
            }
        }

        public void Add(T[] segment)
        {
            if (segment.Length != SegmentLength)
            {
                throw new Exception( "Wrong segment length" );
            }
            base.AddRange( segment );
        }

        public void AddRange(T[] segments)
        {
            if (segments.Length % SegmentLength != 0)
            {
                throw new Exception( "Wrong segment length" );
            }
            base.AddRange( segments );
        }

        //Removing segments will then move the tail segments back to the removed range.
        public (int moved_start_segment_index, int moved_segment_count ) RemoveRange(int start_segment_index, int count = 1, bool parallel_move = false)
        {
            int remove_length = count * SegmentLength;
            int tail_start_index = SegmentStartIndex( SegmentCount - count );
            int remove_start_index = SegmentStartIndex( start_segment_index );
            int remove_end_next_index = remove_start_index + count;

            //If the tail segments length is not enough, then just move less segments
            if (remove_end_next_index > tail_start_index)
            {
                count -= remove_end_next_index - tail_start_index;
                tail_start_index = remove_end_next_index;
            }

            int offset = tail_start_index - remove_start_index;
            if (!parallel_move)
            {
                for (int i = tail_start_index; i - tail_start_index < remove_length; i++)
                {
                    this[i - offset] = this[i];
                }
            }
            else
            {
                Parallel.For( tail_start_index, tail_start_index + remove_length, (i) =>
                {
                    this[i - offset] = this[i];
                } );
            }
            base.RemoveRange( tail_start_index, remove_length );

            return (tail_start_index / SegmentLength, remove_length / SegmentLength);

        }

    }

}
