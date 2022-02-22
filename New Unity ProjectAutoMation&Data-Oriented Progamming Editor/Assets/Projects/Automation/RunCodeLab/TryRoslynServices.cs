using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Automation.RunCodeLab
{

    public class TryRoslynServices : MonoBehaviour
    {
        void Start()
        {


            var netstandard = AppDomain.CurrentDomain.GetAssemblies().First( a => a.FullName.Contains( "netstandard" ) );
            var other = typeof(object).Assembly;
            // Debug.Log( string.Join( "\n", netstandard ) );


            var list_b = typeof(Debug).Assembly.GetReferencedAssemblies().Select( a => a.Name );
            Debug.Log( $"ref_ass:\n {string.Join( "\n", list_b )}" );

            var UnityEngineAssembly = typeof(Object).Assembly;

            var ref_list = new[]
            {
                UnityEngineAssembly,
                Assembly.Load( UnityEngineAssembly.GetReferencedAssemblies().First() )
            };

            var option = ScriptOptions.Default.
                AddImports( "UnityEngine" ).
                AddReferences( ref_list );

            // Debug.Log( string.Join( "\n", ref_list ) );
            var m_script = CSharpScript.Create( "Debug.Log(\"Async Run.\");", option );
            m_script.RunAsync();

        }

    }
}
