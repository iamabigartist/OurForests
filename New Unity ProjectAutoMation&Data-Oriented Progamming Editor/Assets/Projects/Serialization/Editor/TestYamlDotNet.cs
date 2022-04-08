using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
namespace Serialization
{
    public class TestYamlDotNet : EditorWindow
    {
        [MenuItem( "Labs/Serialization/TestYamlDotNet" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestYamlDotNet>();
            window.titleContent = new("TestYamlDotNet");
            window.Show();
        }


        void OnEnable()
        {
            serializer = new SerializerBuilder().Build();
            emitter_settings = EmitterSettings.Default.WithBestIndent( 4 );

            var graph = new
            {
                DeepDiveStudio = new
                {
                    summary = new
                    {
                        found_date = new DateTime( 2022, 3, 21 ),
                        type = "Indie Game Studio",
                        scale = 20
                    },
                    members = new[]
                    {
                        new
                        {
                            position = "founder",
                            name = "Chenghua",
                            join_date = new DateTime( 2022, 3, 21 )
                        },
                        new
                        {
                            position = "bird1",
                            name = "huahua",
                            join_date = new DateTime( 2022, 4, 5 )
                        }
                    },
                    works = new[]
                    {
                        new
                        {
                            name = "Our Forest",
                            design_progress = 0.5f,
                            dev_progress = 0.4f
                        },

                        new
                        {
                            name = "Flesh Frozen",
                            design_progress = 0.6f,
                            dev_progress = 0.0f
                        }

                    }
                }
            };

            using (var string_writer = new StringWriter())
            {
                var emitter = new Emitter( string_writer, emitter_settings );
                serializer.Serialize( emitter, graph );
                yaml_1 = string_writer.ToString();
            }

            var literal_object = new
            {
                text = yaml_1
            };

            using (var string_writer = new StringWriter())
            {
                var emitter = new Emitter( string_writer, emitter_settings );
                serializer.Serialize( emitter, literal_object );
                literal_yaml = string_writer.ToString();
            }
        }

        EmitterSettings emitter_settings;
        ISerializer serializer;
        string yaml_1;
        string literal_yaml;
        void OnGUI()
        {
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUI.skin.box.wordWrap = true;
            GUILayout.Box( yaml_1 );
            GUILayout.Box( literal_yaml );
        }

    }
}
