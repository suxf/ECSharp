using ES.Data.Buffer;
using System.Collections.Generic;


namespace Sample
{
    // 简单序列化数据数据展示
    class Test_ESBuffer
    {
        [System.Obsolete]
        public void test()
        {
            var test = new HelloMessage();
            test.sssgggg = new fgffff();
            test.sssgggg.sdas = "dsad";
            test.sssgggg.wqqwrq = new Dictionary<string, int>() { { "s", 11 } };
            test.ffff = new int[] { 111, 222, 333 };
            test.ssssad = new Dictionary<string, int>() { { "dsada", 11 } };
            test.ssff = new List<Dictionary<string, byte[]>>();
            test.a = new Dictionary<string, fgffff>();
            var sffff = new fgffff();
            sffff.id = 33333;
            sffff.wqqwrq = new Dictionary<string, int>();
            test.a.Add("test", sffff);
            Dictionary<string, byte[]> ssffsdff = new Dictionary<string, byte[]>() { { "213123", new byte[] { 1, 2, 3, 4 } } };
            test.ssff.Add(ssffsdff);
            test.ssff.Add(ssffsdff);
            //Console.WriteLine(ssss.ToString(false) + ":" + ssss.ToString(false).Length);
            // Console.WriteLine(ssss.ToString() + ":" + ssss.ToString().Length);

            // var vvv = ssss.ToBytes();
            // 
            // var sss = HelloMessage.Parse(vvv);


            Easyeeeee e = new Easyeeeee();
            e.errcode = 1;
            e.errmsg = "Hello World";
            // Console.WriteLine(e.ToString() + ":" + e.ToString().Length);
            byte[] eeee = null;
            // Console.WriteLine(e.ToString());
            for (int i = 0, len = 100000; i < len; i++) eeee = e.ToBytes();
            for (int i = 0, len = 100000; i < len; i++) _ = Easyeeeee.Parse(eeee);
        }

        [System.Obsolete]
        class Easyeeeee : EasyBuffer<Easyeeeee>
        {
            public int errcode;
            public string errmsg;
        }

        [System.Obsolete]
        class HelloMessage : EasyBuffer<HelloMessage>
        {
            public string name = "Hello";
            public int age = 111;
            public string[] sss = { "sadad", "dasd" };
            public int[] ffff;
            public Dictionary<string, int> ssssad;
            public Dictionary<string, fgffff> a;
            public List<Dictionary<string, byte[]>> ssff;
            public fgffff sssgggg;
        }

        [System.Obsolete]
        class fgffff : EasyBuffer<fgffff>
        {
            public int id = 1111;
            public string sdas;
            public Dictionary<string, int> wqqwrq;
        }
    }
}
