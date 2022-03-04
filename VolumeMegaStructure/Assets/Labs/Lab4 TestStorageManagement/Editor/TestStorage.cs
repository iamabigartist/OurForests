using UnityEditor;
using UnityEngine;
namespace Labs.Lab4_TestStorageManagement.Editor
{
    public class TestStorage : EditorWindow
    {
        [MenuItem( "Labs/Labs.Lab4_TestStorageManagement.Editor/TestStorage" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestStorage>();
            window.titleContent = new("TestStorage");
            window.Show();
        }

        int m_data_length;
        int m_capacity;
        int m_segment_length;

        Vector2Int flush_range;

        void AddCap(int l) { m_capacity += l; }
        void RemoveCap(int l) { m_capacity -= l; }

        void Decide(int new_data_length)
        {
            bool overflow = new_data_length > m_capacity;

            if (overflow) { }
        }

        int DecideHowRemove(int data_length, int capacity, int trend)
        {
            if (data_length < capacity / 2)
            {
                return capacity / 2;
            }

            return 0;
        }

        int DecideAdd(int data_length, int capacity)
        {
            if (data_length > capacity)
            {
                return m_segment_length;
            }

            return 0;
        }

        int AdjustsRatio(int data_len, int capacity)
        {
            const float adjust = (0.3f + 1f) / 2f;
            float cur_ratio = (float)data_len / capacity;

            return cur_ratio switch
            {
                < 0.3f => -(int)(data_len / adjust) + capacity,
                >= 1f  => (int)(data_len / adjust) - capacity,
                _      => 0
            };
        }

        void OnEnable()
        {
            m_data_length = 100;
            m_capacity = 100;
            m_segment_length = 10;
        }

        void OnGUI() { }
    }
}
