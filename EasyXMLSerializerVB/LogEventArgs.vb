
Public Class LogEventArgs
    Inherits EventArgs

    Public Sub New(message As String)
        Me.Message = message
    End Sub

    Public Property Message As String

End Class
