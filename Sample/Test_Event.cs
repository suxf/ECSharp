using ECSharp;
using ECSharp.Time;

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
            string testStr = "abc";
            Event<int, string> event1 = new Event<int, string>();
            event1.Add(10, static (string str) => { Log.Info("事件测试1:" + str); }, 0, 1);
            event1.Add(10, static (string str) => { Log.Info("事件测试2:" + str); }, 2);
            event1.Add(10, static (string str) => { Log.Info("事件测试3:" + str); }, -1);
            event1.Add(10, static (string str) => { Log.Info("事件测试4:" + str); }, 1, 1);
            event1.Add(10, static (object s, string str) => { Log.Info("事件测试4:" + s); }, testStr, 1);
            Log.Info("3秒后触发");
            // 3秒后触发
            TimeCaller.Create(static delegate (object e)
            {
                Log.Info("触发------------");
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
            MultiCommand<string, string, int> multiCommand = new MultiCommand<string, string, int>();
            int i = 0;
            int waitId1 = command.AutoWaitID;
            int waitId2 = multiCommand.AutoWaitID;
            TimeCaller.Create(delegate () {
                Log.Info($"触发第{i + 1}次------------");
                command.Call("test1");
                command.Call("test2", waitId1);
                if(i == 1)
                {
                    multiCommand.Call("key1", "key2", waitId2);
                }
                i++;
            }, 5000, 1000, 3).Start(true);
            Log.Info("5秒后触发");
            string str = "Your";
            command.Add("test1", static () => { Log.Info("回调处理成功 command 1"); return ""; });
            command.Add("test2", static () => { Log.Info("回调处理成功 Wait command 2"); return "World"; });
            str = command.WaitCall("test2", waitId1);
            Log.Info("测试成功: Hello " + str);

            multiCommand.Add("key1", "key2", static () => { Log.Info("回调处理成功 Wait MultiCommand"); return 1; });
            int code = multiCommand.WaitCall("key1", "key2", waitId2);
            Log.Info("MultiCommand 代码：" + code);
        }
    }
}
