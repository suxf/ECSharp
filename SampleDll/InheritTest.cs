using ECSharp;
using ECSharp.Hotfix;
using Sample;
using System;

namespace SampleDll
{
    /** 继承测试 **/
    /// <summary>
    /// A抽象代理实现
    /// </summary>
    public abstract class A_Agent : AbstractAgent, IAgent<A>
    {
        public A self => _self as A;
        public void WriteHelloA()
        {
            self.test1 += 100;
            self.test2 += " world A";
        }
        public abstract void Hello();
        protected override void Initialize()
        {
            self.test1 += 1000;
        }
    }

    /// <summary>
    /// B抽象代理
    /// </summary>
    public class B_Agent : A_Agent, IAgent<B>
    {
        public new B self => _self as B;
        public override void Hello()
        {
            self.test1 += 20;
            self.test3 += " world B";
            Log.Info(self.test1 + "," + self.test2 + "," + self.test3);
        }
        protected override void Initialize()
        {
            base.Initialize();
            self.test1 += 20;
        }
    }

    /// <summary>
    /// C抽象代理
    /// </summary>
    public class C_Agent : A_Agent, IAgent<C>
    {
        public new C self => _self as C;
        public override void Hello()
        {
            self.test1 += 50;
            self.test4 += " world C";
            Log.Info(self.test1 + "," + self.test2 + "," + self.test4);
        }
        protected override void Initialize()
        {
            base.Initialize();
            self.test1 += 10;
        }
    }
}
