#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System.Collections.Generic;

namespace ECSharp
{
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC();
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC<in TValue1>(TValue1? value1);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC<in TValue1, in TValue2>(TValue1? value1, TValue2? value2);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC<in TValue1, in TValue2, in TValue3>(TValue1? value1, TValue2? value2, TValue3? value3);

    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC_WITH_PARAMETER(object? parameter);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC_WITH_PARAMETER<in TValue1>(object? parameter, TValue1? value1);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC_WITH_PARAMETER<in TValue1, in TValue2>(object? parameter, TValue1? value1, TValue2? value2);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate void EVENT_FUNC_WITH_PARAMETER<in TValue1, in TValue2, in TValue3>(object? parameter, TValue1? value1, TValue2? value2, TValue3? value3);

    /// <summary>
    /// 事件
    /// </summary>
    public sealed class Event<TKey> where TKey : notnull
    {
        private class FuncData { public EVENT_FUNC? func; public EVENT_FUNC_WITH_PARAMETER? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey, List<FuncData>> funcMap = new Dictionary<TKey, List<FuncData>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC_WITH_PARAMETER func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key">事件名</param>
        public void Call(TKey key)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v1.Count; i < len; i++)
            {
                FuncData data = v1[i];
                if (data.func != null) data.func();
                if (data.func2 != null) data.func2(data.parameter);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v1.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC_WITH_PARAMETER func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func2 == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func2 == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key">事件名</param>
        public bool Clear(TKey key)
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
    /// 事件
    /// </summary>
    public sealed class Event<TKey, TValue1> where TKey : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey, List<FuncData>> funcMap = new Dictionary<TKey, List<FuncData>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC<TValue1?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey key, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v1.Count; i < len; i++)
            {
                FuncData data = v1[i];
                if (data.func != null) data.func(value1);
                if (data.func2 != null) data.func2(data.parameter, value1);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v1.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC<TValue1?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func2 == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func2 == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key">事件名</param>
        public bool Clear(TKey key)
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
    /// 事件
    /// </summary>
    public sealed class Event<TKey, TValue1, TValue2> where TKey : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?, TValue2?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey, List<FuncData>> funcMap = new Dictionary<TKey, List<FuncData>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC<TValue1?, TValue2?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey key, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v1.Count; i < len; i++)
            {
                FuncData data = v1[i];
                if (data.func != null) data.func(value1, value2);
                if (data.func2 != null) data.func2(data.parameter, value1, value2);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null) return;
            for (int i = 0, len = list.Count; i < len; i++)
            {
                v1.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC<TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func2 == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func2 == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key">事件名</param>
        public bool Clear(TKey key)
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
    /// 事件
    /// </summary>
    public sealed class Event<TKey, TValue1, TValue2, TValue3> where TKey : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?, TValue2?, TValue3?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey, List<FuncData>> funcMap = new Dictionary<TKey, List<FuncData>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC<TValue1?, TValue2?, TValue3?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key, out var v1))
            {
                v1 = new List<FuncData>();
                funcMap.Add(key, v1);
            }

            v1.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v1.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key">事件名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey key, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v1.Count; i < len; i++)
            {
                FuncData data = v1[i];
                if (data.func != null) data.func(value1, value2, value3);
                if (data.func2 != null) data.func2(data.parameter, value1, value2, value3);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v1.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC<TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey key, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            for (int i = 0, len = v1.Count; i < len; i++)
            {
                if (v1[i].func2 == func)
                {
                    v1.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                for (int i = v1.Count - 1; i >= 0; i--)
                {
                    if (v1[i].func2 == func)
                    {
                        v1.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key">事件名</param>
        public bool Clear(TKey key)
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
    /// 事件
    /// </summary>
    public sealed class MultiEvent<TKey1, TKey2> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public EVENT_FUNC? func; public EVENT_FUNC_WITH_PARAMETER? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
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

            List<FuncData>? list = null;
            for (int i = 0, len = v2.Count; i < len; i++)
            {
                FuncData data = v2[i];
                if (data.func != null) data.func();
                if (data.func2 != null) data.func2(data.parameter);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v2.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func2 == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func2 == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
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
    /// 事件
    /// </summary>
    public sealed class MultiEvent<TKey1, TKey2, TValue1> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v2.Count; i < len; i++)
            {
                FuncData data = v2[i];
                if (data.func != null) data.func(value1);
                if (data.func2 != null) data.func2(data.parameter, value1);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v2.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func2 == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func2 == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
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
    /// 事件
    /// </summary>
    public sealed class MultiEvent<TKey1, TKey2, TValue1, TValue2> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?, TValue2?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?, TValue2?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v2.Count; i < len; i++)
            {
                FuncData data = v2[i];
                if (data.func != null) data.func(value1, value2);
                if (data.func2 != null) data.func2(data.parameter, value1, value2);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v2.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func2 == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func2 == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
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
    /// 事件
    /// </summary>
    public sealed class MultiEvent<TKey1, TKey2, TValue1, TValue2, TValue3> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public EVENT_FUNC<TValue1?, TValue2?, TValue3?>? func; public EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?>? func2; public object? parameter; public int priority; public int repeat; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, List<FuncData>>>();

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?, TValue2?, TValue3?> func, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }

            v2.Add(new FuncData() { priority = priority, repeat = repeat, func = func });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="priority">优先级 越高越先执行</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func, object? parameter, int priority = 0, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, List<FuncData>>();
                funcMap.Add(key1, v1);
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                v2 = new List<FuncData>();
                v1.Add(key2, v2);
            }
            v2.Add(new FuncData() { priority = priority, repeat = repeat, func2 = func, parameter = parameter });
            v2.Sort((a, b) => -a.priority.CompareTo(b.priority));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="key1">一级事件名</param>
        /// <param name="key2">二级事件名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return;
            }

            List<FuncData>? list = null;
            for (int i = 0, len = v2.Count; i < len; i++)
            {
                FuncData data = v2[i];
                if (data.func != null) data.func(value1, value2, value3);
                if (data.func2 != null) data.func2(data.parameter, value1, value2, value3);
                if (data.repeat > 0) --data.repeat;

                if (data.repeat == 0)
                {
                    if (list == null) list = new List<FuncData>();
                    list.Add(data);
                }
            }

            if (list == null)
                return;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                v2.Remove(list[i]);
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC<TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC<TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                return false;
            }

            if (!v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            for (int i = 0, len = v2.Count; i < len; i++)
            {
                if (v2[i].func2 == func)
                {
                    v2.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove(EVENT_FUNC_WITH_PARAMETER<TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    for (int i = v2.Count - 1; i >= 0; i--)
                    {
                        if (v2[i].func2 == func)
                        {
                            v2.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
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
