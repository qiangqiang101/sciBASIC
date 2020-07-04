﻿Imports System.Drawing

Public Module ColorSpectrum

    ''' <summary>
    ''' takes wavelength in nm And returns an rgba value
    ''' </summary>
    ''' <param name="wavelength">wavelength (nm, range in [380,780])</param>
    ''' <returns></returns>
    Public Function wavelengthToColor(wavelength As Double) As Color
        Dim r#,
            g#,
            b#,
            alpha#
        Dim colorSpace As Color,
            wl = wavelength,
            gamma = 1

        If (wl >= 380 AndAlso wl < 440) Then
            r = -1 * (wl - 440) / (440 - 380)
            g = 0
            b = 1
        ElseIf (wl >= 440 AndAlso wl < 490) Then
            r = 0
            g = (wl - 440) / (490 - 440)
            b = 1
        ElseIf (wl >= 490 AndAlso wl < 510) Then
            r = 0
            g = 1
            b = -1 * (wl - 510) / (510 - 490)
        ElseIf (wl >= 510 AndAlso wl < 580) Then
            r = (wl - 510) / (580 - 510)
            g = 1
            b = 0
        ElseIf (wl >= 580 AndAlso wl < 645) Then
            r = 1
            g = -1 * (wl - 645) / (645 - 580)
            b = 0.0
        ElseIf (wl >= 645 AndAlso wl <= 780) Then
            r = 1
            g = 0
            b = 0
        Else
            r = 0
            g = 0
            b = 0
        End If

        ' intensty Is lower at the edges of the visible spectrum.
        If (wl > 780 OrElse wl < 380) Then
            alpha = 0
        ElseIf (wl > 700) Then
            alpha = (780 - wl) / (780 - 700)
        ElseIf (wl < 420) Then
            alpha = (wl - 380) / (420 - 380)
        Else
            alpha = 1
        End If

        colorSpace = Color.FromArgb(alpha, r, g, b)

        ' colorSpace Is an array with 5 elements.
        ' The first element Is the complete code as a string.  
        ' Use colorSpace[0] as Is to display the desired color.  
        ' use the last four elements alone Or together to access each of the individual r, g, b And a channels.  

        Return colorSpace
    End Function
End Module
