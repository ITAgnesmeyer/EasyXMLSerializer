using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyXMLSerializer.Tests
{
    [TestClass]
    public class XmlSerializerSpeedTest
    {
        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (File.Exists("test9999.xml"))
            {
                for (int i = 0; i < 10000; i++)
                {
                    File.Delete("test" + i + ".xml");
                }

            }
        }
        [TestMethod]
        public void TestNewSerializerForEveryRowWriteFile()
        {
            SerializeTool serializer = new SerializeTool();
            for (int i = 0; i < 10000; i++)
            {
                TestObject testObject = SerializeToolTests.GetTestObject();
                if (!serializer.WriteXmlFile(testObject, "test" + i + ".xml"))
                {
                    Assert.Fail(serializer.LastError);
                }
            }

        }
        [TestMethod]
        public void TestSingleSerializerForTypeWriteFile()
        {
            Type[] types = new Type[] {typeof(TestObject)};
            SerializeTool serializer = new SerializeTool(types);
            for (int i = 0; i < 10000; i++)
            {
                TestObject testObject = SerializeToolTests.GetTestObject();
                if (!serializer.WriteXmlFile(testObject, "test" + i + ".xml"))
                {
                    Assert.Fail(serializer.LastError);
                }
            }

            //for (int i = 0; i < 1000; i++)
            //{
            //    File.Delete("test" + i + ".xml");
            //}
        }
        [TestMethod]
        public void TestSingleSerializerForTypeReadString()
        {
            Type[] types = new Type[] {typeof(TestObject)};
            SerializeTool serializer = new SerializeTool(types);
            for (int i = 0; i < 10000; i++)
            {
                var testObject = serializer.ReadXmlFromString<TestObject>(SerializeToolTests.OBJ_RESULT);
                if (testObject == null)
                    Assert.Fail(serializer.LastError);
            }
        }

        [TestMethod]
        public void TestNewSerializerForEveryRowReadString()
        {
            SerializeTool serializer = new SerializeTool();
            for (int i = 0; i < 10000; i++)
            {
                var testObject = serializer.ReadXmlFromString<TestObject>(SerializeToolTests.OBJ_RESULT);
                if (testObject == null)
                    Assert.Fail(serializer.LastError);
            }
        }

        [TestMethod]
        public void TestSingleSerializerForTypeWriteSingleFile()
        {
            Type[] types = new Type[] {typeof(TestObject), typeof(List<TestObject>)};
            SerializeTool serializer = new SerializeTool(types);
            List<TestObject> testObjects = new List<TestObject>();
            for (int i = 0; i < 10000; i++)
            {
                testObjects.Add(SerializeToolTests.GetTestObject());
            }

            if (!serializer.WriteXmlFile(testObjects, "test_objects.xml"))
            {
                Assert.Fail(serializer.LastError);
            }
        }
        [TestMethod]
        public void TestNewSerializerForEveryRowWriteSingleFile()
        {
            SerializeTool serializer = new SerializeTool();
            List<TestObject> testObjects = new List<TestObject>();
            for (int i = 0; i < 10000; i++)
            {
                testObjects.Add(SerializeToolTests.GetTestObject());
            }

            if (!serializer.WriteXmlFile(testObjects, "test_objects.xml"))
            {
                Assert.Fail(serializer.LastError);
            }
        }
    }
}