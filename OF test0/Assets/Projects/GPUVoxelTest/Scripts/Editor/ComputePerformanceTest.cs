using UnityEditor;
using UnityEngine;
namespace VoxelTest
{
    public class ComputePerformanceTest : EditorWindow
    {
        [MenuItem("Tests/ComputePerformanceTest")]
        private static void ShowWindow()
        {
            var window = GetWindow<ComputePerformanceTest>();
            window.titleContent = new GUIContent("ComputePerformanceTest");
            window.Show();
        }

        private RenderTexture r;

        public readonly Vector3Int group_size = new Vector3Int(32, 32, 1);
        public Vector3Int size;
        public Vector3Int group_num;
        private ComputeShader shader;
        private double time_used;
        private int ScheduleNumber =>
            this.size.x * this.size.y * this.size.z /
            (this.group_num.x * this.group_num.y * this.group_num.z *
             this.group_size.x * this.group_size.y * this.group_size.z);
        private void OnEnable()
        {
            this.shader = Resources.Load<ComputeShader>("TestRunCompute");
            this.r = new RenderTexture(100, 100, 1);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Run")) TestOnce();
            this.size = EditorGUILayout.Vector3IntField("DataSize", this.size);
            this.group_num = EditorGUILayout.Vector3IntField("GroupNum", this.group_num);
            EditorGUILayout.LabelField($"Take Time: {this.time_used}");
            // GUI.DrawTexture(new Rect(100, 100, 800, 600), this.r);
        }
        private void TestOnce()
        {


            this.r.enableRandomWrite = true;
            var t = ScheduleNumber;
            Debug.Log(t);
            this.shader.SetTexture(0, "Result", this.r);

            var start = EditorApplication.timeSinceStartup;
            for (int i = 0; i < t; i++) this.shader.Dispatch(0, this.group_num.x, this.group_num.y, this.group_num.z);
            var end = EditorApplication.timeSinceStartup;
            this.time_used = end - start;
        }
    }
}