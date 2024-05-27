using System;
using System.Threading.Tasks;

namespace SharedServices.ObjectStorage.V1
{
    public static partial class AsyncUtil
    {
        public static Task CallbackToTask(Action<Action> action)
        {
            var tcs = new TaskCompletionSource<object>();
            action(() => tcs.SetResult(null));
            return tcs.Task;
        }
        
        public static Task<T0> CallbackToTask<T0>(Action<Action<T0>> action)
        {
            var tcs = new TaskCompletionSource<T0>();
            action(tcs.SetResult);
            return tcs.Task;
        }
        
        public static Task<(T0, T1)> CallbackToTask<T0, T1>(Action<Action<T0, T1>> action)
        {
            var tcs = new TaskCompletionSource<(T0, T1)>();
            action((t0, t1) => tcs.SetResult((t0, t1)));
            return tcs.Task;
        }
        
        public static Task<(T0, T1, T2)> CallbackToTask<T0, T1, T2>(Action<Action<T0, T1, T2>> action)
        {
            var tcs = new TaskCompletionSource<(T0, T1, T2)>();
            action((t0, t1, t2) => tcs.SetResult((t0, t1, t2)));
            return tcs.Task;
        }
        
        public static Task<(T0, T1, T2, T3)> CallbackToTask<T0, T1, T2, T3>(Action<Action<T0, T1, T2, T3>> action)
        {
            var tcs = new TaskCompletionSource<(T0, T1, T2, T3)>();
            action((t0, t1, t2, t3) => tcs.SetResult((t0, t1, t2, t3)));
            return tcs.Task;
        }
        
        public static Task<(T0, T1)> CallbackToTask<T0, T1>(Action<Action<T0>> action1, Action<Action<T1>> action2)
        {
            var tcs = new TaskCompletionSource<(T0, T1)>();
            var result = (default(T0), default(T1));
            var receivedCallback1 = false;
            var receivedCallback2 = false;
            action1((t0) =>
            {
                receivedCallback1 = true;
                result.Item1 = t0;
                if (receivedCallback2) tcs.SetResult(result);
            });
            action2((t1) =>
            {
                receivedCallback2 = true;
                result.Item2 = t1;
                if (receivedCallback1) tcs.SetResult(result);
            });
            return tcs.Task;
        }
        
        public static Task<T1> CallbackToTask<T0, T1>(T0 arg0, Action<T0, Action<T1>> action)
        {
            var tcs = new TaskCompletionSource<T1>();
            action(arg0, tcs.SetResult);
            return tcs.Task;
        }
        
        public static Task<T2> CallbackToTask<T0, T1, T2>(T0 arg0, T1 arg1, Action<T0, T1, Action<T2>> action)
        {
            var tcs = new TaskCompletionSource<T2>();
            action(arg0, arg1, tcs.SetResult);
            return tcs.Task;
        }
        
        public static Task<T3> CallbackToTask<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, Action<T0, T1, T2, Action<T3>> action)
        {
            var tcs = new TaskCompletionSource<T3>();
            action(arg0, arg1, arg2, tcs.SetResult);
            return tcs.Task;
        }
        
        public static Task<(T1, T2)> CallbackToTask<T0, T1, T2>(T0 arg0, Action<T0, Action<T1, T2>> action)
        {
            var tcs = new TaskCompletionSource<(T1, T2)>();
            var result = (default(T1), default(T2));
            action(arg0, (t1, t2) =>
            {
                result.Item1 = t1;
                result.Item2 = t2;
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
        
        public static Task<(T1, T2, T3)> CallbackToTask<T0, T1, T2, T3>(T0 arg0, Action<T0, Action<T1, T2, T3>> action)
        {
            var tcs = new TaskCompletionSource<(T1, T2, T3)>();
            var result = (default(T1), default(T2), default(T3));
            action(arg0, (t1, t2, t3) =>
            {
                result.Item1 = t1;
                result.Item2 = t2;
                result.Item3 = t3;
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
        
        public static Task<(T2, T3)> CallbackToTask<T0, T1, T2, T3>(T0 arg0, T1 arg1, Action<T0, T1, Action<T2, T3>> action)
        {
            var tcs = new TaskCompletionSource<(T2, T3)>();
            var result = (default(T2), default(T3));
            action(arg0, arg1, (t2, t3) =>
            {
                result.Item1 = t2;
                result.Item2 = t3;
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
        
        public static Task<T4> CallbackToTask<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, Action<T0, T1, T2, T3, Action<T4>> action)
        {
            var tcs = new TaskCompletionSource<T4>();
            action(arg0, arg1, arg2, arg3, tcs.SetResult);
            return tcs.Task;
        }
    }
}