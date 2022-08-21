using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sample
{
    internal class Test_Json2Object
    {
        public Test_Json2Object()
        {
            Dictionary<int, ClassA> configA = new Dictionary<int, ClassA>();
            ClassA classA = new ClassA();
            classA.Id = 1;
            classA.Name = "test";
            classA.Description = "test";
            ClassB classB = new ClassB();
            classB.Id = "10";
            classB.data.Add(1);
            classB.data.Add(0);
            classB.data.Add(2);
            classB.data.Add(3);
            classA.Children.Add(classB);
            ClassA classC = new ClassA();
            classC.Id = 2;
            classC.Name = "test2";
            classC.Description = "test2";
            classA.Props.Add("111", classC);
            configA.Add(123, classA);
            string json = JsonConvert.SerializeObject(configA);

            Dictionary<int, ClassA> config = JsonConvert.DeserializeObject<Dictionary<int, ClassA>>(json);
        }

        public class ClassA
        {
            public int Id;
            public string Name;
            public string Description;
            public List<ClassB> Children = new List<ClassB>();
            public Dictionary<string, ClassA> Props = new Dictionary<string, ClassA>();
        }

        public class ClassB
        {
            public string Id;
            public bool Sex = false;
            public int Age = 1;
            public List<int> data = new List<int>();
        }
    }
}
