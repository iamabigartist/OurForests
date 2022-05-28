using System.Collections.Generic;
namespace VolumeMegaStructure.Util
{
	public class BiDictionary<TLValue, TRValue>
	{
		Dictionary<TLValue, TRValue> left_to_right = new();
		Dictionary<TRValue, TLValue> right_to_left = new();

		public void Add(TLValue left, TRValue right)
		{
			left_to_right.Add(left, right);
			right_to_left.Add(right, left);
		}

		public bool ContainsLeft(TLValue left)
		{
			return left_to_right.ContainsKey(left);
		}

		public bool ContainsRight(TRValue right)
		{
			return right_to_left.ContainsKey(right);
		}

		public TRValue this[TLValue left]
		{
			get => left_to_right[left];
			set
			{
				left_to_right[left] = value;
				right_to_left[value] = left;
			}
		}

		public TLValue this[TRValue right]
		{
			get => right_to_left[right];
			set
			{
				right_to_left[right] = value;
				left_to_right[value] = right;
			}
		}

		public void RemoveByLeft(TLValue left)
		{
			TRValue right = left_to_right[left];
			left_to_right.Remove(left);
			right_to_left.Remove(right);
		}

		public void RemoveByRight(TRValue right)
		{
			TLValue left = right_to_left[right];
			right_to_left.Remove(right);
			left_to_right.Remove(left);
		}

		public (TLValue v_left, TRValue v_right)[] ToArray()
		{
			var array = new(TLValue v_left, TRValue v_right)[left_to_right.Count];
			int i = 0;
			foreach (var (left, right) in left_to_right)
			{
				array[i] = (left, right);
				i++;
			}
			return array;
		}
	}
}