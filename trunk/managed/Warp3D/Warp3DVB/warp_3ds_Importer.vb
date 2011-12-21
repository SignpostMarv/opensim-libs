' Created by: X
' http://www.createdbyx.com/
' ----------------------
' C# to VB.NET conversion of this code was done by hand and with the assistance of the 
' Convert C# to VB .NET application availible at http://www.kamalpatel.net/ConvertCSharp2VB.aspx
' ConvertCSharp2VB utility was developed by: Kamal Patel (http://www.KamalPatel.net) 
' ----------------------
' Thanks to Alan Simes ( http://alansimes.blogdns.net/ ) for creating the Warp3D library.
' http://alansimes.blogdns.net/cs/files/default.aspx
' ----------------------

Imports System
Imports System.IO
Imports System.Collections
Imports System.Net

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_3ds_Importer.
    '/ </summary>
    Public Class warp_3ds_Importer
        Private currentJunkId As Integer
        Private nextJunkOffset As Integer

        Private currentObjectName As String = Nothing
        Private currentObject As warp_Object = Nothing
        Private endOfStream As Boolean = False

        Private _objects As Hashtable = New Hashtable()

        Public Sub New()
        End Sub

        Public Function importFromFile(ByVal name As String, ByVal path As String) As Hashtable
            Dim fs As Stream = Nothing
            _objects.Clear()

            If path.StartsWith("http") Then
                Dim webrq As WebRequest = WebRequest.Create(path)
                fs = webrq.GetResponse().GetResponseStream()
            Else
                fs = New FileStream(path, FileMode.Open)
            End If

            Dim br As BinaryReader = New BinaryReader(fs)
            Return importFromStream(name, br)
        End Function

        Public Function importFromStream(ByVal name As String, ByVal inStream As BinaryReader) As Hashtable
            _objects.Clear()

            readJunkHeader(inStream)
            If currentJunkId <> &H4D4D Then Return Nothing

            Try
                While True
                    readNextJunk(name, inStream)
                End While
            Catch
                ' ignored
            End Try

            inStream.Close()

            Return _objects
        End Function

        Private Sub readJunkHeader(ByVal inStream As BinaryReader)
            currentJunkId = readShort(inStream)
            nextJunkOffset = readInt(inStream)
            endOfStream = currentJunkId < 0
        End Sub


        Private Function readInt(ByVal inStream As BinaryReader) As Integer
            Return inStream.ReadByte() Or (inStream.ReadByte() << 8) Or (inStream.ReadByte() << 16) Or (inStream.ReadByte() << 24)
        End Function

        Private Function readShort(ByVal inStream As BinaryReader) As Integer
            Return (inStream.ReadByte() Or (inStream.ReadByte() << 8))
        End Function

        Private Sub readNextJunk(ByVal name As String, ByVal inStream As BinaryReader)
            readJunkHeader(inStream)

            If currentJunkId = &H3D3D Then Return

            If currentJunkId = &H4000 Then
                currentObjectName = readString(inStream)
                Return
            End If

            If currentJunkId = &H4100 Then
                currentObject = New warp_Object()
                _objects.Add(name & "_" & currentObjectName, currentObject)

                Return
            End If

            If currentJunkId = &H4110 Then
                readVertexList(inStream)
                Return
            End If

            If currentJunkId = &H4120 Then
                readPointList(inStream)
                Return
            End If

            If currentJunkId = &H4140 Then
                readMappingCoordinates(inStream)
                Return
            End If

            skipJunk(inStream)
        End Sub

        Private Function readString(ByVal inStream As BinaryReader) As String
            Dim result As String = ""
            Dim inByte As Byte

            inByte = inStream.ReadByte()
            While inByte <> 0
                result &= inByte
                inByte = inStream.ReadByte()
            End While

            Return result
        End Function

        Private Function readFloat(ByVal inStream As BinaryReader) As Single
            Dim bits As Integer = readInt(inStream)

            Dim s As Integer = CInt(IIf((bits >> 31) = 0, 1, -1))
            Dim e As Integer = ((bits >> 23) And &HFF)
            Dim m As Integer = CInt(IIf((e = 0), (bits And &H7FFFFF) << 1, (bits And &H7FFFFF) Or &H800000))

            Dim v As Double = s * m * Math.Pow(2, e - 150)

            Return CSng(v)
        End Function


        Private Sub skipJunk(ByVal inStream As BinaryReader)
            Try

                For i As Integer = 0 To nextJunkOffset - 6 'AndAlso (Not endOfStream) 'Step  i + 1
                    If Not endOfStream Then endOfStream = (inStream.ReadByte() < 0)
                Next
            Catch
                endOfStream = True
            End Try

        End Sub

        Private Sub readVertexList(ByVal inStream As BinaryReader)
            Dim x As Single, y As Single, z As Single
            Dim vertices As Integer = readShort(inStream)

            For i As Integer = 0 To vertices - 1 'Step i + 1
                x = readFloat(inStream)
                y = readFloat(inStream)
                z = readFloat(inStream)

                currentObject.addVertex(x, -y, z)
            Next
        End Sub


        Private Sub readPointList(ByVal inStream As BinaryReader)
            Dim v1 As Integer, v2 As Integer, v3 As Integer
            Dim triangles As Integer = readShort(inStream)

            For i As Integer = 0 To triangles - 1 'Step i + 1
                v1 = readShort(inStream)
                v2 = readShort(inStream)
                v3 = readShort(inStream)

                readShort(inStream)

                currentObject.addTriangle(currentObject.vertex(v1), _
                        currentObject.vertex(v2), _
                        currentObject.vertex(v3))
            Next
        End Sub

        Private Sub readMappingCoordinates(ByVal inStream As BinaryReader)
            Dim vertices As Integer = readShort(inStream)

            For i As Integer = 0 To vertices - 1 ' Step i + 1
                currentObject.vertex(i).u = readFloat(inStream)
                currentObject.vertex(i).v = readFloat(inStream)
            Next
        End Sub
    End Class
End Namespace