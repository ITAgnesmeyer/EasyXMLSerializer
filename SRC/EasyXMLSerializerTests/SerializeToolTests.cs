using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using EasyXMLSerializer.Validation;

namespace EasyXMLSerializer.Tests
{
    public class TestObject
    {
        public string StringValue{get;set;}
        public bool BoolValue{get;set;}
        public int IntValue{get;set;}
        public short ShortValue{get;set;}
        public byte ByteValue{get;set;}
        public float FloatValue{get;set;}
        public double DoubleValue{get; set; }
        public decimal DecimalValue{get;set;}
        public DateTime DateTimeValue{get;set;}
        public object ObjectValue{get;set;}
       
    }

    [TestClass()]
    public class SerializeToolTests
    {
        public const string OBJ_RESULT =
            @"<TestObject xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <StringValue>string</StringValue>
  <BoolValue>true</BoolValue>
  <IntValue>1</IntValue>
  <ShortValue>200</ShortValue>
  <ByteValue>1</ByteValue>
  <FloatValue>1.5</FloatValue>
  <DoubleValue>1.6553</DoubleValue>
  <DecimalValue>1.3232</DecimalValue>
  <DateTimeValue>2020-05-30T00:00:00</DateTimeValue>
  <ObjectValue xsi:type=""xsd:string"">hallo</ObjectValue>
</TestObject>";

        public static TestObject GetTestObject()
        {
            TestObject testObject = new TestObject();
            testObject.StringValue = "string";
            testObject.BoolValue = true;
            testObject.IntValue = 1;
            testObject.ShortValue = 200;
            testObject.ByteValue = 1;
            testObject.FloatValue = 1.5f;
            testObject.DoubleValue = 1.6553d;
            testObject.DecimalValue = (decimal) 1.3232;
            testObject.DateTimeValue = new DateTime(2020, 05, 30);
            testObject.ObjectValue = "hallo";
            return testObject;
        }

        public static void ComparerObjectValues(TestObject expectedObject, TestObject testObject)
        {
            Assert.AreEqual(expectedObject.BoolValue, testObject.BoolValue);
            Assert.AreEqual(expectedObject.ByteValue, testObject.ByteValue);
            Assert.AreEqual(expectedObject.DateTimeValue, testObject.DateTimeValue);
            Assert.AreEqual(expectedObject.DecimalValue, testObject.DecimalValue);
            Assert.AreEqual(expectedObject.DoubleValue, testObject.DoubleValue);
            Assert.AreEqual(expectedObject.FloatValue, testObject.FloatValue);
            Assert.AreEqual(expectedObject.IntValue, testObject.IntValue);
            Assert.AreEqual(expectedObject.ShortValue, testObject.ShortValue);
            Assert.AreEqual(expectedObject.StringValue, testObject.StringValue);
            Assert.AreEqual(expectedObject.ObjectValue, testObject.ObjectValue);
        }

        [TestMethod()]
        public void SerializeToolTest()
        {
            
            SerializeTool serializer = new SerializeTool("testObject.xml");
            TestObject expectedObject = GetTestObject();
            TestObject testObject = serializer.ReadXmlFile<TestObject>();
            Assert.IsNotNull(testObject, serializer.LastError);
            ComparerObjectValues(expectedObject, testObject);
        }

        [TestMethod()]
        public void SerializeToolTest1()
        {
            SerializeTool serialize = new SerializeTool("test1.xml");
            TestObject testObject = GetTestObject();
            if (!serialize.WriteXmlFile(testObject))
            {
                Assert.Fail(serialize.LastError);
            }

            string xmlString = File.ReadAllText("test1.xml");
            Assert.AreEqual(OBJ_RESULT, xmlString);
        }

        [TestMethod()]
        public void SerializeToolTest2()
        {
            
            SerializeTool serializer = new SerializeTool(new Type[] {typeof(TestObject)});
            if (serializer.WriteXmlToString(GetTestObject(), out string result))
            {
                Assert.AreEqual(OBJ_RESULT, result);
            }
            else
            {
                Assert.Fail(serializer.LastError);
            }
        }

        [TestMethod()]
        public void SerializeToolTest3()
        {
            SerializeTool serializer = new SerializeTool("testObject.xml",new Type[] {typeof(TestObject)});
            TestObject testObject = serializer.ReadXmlFile<TestObject>();
            Assert.IsNotNull(testObject);
            TestObject expected = GetTestObject();
            ComparerObjectValues(expected, testObject);
        }

        [TestMethod()]
        public void ReadXmlFromStringTest()
        {
            SerializeTool serialize = new SerializeTool();
            var testObject = serialize.ReadXmlFromString<TestObject>(OBJ_RESULT);
            Assert.IsNotNull(testObject, serialize.LastError);
            TestObject sourceObject = GetTestObject();
            ComparerObjectValues(sourceObject, testObject);

        }

        [TestMethod()]
        public void ReadXmlFromStreamTest()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(OBJ_RESULT)))
            {
                SerializeTool serializer = new SerializeTool();
                var testObject = serializer.ReadXmlFromStream<TestObject>(stream);
                Assert.IsNotNull(testObject,serializer.LastError);
                var sourceObject = GetTestObject();
                ComparerObjectValues(sourceObject, testObject);

            }
        }

        [TestMethod()]
        public void WriteXmlToStringTest()
        {
            SerializeTool serializer = new SerializeTool();
            var testObject = GetTestObject();
            if (serializer.WriteXmlToString(testObject, out string result))
            {
                Assert.AreEqual(OBJ_RESULT, result);
            }
            else
            {
                Assert.Fail(serializer.LastError);
            }
        }

        [TestMethod()]
        public void WriteXmlToStreamTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SerializeTool serializer = new SerializeTool();
                var testObject = GetTestObject();
                if (serializer.WriteXmlToStream(testObject, stream))
                {
                    string result = "";
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    Assert.AreEqual(OBJ_RESULT, result);
                }
                else
                {
                    Assert.Fail(serializer.LastError);
                }
            }
        }

        [TestMethod()]
        public void ReadXmlFileTest()
        {
            SerializeTool serializer = new SerializeTool("testObject.xml");
            TestObject expectedObject = GetTestObject();
            TestObject testObject = serializer.ReadXmlFile<TestObject>();
            Assert.IsNotNull(testObject, serializer.LastError);
            ComparerObjectValues(expectedObject, testObject);
        }

        [TestMethod()]
        public void WriteXmlFileTest()
        {
            SerializeTool serialize = new SerializeTool("WriteFileTest.xml");
            TestObject testObject = GetTestObject();
            if (!serialize.WriteXmlFile(testObject))
            {
                Assert.Fail(serialize.LastError);
            }

            string xmlString = File.ReadAllText("WriteFileTest.xml");
            Assert.AreEqual(OBJ_RESULT, xmlString);
        }

        [TestMethod()]
        public void ReadXmlFileTest1()
        {
            SerializeTool serializer = new SerializeTool();
            TestObject expectedObject = GetTestObject();
            TestObject testObject = serializer.ReadXmlFile<TestObject>("testObject.xml");
            Assert.IsNotNull(testObject, serializer.LastError);
            ComparerObjectValues(expectedObject, testObject);
        }

        [TestMethod()]
        public void WriteXmlFileTest1()
        {
            SerializeTool serialize = new SerializeTool();
            TestObject testObject = GetTestObject();
            if (!serialize.WriteXmlFile(testObject,"WriteFileTest1.xml"))
            {
                Assert.Fail(serialize.LastError);
            }

            string xmlString = File.ReadAllText("WriteFileTest1.xml");
            Assert.AreEqual(OBJ_RESULT, xmlString);
        }

        [TestMethod()]
        public void GetLogTextTest()
        {
            SerializeTool serialize = new SerializeTool();
            TestObject testObject = serialize.ReadXmlFile<TestObject>();
            string result = serialize.GetLogText();
            Assert.AreEqual("Der Wert darf nicht NULL sein.\r\nParametername: inputUri\r\n", result);
        }

        [TestMethod(), TestCategory("Validator")]
        public void GetDtdValidatorTest()
        {
            SerializeTool serialize = new SerializeTool();
            var validator =  serialize.GetDtdValidator("testObjectToValidate.xml");
            Assert.IsNotNull(validator);
            if (validator.Validate())
            {
                Debug.Print("OK");
            }
            else
            {
                string message = "";
                foreach (ValidationErrorInfo validatorLastException in validator.LastExceptions)
                {
                    message += "\n";
                    message += $"{validatorLastException.Message} (row/col)=>{validatorLastException.LineNumber}/{validatorLastException.LinePosition}";
                }

                Assert.Fail(message);
            }

        }

        [TestMethod(), TestCategory("Validator")]
        public void GetDtdValidatorTest1()
        {
            SerializeTool serialize = new SerializeTool("testObjectToValidate.xml");
            var validator =  serialize.GetDtdValidator();
            Assert.IsNotNull(validator);
            if (validator.Validate())
            {
                Debug.Print("OK");
            }
            else
            {
                string message = "";
                foreach (ValidationErrorInfo validatorLastException in validator.LastExceptions)
                {
                    message += "\n";
                    message += $"{validatorLastException.Message} (row/col)=>{validatorLastException.LineNumber}/{validatorLastException.LinePosition}";
                }

                Assert.Fail(message);
            }
        }
        [TestMethod(), TestCategory("Validator")]
        public void GetXsdValidatorTest()
        {
            SerializeTool serialize = new SerializeTool();
            var validator = serialize.GetXsdValidator("testobjectToValidate1.xml");
            Assert.IsNotNull(validator);
            if (validator.Validate())
            {
                Debug.Print("OK");
            }
            else
            {
                string message = "";
                foreach (ValidationErrorInfo validatorLastException in validator.LastExceptions)
                {
                    message += "\n";
                    message += $"{validatorLastException.Message} (row/col)=>{validatorLastException.LineNumber}/{validatorLastException.LinePosition}";
                }

                Assert.Fail(message);
            }
        }
        [TestMethod(), TestCategory("Validator")]
        public void GetXmlValidatorFaialTest()
        {
            SerializeTool serialize = new SerializeTool();
            var validator = serialize.GetXsdValidator("testobjectToValidate1Fail.xml");
            Assert.IsNotNull(validator);
            if (validator.Validate())
            {
                Assert.Fail("the file must be invalid!");
            }
            else
            {
                string message = "";
                foreach (ValidationErrorInfo validatorLastException in validator.LastExceptions)
                {
                    message += "\n";
                    message += $"{validatorLastException.Message} (row/col)=>{validatorLastException.LineNumber}/{validatorLastException.LinePosition}";
                }

                Assert.IsTrue(message.Length > 0);
                Assert.IsTrue(message.Contains("Elemente: 'ObjectValue'"));
            }
        }
    }
}