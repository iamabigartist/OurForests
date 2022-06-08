using UnityEngine;
namespace Labs.Lab10_TestBitwise
{
	/// <summary>
	///     //结论：GPU仍需要特殊的位运算处理函数，CPU上处理方式不太一样，但是也是必要的。
	/// </summary>
	public class BitwiseOperatorTranslator : MonoBehaviour
	{
		public struct Operator
		{
			public uint value;
			bool IsAdd() { return value >> 30 == 0b00; }
			bool IsSub() { return value >> 30 == 0b01; }
			bool IsMul() { return value >> 30 == 0b10; }
			bool IsDivide() { return value >> 30 == 0b11; }
			uint Value2() { return value & ((1 << 15) - 1); }
			uint Value1() { return (value >> 15) & ((1 << 15) - 1); }

			public string GetResult()
			{
				uint value1 = Value1();
				uint value2 = Value2();
				uint result = 0;
				string value_operator = "";
				if (IsAdd())
				{
					value_operator = "+";
					result = value1 + value2;
				}
				else if (IsSub())
				{
					value_operator = "-";
					result = value1 - value2;
				}
				else if (IsMul())
				{
					value_operator = "*";
					result = value1 * value2;
				}
				else if (IsDivide())
				{
					value_operator = "/";
					result = value1 / value2;
				}
				return $"operator: {value_operator} value1: {value1} value2: {value2} result: {result}";
			}
		}

		public static uint GetBits(uint source, int start, int len)
		{
			return (uint)((source >> (start - len + 1)) & ((1 << len) - 1));
		}

		public static uint SetBits1(uint source, int start, int len)
		{
			int offset = start - len + 1;
			return source | ((0xFFFFFFFF >> offset) << offset);
		}

		public static uint SetBits0(uint source, int start, int len)
		{
			int offset = start - len + 1;
			return source & ~((0xFFFFFFFF >> offset) << offset);
		}
		void Start()
		{
			Debug.Log(GetBits(0b11111111111111111111111111111111, 3, 3));
		}

		void Update() {}
	}
}