
# EasyXMLSerializerUWP #

UWP Version of EasyXMLSerializer.
The UWP - Version got no Events.

## Usage: ##

Example Object
```c#
public class Configuratio
{
   public string Name { get; set; }
   public string Alter { get; set; }
   public Details Details { get; set; }
}

public class Details
{
   public string Vorname { get; set; }
   public string Nachname { get; set; }
}
```

Read from XML File
```c#
var storageFolder = ApplicationData.Current.LocalFolder;
SerializeTool serializer = new SerializeTool(Path.Combine(storageFolder.Path,"testxy.xml"));

Configuratio config = null;
string retMessage = "";
config = await serializer.ReadXmlFileAsync<Configuratio>();
retMessage = serializer.LastError;
if (config == null)
{
   MessageDialog msgDialog = new MessageDialog(retMessage,"Fehelr");
   await msgDialog.ShowAsync();
}
else
{
   MessageDialog msgDialog = new MessageDialog("Wurde geschrieben","OK");
   await msgDialog.ShowAsync();
}
```
Write to XML File
```c#
var storageFolder = ApplicationData.Current.LocalFolder;
SerializeTool serializer = new SerializeTool(Path.Combine(storageFolder.Path,"testxy.xml"));
//Fill the Object to write
Configuratio config = new Configuratio
{
   Name = "TEST",
   Alter = "10"
};
Details details = new Details
{
   Vorname = "Guido",
   Nachname = "Agnesmeyer"
};
config.Details = details;

bool result = await serializer.WriteXmlFileAsync(config);
string retMessage = serializer.LastError;
if (result)
{
   MessageDialog msgDialog = new MessageDialog("The file was written!","OK");
   await msgDialog.ShowAsync();
}
else
{
   MessageDialog msgDialog = new MessageDialog(retMessage,"Error");
   await msgDialog.ShowAsync();
}
```
----
## Please use Visual - Stuido 2017 for this Project. ##
