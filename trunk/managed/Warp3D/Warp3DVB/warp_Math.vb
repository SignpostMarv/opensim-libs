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

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Math.
    '/ </summary>
    Public Class warp_Math
        Private Shared sinus() As Single
        Private Shared cosinus() As Single
        Private Shared trig As Boolean = False
        Public Shared pi As Single = 3.14159274F
        Private Shared rad2scale As Single = 4096.0F / 3.14159274F / 2.0F
        Private Shared pad As Single = 256 * 3.14159274F

        Private Shared fastRandoms() As Integer
        Private Shared fastRndPointer As Integer = 0
        Private Shared fastRndInit As Boolean = False

        Private Shared _rnd As Random = New Random()


        Public Sub New()
            '
            ' TODO: Add constructor logic here
            '
        End Sub


        Public Shared Function interpolate(ByVal a As Single, ByVal b As Single, ByVal d As Single) As Single
            Dim f As Single = (1 - cos(d * pi)) * 0.5F
            Return a + f * (b - a)
        End Function

        Public Shared Function random() As Single
            Dim rnd As Random = New Random()
            Return CType((rnd.NextDouble() * 2 - 1), Single)
        End Function

        Public Shared Function random(ByVal min As Single, ByVal max As Single) As Single
            'Random rnd = new Random();
            Return CType((_rnd.NextDouble() * (max - min) + min), Single)
        End Function

        Public Shared Function randomWithDelta(ByVal averidge As Single, ByVal delta As Single) As Single
            Return averidge + random() * delta
        End Function


        Public Shared Function fastRnd(ByVal bits As Integer) As Integer
            If bits < 1 Then Return 0

            fastRndPointer = (fastRndPointer + 1) And 31
            If Not fastRndInit Then
                fastRandoms = New Integer(31) {}
                Dim i As Integer
                For i = 0 To 30 ' - 1 Step i + 1
                    fastRandoms(i) = CInt(random(0, &HFFFFFF))
                Next
                fastRndInit = True
            End If
            Return fastRandoms(fastRndPointer) And (1 << (bits - 1))
        End Function


        Public Shared Function fastRndBit() As Integer
            Return fastRnd(1)
        End Function

        Public Shared Function deg2rad(ByVal deg As Single) As Single
            Return deg * 0.0174532924F
        End Function

        Public Shared Function rad2deg(ByVal rad As Single) As Single
            Return rad * 57.29578F
        End Function

        Public Shared Function sin(ByVal angle As Single) As Single
            If Not trig Then buildTrig()
            Return sinus(CInt((angle + pad) * rad2scale) And &HFFF)
        End Function

        Public Shared Function cos(ByVal angle As Single) As Single
            If Not trig Then buildTrig()
            Return cosinus(CInt((angle + pad) * rad2scale) And &HFFF)
        End Function


        Private Shared Sub buildTrig()
            '  System.Console.WriteLine(">> Building warp_Math LUT")

            sinus = New Single(4095) {}
            cosinus = New Single(4095) {}

            For i As Integer = 0 To 4095 '6 - 1 Step i + 1
                sinus(i) = CSng(Math.Sin(i / rad2scale))
                cosinus(i) = CSng(Math.Cos(i / rad2scale))
            Next

            trig = True
        End Sub


        Public Shared Function pythagoras(ByVal a As Single, ByVal b As Single) As Single
            Return CSng(Math.Sqrt(a * a + b * b))
        End Function

        Public Shared Function pythagoras(ByVal a As Integer, ByVal b As Integer) As Integer
            Return CInt(Math.Sqrt(a * a + b * b))
        End Function

        Public Shared Function crop(ByVal num As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
            If num < min Then Return min
            If num > max Then Return max
            Return num
        End Function

        Public Shared Function crop(ByVal num As Single, ByVal min As Single, ByVal max As Single) As Single
            If num < min Then Return min
            If num > max Then Return max
            Return num
        End Function

        Public Shared Function inrange(ByVal num As Integer, ByVal min As Integer, ByVal max As Integer) As Boolean
            Return ((num >= min) AndAlso (num < max))
        End Function

        Public Shared Sub clearBuffer(ByRef buffer() As Integer, ByVal c As Integer)
            System.Array.Clear(buffer, 0, buffer.GetLength(0))
            ' For i As Integer = 0 To buffer.Length - 1
            '     buffer(i) = c
            ' Next
        End Sub

        Public Shared Sub clearBuffer(ByRef buffer() As Long, ByVal c As Integer)
            System.Array.Clear(buffer, 0, buffer.GetLength(0))
            ' For i As Integer = 0 To buffer.Length - 1
            '     buffer(i) = c
            ' Next
        End Sub

        Public Shared Sub clearBuffer(ByVal buffer() As Byte, ByVal value As Byte)
            System.Array.Clear(buffer, 0, buffer.GetLength(0))
        End Sub

        Public Shared Sub cropBuffer(ByVal buffer() As Integer, ByVal min As Integer, ByVal max As Integer)
            For i As Integer = buffer.GetLength(0) - 1 To 0 Step -1
                buffer(i) = crop(buffer(i), min, max)
            Next
        End Sub

        Public Shared Sub copyBuffer(ByVal source() As Integer, ByVal target() As Integer)
            System.Array.Copy(source, 0, target, 0, crop(source.GetLength(0), 0, target.GetLength(0)))
        End Sub
        ' 		
        ' 		Public static single random()
        ' 		{
        ' 			Return CType((Math.r.random()*2-1), single)
        ' 		}
        '  
        ' 		Public static single random(single min, single max)
        ' 		{
        ' 			Return CType((Math.random()*(max-min)+min), single)
        ' 		}
        '  
        ' 		Public static single randomWithDelta(single averidge, single delta)
        ' 		{
        ' 			Return averidge+random()*delta
        ' 		}
        ' 		*/
    End Class
End Namespace