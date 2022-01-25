using System;
namespace MyUtils
{
    public static class ReactiveUtil
    {

        public delegate void ValueChangeAction<T>(T value, T old_value);
        public static bool Update<T>(
            this T new_value,
            ref T value,
            ValueChangeAction<T> value_change_action)
            where T : struct, IEquatable<T>
        {
            if (new_value.Equals( value ))
            {
                return false;
            }
            var old_value = value;
            value = new_value;
            value_change_action( value, old_value );
            return true;
        }
    }
}
