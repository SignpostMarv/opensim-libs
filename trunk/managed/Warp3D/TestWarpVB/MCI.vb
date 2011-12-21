'using System;
'using System.Runtime.InteropServices;
'using System.Text;
'using System.IO;
Imports System.Text

Namespace MCI

    Public Delegate Sub PositionEventHandler(ByVal sender As Object, ByVal e As PositionChangedEventArgs)

    Public Class Media
        Private timer1 As System.Windows.Forms.Timer
        Public FileIsOpen As Boolean = False
        Public PositionChanged As PositionEventHandler

        Private Declare Auto Function PlaySound Lib "winmm.dll" (ByVal data() As Byte, ByVal hMod As IntPtr, ByVal dwFlags As Integer) As Long
        Private Declare Auto Function mciSendString Lib "winmm.dll" (ByVal strCommand As String, ByRef strReturn As String, ByRef iReturnLength As Integer, ByVal hwndCallback As IntPtr) As Long
        ' Private Declare Auto Function mciSendString Lib "winmm.dll" (ByVal strCommand As String, ByVal strReturn As StringBuilder, ByVal iReturnLength As Integer, ByVal hwndCallback As IntPtr) As Long

        '<DllImport("winmm.dll")> _
        ' Private Shared Function mciSendString(ByVal strCommand As String, ByVal strReturn As StringBuilder, ByVal iReturnLength As Integer, ByVal hwndCallback As IntPtr) As Long

        '   <DllImport("Winmm.dll")> _
        '  Private Shared Function PlaySound(ByVal data() As Byte, ByVal hMod As IntPtr, ByVal dwFlags As UInt32) As Long


        Private sCommand As String
        Private sBuffer As String 'New StringBuilder(128)
        Private m_Repeat As Boolean = False
        Private m_Pause As Boolean = False
        Private seconds As Integer

        Public Sub New()
            timer1 = New System.Windows.Forms.Timer()
            timer1.Enabled = False
            timer1.Interval = 100
            AddHandler timer1.Tick, AddressOf Me.timer1_Tick
        End Sub

        Private Sub timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim pArgs As New PositionChangedEventArgs(Me.Position)

            If PositionChanged IsNot Nothing Then PositionChanged(Me, pArgs)
        End Sub

        Public Sub Close()
            sCommand = "close MediaFile"
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)

            timer1.Enabled = False
            FileIsOpen = False
        End Sub

        Public Sub [Stop]()
            Me.Position = 0
            sCommand = "stop MediaFile"
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)

            timer1.Enabled = False
        End Sub

        Public Property Pause() As Boolean
            Get
                Return m_Pause
            End Get
            Set(ByVal value As Boolean)
                m_Pause = value

                If m_Pause Then
                    sCommand = "pause MediaFile"
                Else
                    sCommand = "resume MediaFile"
                End If

                mciSendString(sCommand, Nothing, 0, IntPtr.Zero)
            End Set
        End Property

        Public Sub Open(ByVal sFileName As String)
            If Me.Status() = "playing" Then Me.Close()

            sCommand = "open """ + sFileName + """ type mpegvideo alias MediaFile"
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)

            FileIsOpen = True
        End Sub

        Public Sub Open(ByVal sFileName As String, ByVal videobox As System.Windows.Forms.PictureBox)
            If Me.Status = "playing" Then Me.Close()

            sCommand = "open """ & sFileName & """ type mpegvideo alias MediaFile style child parent " & videobox.Handle.ToInt32()
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)
            sCommand = "put MediaFile window at 0 0 " & videobox.Width & " " & videobox.Height
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)

            FileIsOpen = True
        End Sub

        Public Sub Play()

            If FileIsOpen Then
                sCommand = "play MediaFile"

                If Repeat Then sCommand += " REPEAT"

                mciSendString(sCommand, Nothing, 0, IntPtr.Zero)

                timer1.Enabled = True
            End If
        End Sub

        Public Sub FullScreen()
            sCommand = "play MediaFile FullScreen"
            mciSendString(sCommand, Nothing, 0, IntPtr.Zero)
        End Sub

        Public Property Position() As Integer
            Get
                sCommand = "status MediaFile position"
                mciSendString(sCommand, sBuffer, Nothing, IntPtr.Zero)
                '  mciSendString(sCommand, sBuffer, sBuffer.Capacity, IntPtr.Zero)

                seconds = Integer.Parse(sBuffer.ToString())
                seconds = seconds \ 1000
                Return seconds
            End Get
            Set(ByVal value As Integer)
                seconds = value
                seconds = seconds * 1000
                sCommand = "play MediaFile from " & seconds.ToString()
                mciSendString(sCommand, Nothing, 0, IntPtr.Zero)
            End Set
        End Property

        Public Function Duration() As Integer
            Dim ReturnSeconds As Integer
            sCommand = "status MediaFile length"
            sBuffer = Space(128)
            mciSendString(sCommand, sBuffer, 128, IntPtr.Zero)

            ReturnSeconds = Integer.Parse(sBuffer.ToString())
            ReturnSeconds = ReturnSeconds \ 1000
            Return ReturnSeconds
        End Function

        Public Function Status() As String
            mciSendString("status MediaFile mode", sBuffer, Nothing, IntPtr.Zero)
            '  mciSendString(sCommand, sBuffer, sBuffer.Capacity, IntPtr.Zero)
            Return sBuffer.ToString()
        End Function

        Public Property Volume() As Integer
            Get
                If Me.Status() <> String.Empty Then
                    mciSendString("status MediaFile volume", sBuffer, Nothing, IntPtr.Zero)
                    '  mciSendString(sCommand, sBuffer, sBuffer.Capacity, IntPtr.Zero)
                    Return Integer.Parse(sBuffer.ToString())
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                If value <= 1000 Then
                    mciSendString("setaudio MediaFile volume to " & value.ToString(), Nothing, 0, IntPtr.Zero)
                Else
                    System.Windows.Forms.MessageBox.Show("Volume value must be smaller than 1000")
                End If
            End Set
        End Property

        Public Property Repeat() As Boolean
            Get
                Return m_Repeat
            End Get
            Set(ByVal value As Boolean)
                m_Repeat = value
            End Set
        End Property

        Public Function SecondsToTime(ByVal seconds As Integer) As String
            Dim hours, minutes As Integer

            hours = seconds \ 3600
            minutes = seconds \ 60
            minutes = minutes Mod 60
            seconds = seconds Mod 60

            Return hours.ToString("00") & ":" & minutes.ToString("00") & ":" & seconds.ToString("00")
        End Function

        Public Sub PlayWavResource(ByVal wav As String)
            '// get the namespace 
            Dim strNameSpace As String = System.Reflection.Assembly.GetExecutingAssembly().GetName.Name

            '// get the resource into a stream
            Dim Str As IO.Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(strNameSpace & "." & wav)

            If Str Is Nothing Then Return

            '// bring stream into a byte array
            Dim bStr(CInt(Str.Length - 1)) As Byte
            Str.Read(bStr, 0, CInt(Str.Length))

            '// play the resource
            PlaySound(bStr, IntPtr.Zero, 1 Or 4 Or 8)
        End Sub
    End Class

    Public Class PositionChangedEventArgs
        Inherits EventArgs

        Private _position As Integer

        Public Sub New(ByVal num As Integer)
            Me._position = num
        End Sub

        Public ReadOnly Property newPosition() As Integer
            Get
                Return _position
            End Get
        End Property
    End Class
End Namespace
