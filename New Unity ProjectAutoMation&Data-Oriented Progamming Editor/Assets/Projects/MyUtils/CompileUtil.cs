using System.Linq;
using System.Reflection;
namespace MyUtils
{
    public static class CompileUtil
    {
        public static Assembly[] GetLocatable(this Assembly[] assemblies)
        {
            return assemblies.Where( a => a.Location != string.Empty ).ToArray();
        }
    }
}
