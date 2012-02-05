Public Class MainForm
    '   /*
    ' * ODE stuff
    ' */
    Protected _world As New Ode.World()
    Protected _space As Ode.Space = New Ode.HashSpace()
    Protected _contactGroup As New Ode.JointGroup()
    Private collideCallback As Ode.CollideHandler
    Protected _floor As Ode.Geom
    Protected _walls As Ode.PlaneGeom()
    Private Const HALF_WIDTH As Integer = 256
    Private Const HALF_LENGTH As Integer = 256
    Protected _useStepFast As Boolean = False
    Private Const MAX_STEP_SIZE As Single = 0.1F
    Private lastframe As DateTime = DateTime.MinValue
    Private leftOverTime As Single = 0
    Protected pauseSim As Boolean
    Private _prevpos As New Ode.Vector3
    Private _bodyIndex As Integer = 0
    Private _body(499) As Ode.Body
    Private _geom As Ode.Geom


    '/*
    ' * End
    ' */


    '  Private media As New MCI.Media()
    Private WithEvents Clock As Timer

    Enum State
        demo1
        demo2
        demo3
        demo4
        demo5
        demo6
    End Enum


    Private _state As State = State.demo1



    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        warp3D1.DisplayDefaultScene()
        '//media.PlayWavResource( "tunes.Bass_Is_Extra_90bpm.wav" );

        Clock = New Timer()
        Clock.Interval = 50

        Clock.Start()
    End Sub

    Public Sub Timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Clock.Tick
        If sender Is Clock Then
            Select Case _state
                Case State.demo1
                    warp3D1.RotateObject("LensFlare1", -0.05F, -0.15F, -0.05F)
                    warp3D1.RotateScene(0.05F, 0.05F, 0.05F)


                Case State.demo6
                    UpdateWorld()

                    For q As Integer = 0 To _body.Length - 1
                        If _body(q) IsNot Nothing Then
                            Dim pos As Ode.Vector3 = _body(q).Position

                            warp3D1.RotateSelf("cube" & q, OdeWarpHelpers.Ode2WarpQuaternion(_body(q).Quaternion))
                            warp3D1.SetPos("cube" & q, pos.X, pos.Y, pos.Z)
                        End If
                    Next


                Case State.demo4
                    warp3D1.RotateModelSelf("satellite", -0.15F, -0.15F, -0.15F)
                    warp3D1.RotateModel("satellite", -0.02F, -0.02F, -0.02F)

                Case State.demo3
                Case State.demo2
                    warp3D1.RotateScene(0.05F, 0.05F, 0.05F)
            End Select

            warp3D1.Render()
            warp3D1.Refresh()
        End If
    End Sub

    Private Sub Load_Scene_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Load_Scene.Click
        _state = State.demo1

        warp3D1.CreateScene(512, 512)
        warp3D1.SetBackgroundColor(&HECE9D8)

        Dim objects As Collections.Hashtable = warp3D1.Import3Ds("chair", "DesignerChair3.3ds", True)

        warp3D1.AddLight("Light1", -1.0F, -1.0F, -1.0F, &HFFFFFF, 320, 200)
        warp3D1.AddLight("Light2", 0.2F, 0.2F, 1.0F, &HFFFFFF, 320, 200)

        warp3D1.AddMaterial("seat")
        warp3D1.SetTexture("seat", "glass.jpg")
        warp3D1.SetReflectivity("seat", 80)
        warp3D1.SetTransparency("seat", 180)
        warp3D1.SetWireframe("seat", True)

        warp3D1.AddMaterial("wood")
        warp3D1.SetTexture("wood", "cream.jpg")
        warp3D1.SetReflectivity("wood", 100)

        warp3D1.AddMaterial("strut")
        warp3D1.SetTexture("strut", "chrome.jpg")
        warp3D1.SetReflectivity("strut", 255)
        warp3D1.SetEnvMap("strut", "skymap.jpg")

        warp3D1.SetObjectMaterial("chair_Object", "seat")
        warp3D1.SetObjectMaterial("chair_Object", "seat")
        warp3D1.SetObjectMaterial("chair_Object01", "wood")
        warp3D1.SetObjectMaterial("chair_Object02", "wood")
        warp3D1.SetObjectMaterial("chair_Object03", "strut")
        warp3D1.SetObjectMaterial("chair_Object04", "strut")
        warp3D1.SetObjectMaterial("chair_Object05", "strut")
        warp3D1.SetObjectMaterial("chair_Object06", "strut")
        warp3D1.SetObjectMaterial("chair_Object07", "strut")
        warp3D1.SetObjectMaterial("chair_Object08", "strut")
        warp3D1.SetObjectMaterial("chair_Object09", "strut")

        warp3D1.NormaliseScene()

        warp3D1.Render()
        warp3D1.Refresh()
    End Sub

    Private Sub button_Flare1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Flare1.Click
        _state = State.demo2

        warp3D1.CreateScene(512, 512)

        warp3D1.SetBackgroundMaterial("stardust.material")
        warp3D1.AddMaterial("metal")
        warp3D1.SetEnvMap("metal", "chrome.jpg")
        warp3D1.SetReflectivity("metal", 255)

        warp3D1.AddLight("Light1", 0.2F, 0.2F, 1.0F, &HFFFFFF, 320, 80)

        Dim objects As Collections.Hashtable = warp3D1.Import3Ds("wobble", "wobble.3ds", True)
        For Each myDE As DictionaryEntry In objects
            Dim name As String = myDE.Key.ToString
            warp3D1.SetObjectMaterial(name, "metal")
        Next
        warp3D1.AddLensFlare("LensFlare1")
        warp3D1.NormaliseScene()

        warp3D1.ScaleObject("LensFlare1", 60.0F)
        warp3D1.ScaleScene(0.5F, 0.5F, 0.5F)

        warp3D1.Render()
        warp3D1.Refresh()
    End Sub

    Private Sub button_Stones_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Stones.Click
        _state = State.demo3

        warp3D1.CreateScene(512, 512)
        warp3D1.SetBackgroundColor(&HECE9D8)

        warp3D1.AddMaterial("Stone1")
        warp3D1.SetTexture("Stone1", "stone1.jpg")

        warp3D1.AddMaterial("Stone2")
        warp3D1.SetTexture("Stone2", "stone2.jpg")

        warp3D1.AddMaterial("Stone3")
        warp3D1.SetTexture("Stone3", "stone3.jpg")

        warp3D1.AddMaterial("Stone4")
        warp3D1.SetTexture("Stone4", "stone4.jpg")

        warp3D1.AddLight("Light1", 0.2F, 0.2F, 1.0F, &HFFFFFF, 144, 120)
        warp3D1.AddLight("Light2", -1.0F, -1.0F, 1.0F, &H332211, 100, 40)
        warp3D1.AddLight("Light3", -1.0F, -1.0F, 1.0F, &H666666, 200, 120)

        Dim objects As Collections.Hashtable = warp3D1.Import3Ds("wobble", "wobble.3ds", True)
        For Each myDE As DictionaryEntry In objects
            Dim name As String = myDE.Key.ToString
            warp3D1.ProjectFrontal(name)
        Next

        warp3D1.SetObjectMaterial("wobble_Sphere1", "Stone1")
        warp3D1.SetObjectMaterial("wobble_Wobble1", "Stone2")
        warp3D1.SetObjectMaterial("wobble_Wobble2", "Stone3")
        warp3D1.SetObjectMaterial("wobble_Wobble3", "Stone4")

        warp3D1.NormaliseScene()
        warp3D1.ShiftObject("Wobble3", 55.0F, 1.0F, 1.0F)
        warp3D1.ScaleScene(0.5F, 0.5F, 0.5F)

        warp3D1.Render()
        warp3D1.Refresh()
    End Sub

    Private Sub button_Bounce_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Bounce.Click
        _state = State.demo4

        warp3D1.CreateScene(512, 512)
        warp3D1.SetBackgroundMaterial("stardust.material")

        warp3D1.AddMaterial("earth")
        warp3D1.SetTexture("earth", "land_shallow_topo_1024.jpg")
        warp3D1.SetReflectivity("earth", 200)

        warp3D1.AddMaterial("panel")
        warp3D1.SetTexture("panel", "icesolar.JPG")
        warp3D1.SetReflectivity("panel", 225)

        warp3D1.AddLight("Light1", 0.2F, 0.2F, 1.0F, &HFFFFFF, 144, 120)
        warp3D1.AddLight("Light2", -1.0F, -1.0F, 1.0F, &H332211, 100, 40)
        warp3D1.AddLight("Light3", -1.0F, -1.0F, 1.0F, &H666666, 200, 120)

        Dim objects As Collections.Hashtable = warp3D1.Import3Ds("satellite", "voyager.3ds", True)
        For Each myDE As DictionaryEntry In objects
            Dim name As String = myDE.Key.ToString
            warp3D1.SetObjectMaterial(name, "panel")
        Next

        warp3D1.ScaleModel("satellite", 0.002F)
        warp3D1.TranslateModel("satellite", 0.66F, 0.66F, 0.0F)

        warp3D1.AddSphere("Sphere1", 0.65F, 32)
        warp3D1.SetObjectMaterial("Sphere1", "earth")

        warp3D1.Render()
        warp3D1.Refresh()
        Clock.Start()
    End Sub

    Private Sub button_Studio_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Studio.Click
        _state = State.demo5

        warp3D1.CreateScene(512, 512)
        warp3D1.SetBackgroundColor(&HECE9D8)

        warp3D1.AddMaterial("panel")
        warp3D1.SetTexture("panel", "voyager.jpg")
        warp3D1.SetReflectivity("panel", 225)

        warp3D1.AddMaterial("default")
        warp3D1.SetReflectivity("default", 225)
        warp3D1.SetTexture("default", "chrome.jpg")

        warp3D1.AddLight("Light1", 0.0F, 0.0F, 1.0F, &HFFFFFF, 320, 120)

        Dim objects As Collections.Hashtable = warp3D1.Import3Ds("voyager", "voyager.3ds", True)
        For Each myDE As DictionaryEntry In objects
            Dim name As String = myDE.Key.ToString
            warp3D1.SetObjectMaterial(name, "default")
        Next

        warp3D1.SetObjectMaterial("voyager_Plaque", "panel")
        warp3D1.NormaliseScene()

    End Sub

    Private Sub button_Physics_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Physics.Click

        _state = State.demo6

        warp3D1.CreateScene(512, 512)
        warp3D1.SetBackgroundColor(&HECE9D8)

        warp3D1.AddMaterial("default", &HDD00)
        warp3D1.SetReflectivity("default", 255)

        warp3D1.AddMaterial("yellow", &HFFFF00)
        warp3D1.SetReflectivity("yellow", 200)

        warp3D1.AddMaterial("floor", &HCCCCCC)
        warp3D1.AddPlane("plane1", 10.0F)
        warp3D1.SetObjectMaterial("plane1", "floor")


        warp3D1.AddLight("Light1", 0.2F, 0.2F, 1.0F, &HFFFFFF, 144, 120)
        warp3D1.AddLight("Light2", -1.0F, 0.0F, 0.0F, &H332211, 100, 40)
        warp3D1.AddLight("Light3", 1.0F, 0.0F, 0.0F, &H666666, 200, 120)
        warp3D1.AddCube("cube0", 2.0F)
        warp3D1.SetObjectMaterial("cube0", "default")

        _world.SetGravity(0, -9.81F, 0)
        _world.CFM = 0.000001F
        _floor = New Ode.PlaneGeom(_space, 0, 1, 0, 0)
        collideCallback = New Ode.CollideHandler(AddressOf Me.CollideCallbackHandler)

        ReDim _walls(3)
        _walls(0) = New Ode.PlaneGeom(_space, 1, 0, 0, -10)
        _walls(1) = New Ode.PlaneGeom(_space, -1, 0, 0, -10)
        _walls(2) = New Ode.PlaneGeom(_space, 0, 0, 1, -10)
        _walls(3) = New Ode.PlaneGeom(_space, 0, 0, -1, -10)

        InitialiseWorld()

        warp3D1.ShiftDefaultCamera(0.0F, 0.0F, -20.0F)

        button_Body.Enabled = True
        button_Sphere.Enabled = True
    End Sub

    Protected Sub CollideCallbackHandler(ByVal o1 As Ode.Geom, ByVal o2 As Ode.Geom)
        Dim cgeoms() As Ode.ContactGeom = o1.Collide(o2, 30)
        If cgeoms.Length > 0 Then
            Dim contacts() As Ode.Contact = Ode.Contact.FromContactGeomArray(cgeoms)

            For i As Integer = 0 To contacts.Length - 1
                contacts(i).Surface.mode = Ode.SurfaceMode.Bounce
                contacts(i).Surface.mu = 100  '//World.Infinity
                contacts(i).Surface.bounce = 0.7F
                contacts(i).Surface.bounce_vel = 0.05F
                Dim cj As Ode.ContactJoint = New Ode.ContactJoint(_world, _contactGroup, contacts(i))
                cj.Attach(contacts(i).Geom.Geom1.Body, contacts(i).Geom.Geom2.Body)
            Next
        End If
    End Sub

    Private Sub UpdateWorld()
        If lastframe = DateTime.MinValue Or pauseSim Then
            lastframe = DateTime.Now
            Return
        End If

        Dim thisframe As DateTime = DateTime.Now
        Dim elapsed As TimeSpan = thisframe - lastframe

        Dim t As Single = CSng(elapsed.TotalSeconds + leftOverTime)
        Dim st As Single

        If t = 0 Then Return

        While (t > 0.02)
            If t > MAX_STEP_SIZE Then
                st = MAX_STEP_SIZE
                t -= MAX_STEP_SIZE
            Else
                st = t
                t = 0
            End If

            _contactGroup.Empty()
            UpdateWorldForces(st)

            _space.Collide(collideCallback)

            If _useStepFast Then
                _world.StepFast1(st, 5)
            Else
                _world.Step(st)
            End If
        End While
        leftOverTime = t

        lastframe = thisframe
    End Sub

    Protected Sub InitialiseWorld()
        Dim mass As New Ode.Mass
        mass.SetBox(500.0F, 2.0F, 2.0F, 2.0F)

        _body(_bodyIndex) = New Ode.Body(_world)
        _body(_bodyIndex).Position = New Ode.Vector3(0, 8, 0)
        _body(_bodyIndex).SetMass(mass)
        _body(_bodyIndex).Geoms.Add(New Ode.BoxGeom(_space, 2.0F, 2.0F, 2.0F))
    End Sub

    Protected Sub RenderWorld()
    End Sub

    Protected Sub UpdateWorldForces(ByVal stepSize As Single)
        '// eg _body[n].AddForce( new Ode.Vector3( 0, 0, 1.0f ) );
    End Sub

    Private Sub button_Body_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Body.Click
        If _state <> State.demo6 Then Return


        _bodyIndex += 1

        warp3D1.AddCube("cube" & _bodyIndex, 2.0F)
        warp3D1.SetObjectMaterial("cube" & _bodyIndex, "default")


        _body(_bodyIndex) = New Ode.Body(_world)
        _body(_bodyIndex).Position = New Ode.Vector3(0, 8, 0)
        _body(_bodyIndex).Geoms.Add(New Ode.BoxGeom(_space, 2.0F, 2.0F, 2.0F))

    End Sub

    Private Sub button_Sphere_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_Sphere.Click
        If _state <> State.demo6 Then Return

        _bodyIndex += 1

        warp3D1.AddSphere("cube" & _bodyIndex, 1.0F, 24)
        warp3D1.SetObjectMaterial("cube" & _bodyIndex, "yellow")

        _body(_bodyIndex) = New Ode.Body(_world)
        _body(_bodyIndex).Position = New Ode.Vector3(0, 8, 0)
        _body(_bodyIndex).Geoms.Add(New Ode.SphereGeom(_space, 1.0F))
    End Sub

    'Private Sub button_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs)
    'End Sub

End Class
