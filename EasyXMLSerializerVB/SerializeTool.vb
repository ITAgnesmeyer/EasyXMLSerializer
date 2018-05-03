Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization



Public Class SerializeTool
    Private ReadOnly _logStringBuilder As StringBuilder

    Public Event LogEvent As EventHandler(Of LogEventArgs)

    Public Sub New()
        _logStringBuilder = New StringBuilder()
    End Sub

    Public Sub New(ByVal fileName As String)
        ConfigurationFileName = fileName
    End Sub

    Private Sub MappEvents(ByVal serializer As XmlSerializer)
        If serializer Is Nothing Then Return
        AddHandler serializer.UnknownAttribute, AddressOf OnUnknownAttribute
        AddHandler serializer.UnknownElement, AddressOf OnUnknownElement
        AddHandler serializer.UnknownNode, AddressOf OnUnknownNode
        AddHandler serializer.UnreferencedObject, AddressOf OnUnreferencedObject
    End Sub

    Private Sub DisposeXmlSerializer(ByVal serializer As XmlSerializer)
        If serializer Is Nothing Then Return
        RemoveHandler serializer.UnknownAttribute, AddressOf OnUnknownAttribute
        RemoveHandler serializer.UnknownElement, AddressOf OnUnknownElement
        RemoveHandler serializer.UnknownNode, AddressOf OnUnknownNode
        RemoveHandler serializer.UnreferencedObject, AddressOf OnUnreferencedObject
    End Sub

    Private Function NewXmlSerializer(ByVal objectType As Type) As XmlSerializer
        _logStringBuilder.Clear()
        Dim returnSerializer As XmlSerializer = New XmlSerializer(objectType)
        MappEvents(returnSerializer)
        Return returnSerializer
    End Function

    Private Sub OnUnreferencedObject(ByVal sender As Object, ByVal e As UnreferencedObjectEventArgs)
        Dim value = $"UnreferecedObject:{e.UnreferencedObject.ToString()} ID:{e.UnreferencedId}"
        _logStringBuilder.AppendLine(value)
    End Sub

    Private Sub OnUnknownNode(ByVal sender As Object, ByVal e As XmlNodeEventArgs)
        Dim value = $"UnknownNode:{e.Name} =>L:{e.LineNumber},C:{e.LinePosition}"
        _logStringBuilder.AppendLine(value)
    End Sub

    Private Sub OnUnknownElement(ByVal sender As Object, ByVal e As XmlElementEventArgs)
        Dim value = $"UnknownElement:{e.ExpectedElements},L:{e.LineNumber},C:{e.LinePosition}"
        _logStringBuilder.AppendLine(value)
    End Sub

    Private Sub OnUnknownAttribute(ByVal sender As Object, ByVal e As XmlAttributeEventArgs)
        Dim value = $"UnknownAttribute:{e.ExpectedAttributes},L:{e.LineNumber},C:{e.LinePosition}"
        _logStringBuilder.AppendLine(value)
    End Sub

    Public Function ReadXmlFromString(Of T)(ByVal xmlString As String) As T
        Dim returnObject As T = Nothing
        Dim serializer As XmlSerializer = Nothing
        Try
            serializer = NewXmlSerializer(GetType(T))
            Using sr = New StringReader(xmlString)
                Using tr = New XmlTextReader(sr)
                    returnObject = CType(serializer.Deserialize(tr), T)
                End Using
            End Using
        Catch ex As Exception
            OnLogEvent(ex.ToString())
            LastError = ex.Message
        Finally
            DisposeXmlSerializer(serializer)
        End Try

        Return returnObject
    End Function

    Public Function ReadXmlFromStream(Of T)(ByVal stream As Stream) As T
        Dim returnObject As T = Nothing
        Dim serializer As XmlSerializer = Nothing
        Try
            serializer = NewXmlSerializer(GetType(T))
            returnObject = CType(serializer.Deserialize(stream), T)
        Catch ex As Exception
            LastError = ex.Message
            OnLogEvent($"{ConfigurationFileName}:{ex.Message}")
        Finally
            DisposeXmlSerializer(serializer)
        End Try

        Return returnObject
    End Function

    Public Function WriteXmlToString(Of T)(ByVal objectToWrite As T, <Out> ByRef returnString As String) As Boolean
        Using memStream = New MemoryStream()
            If WriteXmlToStream(objectToWrite, memStream) Then
                Using streamReader = New StreamReader(memStream)
                    returnString = streamReader.ReadToEnd()
                    Return True
                End Using
            End If

            returnString = Nothing
            Return False
        End Using
    End Function

    Public Function WriteXmlToStream(Of T)(ByVal objectToWrite As T, ByVal stream As Stream) As Boolean
        Dim xmlSettings As XmlWriterSettings = New XmlWriterSettings _
                With {.Indent = True, .OmitXmlDeclaration = True, .Encoding = Encoding.UTF8}
        Dim serializer As XmlSerializer = Nothing
        Try
            serializer = NewXmlSerializer(GetType(T))
            Using writer As XmlWriter = XmlWriter.Create(stream, xmlSettings)
                serializer.Serialize(writer, objectToWrite)
            End Using

            stream.Position = 0
            Return True
        Catch ex As Exception
            LastError = ex.Message
            OnLogEvent(ex.ToString())
            Return False
        Finally
            DisposeXmlSerializer(serializer)
        End Try
    End Function

    Public Function ReadXmlFile(Of T)(ByVal configurationFile As String) As T
        ConfigurationFileName = configurationFile
        Return ReadXmlFile(Of T)()
    End Function

    Public Function WriteXmlFile(Of T)(ByVal objectToWrite As T, ByVal configurationFile As String) As Boolean
        ConfigurationFileName = configurationFile
        Return WriteXmlFile(objectToWrite)
    End Function

    Public Function ReadXmlFile(Of T)() As T
        Dim returnObject As T = Nothing
        Dim serializer As XmlSerializer = Nothing
        Try
            serializer = NewXmlSerializer(GetType(T))
            Using fs As XmlReader = XmlReader.Create(ConfigurationFileName)
                returnObject = CType(serializer.Deserialize(fs), T)
            End Using
        Catch ex As Exception
            LastError = ex.Message
            OnLogEvent($"{ConfigurationFileName}:{ex.Message}")
        Finally
            DisposeXmlSerializer(serializer)
        End Try

        Return returnObject
    End Function

    Public Function WriteXmlFile(Of T)(ByVal objectToWrite As T) As Boolean
        Dim xmlSettings As XmlWriterSettings = New XmlWriterSettings _
                With {.Indent = True, .OmitXmlDeclaration = False, .Encoding = Encoding.UTF8}
        Dim serializer As XmlSerializer = Nothing
        Try
            serializer = NewXmlSerializer(GetType(T))
            Using writer As XmlWriter = XmlWriter.Create(ConfigurationFileName, xmlSettings)
                serializer.Serialize(writer, objectToWrite)
            End Using

            Return True
        Catch ex As Exception
            LastError = ex.Message
            OnLogEvent($"{ConfigurationFileName}:{ex.Message}")
            Return False
        Finally
            DisposeXmlSerializer(serializer)
        End Try
    End Function

    Public Function GetLogText() As String
        _logStringBuilder.AppendLine(LastError)
        Return _logStringBuilder.ToString()
    End Function

    Public Property LastError As String

    Public Property ConfigurationFileName As String

    Protected Overridable Sub OnLogEvent(ByVal message As String)
        RaiseEvent LogEvent(Me, New LogEventArgs(message))
    End Sub
End Class