using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
namespace MyUtils
{
    public static class AsyncUtil
    {
        public static async Task WaitUntil(Func<bool> predicate, int check_time_interval = 100)
        {
            while (!predicate())
            {
                await Task.Delay( check_time_interval );
            }
        }

        public static Task<bool> WaitUntil(Func<bool> predicate, int check_time_interval, int time_out)
        {
            return Task.FromResult( WaitUntil( predicate, check_time_interval ).Wait( time_out ) );
        }

        /// <summary>
        ///     Wait until the <see cref="waited_result" /> turn from null to not null.
        /// </summary>
        /// <returns>the <see cref="waited_result" /></returns>
        /// <example>
        ///     <code>var main_cam = await WaitNull( () => Camera.main );</code>
        /// </example>
        public static async Task<T> WaitNull<T>(Func<T> waited_result, int check_time_interval = 100)
        {
            while (waited_result() == null)
            {
                await Task.Delay( check_time_interval );
            }
            return waited_result();
        }

        /// <inheritdoc cref="WaitNull{T}(System.Func{T},int)" />
        public static Task<T> WaitNull<T>(Func<T> waited_result, int check_time_interval, int time_out)
        {
            var task = WaitNull( waited_result, check_time_interval );
            task.Wait( time_out );
            return Task.FromResult( task.Result );
        }

        public static async Task ToTask(this CustomYieldInstruction instruction)
        {
            await WaitUntil( () => instruction.keepWaiting );
        }

        public static async Task ToTask(this YieldInstruction instruction, MonoBehaviour coroutine_executor)
        {
            var complete = false;
            coroutine_executor.StartCoroutine( instruction.Coroutine( () => { complete = true; } ) );
            await WaitUntil( () => complete );
        }

        public static IEnumerator Coroutine(this YieldInstruction instruction, Action complete)
        {
            yield return instruction;
            complete();
        }
    }
}
