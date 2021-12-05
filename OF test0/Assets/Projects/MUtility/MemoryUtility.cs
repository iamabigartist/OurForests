using System.IO;
using System.Runtime.Serialization.Json;
namespace MUtility
{
    public static class MemoryUtility
    {
        public static long GetObjectByte<T>(this T t) where T : class
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer( typeof(T) );
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.WriteObject( stream, t );
                return stream.Length;
            }
        }
    }
}
