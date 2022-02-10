﻿using System.Diagnostics;
namespace MyUtils
{
    public static class ContextUtil
    {
        public static StackFrame GetCurrentFrame()
        {
            return new StackTrace().GetFrame( 1 );
        }

        public static StackFrame GetCallerFrame()
        {
            return new StackTrace().GetFrame( 2 );
        }

    }
}