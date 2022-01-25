using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace MyUtils
{
    public static class ReflectionUtil
    {
        public static Type GetMethodDelegateType(this MethodInfo methodInfo)
        {
            var parameter_types = methodInfo.GetParameters().
                Select( p => p.ParameterType ).Append( methodInfo.ReturnType );
            var delegate_type = Expression.GetDelegateType( parameter_types.ToArray() );
            return delegate_type;
        }

        public static Delegate GetInstanceDelegate(this Type type, object instance, MethodInfo methodInfo)
        {
            var @delegate = Delegate.CreateDelegate( methodInfo.GetMethodDelegateType(), instance, methodInfo );
            return @delegate;
        }
        public static Delegate GetStaticDelegate(this Type type, MethodInfo methodInfo)
        {
            var @delegate = Delegate.CreateDelegate( methodInfo.GetMethodDelegateType(), methodInfo );
            return @delegate;
        }

        public static (MethodInfo[], Delegate[]) GetStaticDelegates(this Type type)
        {
            var methods = type.GetMethods( BindingFlags.Static );
            var delegates = methods.Select(
                m => Delegate.CreateDelegate( m.GetMethodDelegateType(), m ) );
            return (methods.ToArray(), delegates.ToArray());
        }
        public static (MethodInfo[], Delegate[]) GetInstanceDelegates(this Type type, object instance)
        {
            var methods = type.GetMethods( BindingFlags.Instance );
            var delegates = methods.Select(
                m => Delegate.CreateDelegate( m.GetMethodDelegateType(), instance, m ) );
            return (methods.ToArray(), delegates.ToArray());
        }

        public static (MethodInfo[], Delegate[]) GetDelegates(this Type type, object instance)
        {
            var (static_methods, static_delegates) = type.GetStaticDelegates();
            var (instance_methods, instance_delegates) = type.GetInstanceDelegates( instance );
            return (static_methods.Concat( instance_methods ).ToArray(),
                    static_delegates.Concat( instance_delegates ).ToArray());
        }


    }
}
