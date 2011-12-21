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
Imports System.Collections

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Object.
    '/ </summary>
    Public Class warp_Object
        Inherits warp_CoreObject

        Public userData As Object = Nothing  ' Can be freely used
        Public user As String = Nothing  ' Can be freely used

        Public vertexData As ArrayList = New ArrayList()
        Public triangleData As ArrayList = New ArrayList()


        Public id As Integer  ' This object's index() As id
        Public name As String = ""  ' This object's name
        Public visible As Boolean = True  ' Visibility tag
        Public projected As Boolean = False
        Public parent As warp_Scene = Nothing
        Private dirty As Boolean = True  ' Flag for dirty handling

        Public fastvertex() As warp_Vertex
        Public fasttriangle() As warp_Triangle

        Public vertices As Integer = 0
        Public triangles As Integer = 0

        Public material As warp_Material = Nothing
        Public index As Integer
        Public offset As Integer = 0
        Public split As Integer = 0


        Public Sub New()

        End Sub


        Public Function vertex(ByVal id As Integer) As warp_Vertex
            Return CType(vertexData(id), warp_Vertex)
        End Function

        Public Function triangle(ByVal id As Integer) As warp_Triangle
            Return CType(triangleData(id), warp_Triangle)
        End Function

        Public Sub addVertex(ByVal NewVertex As warp_Vertex)
            NewVertex.parent = Me
            vertexData.Add(NewVertex)
            dirty = True
        End Sub

        Public Sub addTriangle(ByVal NewTriangle As warp_Triangle)
            NewTriangle.parent = Me
            triangleData.Add(NewTriangle)
            dirty = True
        End Sub

        Public Sub addTriangle(ByVal v1 As Integer, ByVal v2 As Integer, ByVal v3 As Integer)
            addTriangle(vertex(v1), vertex(v2), vertex(v3))
        End Sub

        Public Sub removeVertex(ByVal v As warp_Vertex)
            vertexData.Remove(v)
        End Sub

        Public Sub removeTriangle(ByVal t As warp_Triangle)
            triangleData.Remove(t)
        End Sub

        Public Sub removeVertexAt(ByVal pos As Integer)
            vertexData.Remove(pos)
        End Sub

        Public Sub removeTriangleAt(ByVal pos As Integer)
            triangleData.Remove(pos)
        End Sub

        Public Sub setMaterial(ByVal m As warp_Material)
            material = m
        End Sub


        Public Sub rebuild()
            If Not dirty Then
                Return
            End If
            dirty = False

            ' Generate faster structure for vertices
            vertices = vertexData.Count
            fastvertex = New warp_Vertex(vertices - 1) {}

            Dim enumerator As IEnumerator = vertexData.GetEnumerator()

            For i As Integer = vertices - 1 To 0 Step -1
                enumerator.MoveNext()
                fastvertex(i) = CType(enumerator.Current, warp_Vertex)
            Next

            ' Generate faster structure for triangles
            triangles = triangleData.Count
            fasttriangle = New warp_Triangle(triangles - 1) {}

            enumerator = triangleData.GetEnumerator()

            For i As Integer = triangles - 1 To 0 Step -1
                enumerator.MoveNext()
                fasttriangle(i) = CType(enumerator.Current, warp_Triangle)
                fasttriangle(i).id = i
            Next


            For i As Integer = vertices - 1 To 0 Step -1
                fastvertex(i).id = i
                fastvertex(i).resetNeighbors()
            Next

            Dim tri As warp_Triangle

            For i As Integer = triangles - 1 To 0 Step -1
                tri = fasttriangle(i)
                tri.p1.registerNeighbor(tri)
                tri.p2.registerNeighbor(tri)
                tri.p3.registerNeighbor(tri)
            Next

            regenerate()
        End Sub

        '/*
        'public void rebuild()
        '{
        '	if (!dirty) return;
        '	dirty=false;

        '	Enumeration enum;

        '	// Generate faster structure for vertices
        '	vertices=vertexData.size();
        '	vertex=new warp_Vertex[vertices];
        '	enum=vertexData.elements();
        '	for (int i=vertices-1;i>=0;i--) vertex[i]=(warp_Vertex)enum.nextElement();

        '	// Generate faster structure for triangles
        '	triangles=triangleData.size();
        '	triangle=new warp_Triangle[triangles];
        '	enum=triangleData.elements();
        '	for (int i=triangles-1;i>=0;i--)
        '	{
        '		triangle[i]=(warp_Triangle)enum.nextElement();
        '		triangle[i].id=i;
        '	}

        '	for (int i=vertices-1;i>=0;i--)
        '	{
        '		vertex[i].id=i;
        '		vertex[i].resetNeighbors();
        '	}

        '	warp_Triangle tri;
        '	for (int i=triangles-1;i>=0;i--)
        '	{
        '		tri=triangle[i];
        '		tri.p1.registerNeighbor(tri);
        '		tri.p2.registerNeighbor(tri);
        '		tri.p3.registerNeighbor(tri);
        '	}

        '	regenerate();
        '}
        '*/


        Public Sub addVertex(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            addVertex(New warp_Vertex(x, y, z))
        End Sub


        Public Sub addVertex(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal u As Single, ByVal v As Single)
            Dim vert As warp_Vertex = New warp_Vertex(x, y, z)
            vert.setUV(u, v)
            addVertex(vert)
        End Sub

        Public Sub addTriangle(ByVal a As warp_Vertex, ByVal b As warp_Vertex, ByVal c As warp_Vertex)
            addTriangle(New warp_Triangle(a, b, c))
        End Sub

        Public Sub regenerate()
            Dim i As Integer
            For i = 0 To triangles - 1 'Step i + 1
                fasttriangle(i).regenerateNormal()
            Next
            For i = 0 To vertices - 1 'Step i + 1
                fastvertex(i).regenerateNormal()
            Next
        End Sub

        Public Sub remapUV(ByVal w As Integer, ByVal h As Integer, ByVal sx As Single, ByVal sy As Single)
            rebuild()
            Dim p As Integer = 0
            For j As Integer = 0 To h - 1 'Step j + 1
                Dim v As Single = CSng((j / (h - 1)) * sy)
                For i As Integer = 0 To w - 1 'Step i + 1
                    Dim u As Single = CSng((i / (w - 1)) * sx)
                    fastvertex(p).setUV(u, v)
                    p += 1
                Next
            Next
        End Sub

        '/*
        'public void tilt(float fact)
        '{
        '	rebuild();
        '	for (int i=0;i<vertices;i++)
        '		fastvertex[i].pos=warp_Vector.add(fastvertex[i].pos,warp_Vector.random(fact));

        '	regenerate();
        '}
        '*/


        Public Function minimum() As warp_Vector
            If vertices = 0 Then Return New warp_Vector(0.0F, 0.0F, 0.0F)
            Dim minX As Single = fastvertex(0).pos.x
            Dim minY As Single = fastvertex(0).pos.y
            Dim minZ As Single = fastvertex(0).pos.z

            For i As Integer = 1 To vertices - 1 'Step i + 1
                If fastvertex(i).pos.x < minX Then minX = fastvertex(i).pos.x
                If fastvertex(i).pos.y < minY Then minY = fastvertex(i).pos.y
                If fastvertex(i).pos.z < minZ Then minZ = fastvertex(i).pos.z
            Next

            Return New warp_Vector(minX, minY, minZ)
        End Function

        Public Function maximum() As warp_Vector
            If vertices = 0 Then Return New warp_Vector(0.0F, 0.0F, 0.0F)
            Dim maxX As Single = fastvertex(0).pos.x
            Dim maxY As Single = fastvertex(0).pos.y
            Dim maxZ As Single = fastvertex(0).pos.z

            For i As Integer = 1 To vertices - 1 'Step i + 1
                If fastvertex(i).pos.x > maxX Then maxX = fastvertex(i).pos.x
                If fastvertex(i).pos.y > maxY Then maxY = fastvertex(i).pos.y
                If fastvertex(i).pos.z > maxZ Then maxZ = fastvertex(i).pos.z
            Next
            Return New warp_Vector(maxX, maxY, maxZ)
        End Function



        Public Sub detach()
            Dim center As warp_Vector = getCenter()

            For i As Integer = 0 To vertices - 1 Step i + 1
                fastvertex(i).pos.x -= center.x
                fastvertex(i).pos.y -= center.y
                fastvertex(i).pos.z -= center.z
            Next

            shift(center)
        End Sub

        Public Function getCenter() As warp_Vector
            Dim max As warp_Vector = maximum()
            Dim min As warp_Vector = minimum()

            Return New warp_Vector((max.x + min.x) / 2, (max.y + min.y) / 2, (max.z + min.z) / 2)
        End Function

        Public Function getDimension() As warp_Vector
            Dim max As warp_Vector = maximum()
            Dim min As warp_Vector = minimum()

            Return New warp_Vector(max.x - min.x, max.y - min.y, max.z - min.z)
        End Function

        Public Sub matrixMeltdown()
            rebuild()
            Dim i As Integer
            For i = vertices - 1 To 0 Step -1
                fastvertex(i).pos = fastvertex(i).pos.transform(matrix)
            Next

            regenerate()
            matrix.reset()
            normalmatrix.reset()
        End Sub

        Public Function getClone() As warp_Object
            Dim obj As warp_Object = New warp_Object()
            rebuild()
            For i As Integer = 0 To vertices - 1 'Step i + 1
                obj.addVertex(fastvertex(i).getClone())
            Next
            For i As Integer = 0 To triangles - 1 'Step i + 1
                obj.addTriangle(fasttriangle(i).getClone())
            Next
            obj.name = name & " [cloned]"
            obj.material = material
            obj.matrix = matrix.getClone()
            obj.normalmatrix = normalmatrix.getClone()
            obj.rebuild()
            Return obj
        End Function

        '/*
        '		public void removeDuplicateVertices()
        '		{
        '			rebuild();
        '			Vector edgesToCollapse=new Vector();
        '			for (int i=0;i<vertices;i++)
        '				for (int j=i+1;j<vertices;j++)
        '					if (vertex[i].equals(vertex[j],0.0001f))
        '						edgesToCollapse.addElement(new warp_Edge(vertex[i],vertex[j]));


        '			Enumeration enum=edgesToCollapse.elements();
        '			while(enum.hasMoreElements()) 
        '			{
        '				edgeCollapse((warp_Edge)enum.nextElement());
        '			}

        '			removeDegeneratedTriangles();
        '		}

        '		public void removeDegeneratedTriangles()
        '		{
        '			rebuild();
        '			for (int i=0;i<triangles;i++)
        '				if (triangle[i].degenerated()) removeTriangleAt(i);

        '			dirty=true;
        '			rebuild();
        '		}

        '		private void edgeCollapse(warp_Edge edge)
        '		// Collapses the edge [u,v] by replacing v by u
        '		{
        '			warp_Vertex u=edge.start();
        '			warp_Vertex v=edge.end();
        '			if (!vertexData.contains(u)) return;
        '			if (!vertexData.contains(v)) return;
        '			rebuild();
        '			warp_Triangle tri;
        '			for (int i=0; i<triangles; i++)
        '			{
        '				tri=triangle(i);
        '				if (tri.p1==v) tri.p1=u;
        '				if (tri.p2==v) tri.p2=u;
        '				if (tri.p3==v) tri.p3=u;
        '			}
        '			vertexData.removeElement(v);
        '		}
        '		*/
    End Class
End Namespace