using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace SystemTest
{
    /// <summary>
    /// Try to implement a totally responsive system static class for the terrain assets and management.
    /// </summary>
    public static class TerrainManager
    {
        static HashSet<string> available_contexts=new HashSet<string>()
        {
            "GameMain",
        };
        static public void Init([CallerMemberName] string caller_name = "Default")
        {
            if (!available_contexts.Contains( caller_name ))
            {
                throw new ApplicationException(
                    $"Can not call " +
                    $"{MethodBase.GetCurrentMethod().Name} " +
                    $"from {caller_name}." );
            }
            Debug.Log("Init terrain.");
        }
    }
}
