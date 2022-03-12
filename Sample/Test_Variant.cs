﻿using ES.Variant;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sample
{
    public enum TestEnum
    {
        A,
        B,
        C,
    }
    /// <summary>
    /// 可变变量测试
    /// </summary>
    class Test_Variant
    {
        public Test_Variant()
        {
            Var enumValue = TestEnum.C;
            TestEnum enumValue2 = enumValue.ToEnum<TestEnum>();

            Var oo1 = 2L;
            Var oo2 = 22;
            Var oo3 = oo2 - oo1;
            oo1 = oo3 == oo2 - 2;
            if(oo1)
            {
                oo1 = 22222F;
            }

            VarList varlist1 = new VarList() + 123123 + 21323 + "fafasfasf" + "324gsg";
            varlist1 += "gewtt";
            varlist1 += "gewtt";
            varlist1 += 1232131;
            varlist1 += 4443333333333444444;
            varlist1 += 213123213123;
            varlist1 += "fafasfasf";
            byte[] tst = varlist1.GetBytes();
            VarList varlist22 = VarList.Parse(tst, out var len2223);

            TestVarMap();
            TestVarList();

            uint hhh = 1;
            Var hhhh = hhh;
            uint hh = hhhh;

            Var uu = 255;
            byte[] uu2 = uu.GetBytes();
            Var uu3 = uu2;
            uu3 *= uu3 + uu3 / 32;

            uu = 66810;
            bool s = uu!= VarType.BYTE;

            Var ssf = 222222220;
            s = uu.Equals(uu);
            s = uu.Equals(ssf);
            int sss = ssf.GetHashCode();
            int sss2 = uu.GetHashCode();

            byte[] yyy = Var.Empty.GetBytes();
            Var yyy2 = Var.Parse(yyy, out var len222);

            bool g = Var.Empty.Equals(sss);

            Var cc = 230;
            var buff = cc.GetBytes();
            Var bb = buff;
            Var dd = Var.Parse(buff, out var len);
            cc = "ffffffffffffffffffffffffffffffffffffffffffffffaasdasd";
            var bb1 = cc.GetBytes();
            bb = bb1;

            byte sf = 1;
            Var sff = sf;
            long ff = 111L + sff;

            VarList varlist = new VarList();
            varlist += 2;
            varlist += 2;
            varlist.Merge(varlist).Add(2, 6,7,8).Merge(varlist);

            var varlist3 = new VarList() + 2 + false + 30.0;
            varlist3.Add(false);

            VarList varlist2 = new VarList();
            varlist2 += varlist;
            varlist2[0] = 3;
            varlist[1] = false;

            foreach (Var it in varlist)
            {
                Console.WriteLine(it.ToString());
            }

            Var t = 1L;var gss = t.GetBytes();
            long c = t++;
            // t = false;
            Console.WriteLine($"{t}");

            for(int i = 0; i < 10; i++)
            {
                Calc();
                Console.WriteLine($"test index:{i}\n");
            }
        }

        public void Calc()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Var test_1 = 101;
            for (int i = 1; i < 100000000; i++)
            {
                test_1 = i + "";
            }
            watch.Stop();
            Console.WriteLine($"test_1:{watch.Elapsed}, {test_1}");
            watch.Restart();
            Var test_2 = 101;
            for (int j = 1; j < 100000000; j++)
            {
                if (test_2 == 0) test_2 = 1;
                test_2 *= j;
            }
            watch.Stop();
            Console.WriteLine($"test_2:{watch.Elapsed}, {test_2}");
        }

        /// <summary>
        /// 测试复合字典
        /// </summary>
        public void TestVarMap()
        {
            VarMap map = new VarMap();
            map.Add(10, "safas");
            map.Add("sdsd", 222);
            VarMap map1 = new VarMap();
            map1.Add("sss", "asdadasd");
            map1.Add("qwe", 1111);
            map1.Add("qwe2", 1111);
            VarList list1 = new VarList();
            list1.Add(10);
            list1.Add("asd");
            map1.Add("cee", list1);
            VarList list2 = new VarList();
            list2.Add(222);
            list2.Add("2222");
            map.Add(100, list2);
            map.Add(11, map1);
            VarList list3 = new VarList();
            VarMap map11 = new VarMap();
            map11.Add("name", "ss");
            map11.Add("sex", 1);
            VarMap map22 = new VarMap();
            map22.Add("name", "233");
            map22.Add("sex", 2);
            list3.Add(map11);
            list3.Add(map22);
            map.Add("player", list3);

            byte[] data = map.GetBytes();
            VarMap map100 = VarMap.Parse(data, out int length);

            VarMap map200 = new VarMap();
            map200.Add(10, 0);
            map200.Add(20, 20);
            map200.Add(30, 20);
            byte[] data2 = map200.GetBytes();
            VarMap map300 = VarMap.Parse(data2, out int length2);
        }

        /// <summary>
        /// 测试复合列表
        /// </summary>
        public void TestVarList()
        {
            VarList list = new VarList();
            list.Add(10, "safas");
            list.Add("sdsd", 222);
            VarMap map1 = new VarMap();
            map1.Add("sss", "asdadasd");
            map1.Add("qwe", 1111);
            map1.Add("qwe2", 1111);
            VarList list1 = new VarList();
            list1.Add(10);
            list1.Add("asd");
            map1.Add("cee", list1);
            map1.Add(123, "asf");
            VarList list2 = new VarList();
            list2.Add(222);
            list2.Add("2222");
            list.Add(list2);
            list.Add(map1);
            VarList list3 = new VarList();
            VarMap map11 = new VarMap();
            map11.Add("name", "ss");
            map11.Add("sex", 1);
            VarMap map22 = new VarMap();
            map22.Add("name", "233");
            map22.Add("sex", 2);
            list3.Add(map11);
            list3.Add(map22);
            list.Add(list3);

            byte[] data = list.GetBytes();
            VarList list100 = VarList.Parse(data);
            bool s = VarList.TryParse(data, out var list200);
            VarMap map100 = VarMap.Parse(map22.GetBytes());
            bool ss = VarMap.TryParse(map22.GetBytes(), out var map200);

            JArray json1 = list100.ToJson();
            JObject json2 = map100.ToJson();
            string str1 = list100.ToString();
            string str2 = map100.ToString();

            Var varList = new Var(list100);
            string str3 = varList.ToString();

            VarList list40 = VarList.Parse(str1);
            VarMap map40 = VarMap.Parse(str2);

            VarList list50 = VarList.Parse(json1);
            VarMap map50 = VarMap.Parse(json2);
        }
    }
}
