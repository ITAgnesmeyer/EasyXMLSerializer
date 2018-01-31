# EasyXMLSerializer
Easy to use XML Serializer

You can download this Component aswall by [NuGet](https://www.nuget.org/packages/EasyXMLSerializer/ "NuGet").

At Package-Manager-Konsole:

    PM> install-package EasyXMLSerializer

Usage
----------

#### Serialize Objects
```c#
//Object to be Serialized
public class TestObject
{
   public string Name { get; set; }
   public string ConnectionString { get; set; }
}
```

##### Serialize the Object to a File.

```c#
using EasyXMLSerializer;
//...

//Calling Code

// Fill the Object
var testObject = new TestObject
{
	Name = "TestName",
	ConnectionString = "Server=myServerAddress;..."
};

//Save the Object to "TestObject.xml"
var serializeTool = new SerializeTool("TestObject.xml");
serializeTool.WriteXmlFile(testObject);

//...
```
Result

```XML
<?xml version="1.0" encoding="utf-8"?>
<TestObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	<Name>TestName</Name>
	<ConnectionString>Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;</ConnectionString>
</TestObject>
```

You can use XMLSerializeation attributes

```c#
using System.Xml.Serialization;

[XmlRoot("Configuration")]
public class TestObject
{
	[XmlAttribute]
	public string Name { get; set; }
	public string ConnectionString { get; set; }
}
```

Result

```XML
<?xml version="1.0" encoding="utf-8"?>
<Configuration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Name="TestName">
	<ConnectionString>Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;</ConnectionString>
</Configuration>
```

Deserialize to Object
----------

Using a File:

```c#
var deserializeTool = new SerializeTool("TestObject.xml");
var testObject = deserializeTool.ReadXmlFile<TestObject>();

Console.WriteLine($@"TestObject.Name=>{testObject.Name}");
Console.WriteLine($@"TestObject.ConnectionString=>{testObject.ConnectionString}");
//...
//Alternative
var deserializeTool = new SerializeTool();
var testObject = deserializeTool.ReadXmlFile<TestObject>("TestObject.xml");
Console.WriteLine($@"TestObject.Name=>{testObject.Name}");
Console.WriteLine($@"TestObject.ConnectionString=>{testObject.ConnectionString}");
```

Using a XML-String:

```c#
var xmlString = "<Configuration Name=\"TestName\"><ConnectionString>Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;</ConnectionString></Configuration>";
var testObject = deserializeTool.ReadXmlFromString<TestObject>(xmlString);

Console.WriteLine($@"TestObject.Name=>{testObject.Name}");
Console.WriteLine($@"TestObject.ConnectionString=>{testObject.ConnectionString}");
```c#

#### I hope this helps in many Projects :)
Please feel free to comment or suggest features.

