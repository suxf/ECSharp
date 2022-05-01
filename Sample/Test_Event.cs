using ES;
using ES.Time;

namespace Sample
{
    internal class Test_Event
    {
        public Test_Event()
        {
            string opcode = Log.ReadLine("事件测试填【1】，指令测试填【2】:");
            if (opcode == "1") TestEvent();
            else if (opcode == "2") TestCommand();
        }

        /// <summary>
        /// 事件测试
        /// </summary>
        public void TestEvent()
        {
            Log.Info("事件测试开始");
            Event<int, string> event1 = new Event<int, string>();
            event1.Add(10, static (string str) => { Log.Info("事件测试1:" + str); }, 0, 1);
            event1.Add(10, static (string str) => { Log.Info("事件测试2:" + str); }, 2);
            event1.Add(10, static (string str) => { Log.Info("事件测试3:" + str); }, -1);
            event1.Add(10, static (string str) => { Log.Info("事件测试4:" + str); }, 1);
            Log.Info("3秒后触发");
            // 3秒后触发
            TimeCaller.Create(static delegate (object e)
            {
                ((Event<int, string>)e).Call(10, "Hello World");
            }, 3000, 1000, 2).Start(event1, true);
            // 多级事件
            MultiEvent<string, int> event2 = new MultiEvent<string, int>();
            event2.Add("test", 1, static () => { Log.Info("多级事件测试"); });
            event2.Call("test", 1);
        }

        /// <summary>
        /// 指令测试
        /// </summary>
        public void TestCommand()
        {
            Log.Info("指令测试开始");
            Command<string, string> command = new Command<string, string>();
            TimeCaller.Create(delegate () { command.Call("test1"); command.Call("test2"); }, 5000).Start(true);
            Log.Info("5秒后触发");
            string str = "Your";
            command.Add("test1", static (object obj) => { Log.Info("回调处理成功1"); return ""; });
            str = command.AddWaitCall("test2", static (object obj) => 
            {
                Log.Info("回调处理成功2");
                return "World";
            }, null/*, 2000*/);
            Log.Info("测试成功: Hello " + str);
        }
    }
}
