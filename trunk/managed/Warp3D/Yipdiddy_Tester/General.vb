Public Module General
    Public Function Crop(ByVal num As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
        If num < min Then Return min
        If num > max Then Return max
        Return num
    End Function

    Public Function Crop(ByVal num As Single, ByVal min As Single, ByVal max As Single) As Single
        If num < min Then Return min
        If num > max Then Return max
        Return num
    End Function

  
End Module
