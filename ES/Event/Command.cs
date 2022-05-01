using ES.Alias.Collections;
using System.Threading;

namespace ES
{
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult>(object? target);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1>(object? target, TValue1 value1);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1, in TValue2>(object? target, TValue1 value1, TValue2 value2);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1, in TValue2, in TValue3>(object? target, TValue1 value1, TValue2 value2, TValue3 value3);

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey, FuncData> funcMap = new Map<TKey, FuncData>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult> func, int repeat = -1)
        {
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey key, COMMAND_FUNC<TResult> func, object? parameter = null, int waitTimeout = -1)
        {
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            funcMap.Add(key, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        public void Call(TKey key)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }
            if (v1.func != null) v1.result = v1.func(v1.parameter);
            v1.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Remove(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey, FuncData> funcMap = new Map<TKey, FuncData>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult, TValue1> func, int repeat = -1)
        {
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey key, COMMAND_FUNC<TResult, TValue1> func, object? parameter = null, int waitTimeout = -1)
        {
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            funcMap.Add(key, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey key, TValue1 value1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }
            if (v1.func != null) v1.result = v1.func(v1.parameter, value1);
            v1.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Remove(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1, TValue2> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1, TValue2>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey, FuncData> funcMap = new Map<TKey, FuncData>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult, TValue1, TValue2> func, int repeat = -1)
        {
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey key, COMMAND_FUNC<TResult, TValue1, TValue2> func, object? parameter = null, int waitTimeout = -1)
        {
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            funcMap.Add(key, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey key, TValue1 value1, TValue2 value2)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }
            if (v1.func != null) v1.result = v1.func(v1.parameter, value1, value2);
            v1.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Remove(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1, TValue2, TValue3> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1, TValue2, TValue3>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey, FuncData> funcMap = new Map<TKey, FuncData>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult, TValue1, TValue2, TValue3> func, int repeat = -1)
        {
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey key, COMMAND_FUNC<TResult, TValue1, TValue2, TValue3> func, object? parameter = null, int waitTimeout = -1)
        {
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            funcMap.Add(key, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey key, TValue1 value1, TValue2 value2, TValue3 value3)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }
            if (v1.func != null) v1.result = v1.func(v1.parameter, value1, value2, value3);
            v1.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Remove(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey1, Map<TKey2, FuncData>> funcMap = new Map<TKey1, Map<TKey2, FuncData>>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult> func, int repeat = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult> func, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            v1.Add(key2, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public void Call(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }
            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }
            if (v2.func != null) v2.result = v2.func(v2.parameter);
            v2.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;
            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey1, Map<TKey2, FuncData>> funcMap = new Map<TKey1, Map<TKey2, FuncData>>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1> func, int repeat = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1> func, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            v1.Add(key2, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1 value1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }
            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }
            if (v2.func != null) v2.result = v2.func(v2.parameter, value1);
            v2.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;
            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1, TValue2> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1, TValue2>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey1, Map<TKey2, FuncData>> funcMap = new Map<TKey1, Map<TKey2, FuncData>>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1, TValue2> func, int repeat = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1, TValue2> func, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            v1.Add(key2, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1 value1, TValue2 value2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }
            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }
            if (v2.func != null) v2.result = v2.func(v2.parameter, value1, value2);
            v2.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;
            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1, TValue2, TValue3> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult, TValue1, TValue2, TValue3>? func; public ManualResetEventSlim? waitHandle; public int repeat; public object? parameter; }
        private readonly Map<TKey1, Map<TKey2, FuncData>> funcMap = new Map<TKey1, Map<TKey2, FuncData>>();

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1, TValue2, TValue3> func, int repeat = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? AddWaitCall(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult, TValue1, TValue2, TValue3> func, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Map<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            FuncData data = new FuncData() { repeat = 1, func = func, parameter = parameter, waitHandle = new ManualResetEventSlim(false) };
            v1.Add(key2, data);
            data.waitHandle.Reset();
            data.waitHandle.Wait(waitTimeout);
            return data.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1 value1, TValue2 value2, TValue3 value3)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }
            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }
            if (v2.func != null) v2.result = v2.func(v2.parameter, value1, value2, value3);
            v2.waitHandle?.Set();
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;
            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }
}
