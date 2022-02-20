using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Automation.RunCodeLab
{

    public class TryRoslynServices : MonoBehaviour
    {
        NameSyntax a;
        void Start()
        {
            a = IdentifierName( "System" );
            Debug.Log(a.Language);
        }

    }
}
