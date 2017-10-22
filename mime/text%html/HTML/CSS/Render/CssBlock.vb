﻿#Region "Microsoft.VisualBasic::dcd0c3bb5ec8e0b987ecdb3a834a74bd, ..\sciBASIC#\mime\text%html\HTML\CSS\Render\CssBlock.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Reflection
Imports System.Text.RegularExpressions
Imports HTMLParser = Microsoft.VisualBasic.MIME.Markup.HTML.Render.Parser

Namespace HTML.CSS.Render

    ''' <summary>
    ''' Represents a block of CSS property values
    ''' </summary>
    ''' <remarks>
    ''' To learn more about CSS blocks visit CSS spec:
    ''' http://www.w3.org/TR/CSS21/syndata.html#block
    ''' </remarks>
    Public Class CssBlock

#Region "Ctor"

        ''' <summary>
        ''' Initializes internal's
        ''' </summary>
        Private Sub New()
            _propertyValues = New Dictionary(Of PropertyInfo, String)()
            _properties = New Dictionary(Of String, String)()
        End Sub

        ''' <summary>
        ''' Creates a new block from the block's source
        ''' </summary>
        ''' <param name="blockSource"></param>
        Public Sub New(blockSource As String)
            Me.New()
            _BlockSource = blockSource

            'Extract property assignments
            Dim matches As MatchCollection = HTMLParser.Match(HTMLParser.CssProperties, blockSource)

            'Scan matches
            For Each match As Match In matches
                'Split match by colon
                Dim chunks As String() = match.Value.Split(":"c)

                If chunks.Length <> 2 Then
                    Continue For
                End If

                'Extract property name and value
                Dim propName As String = chunks(0).Trim()
                Dim propValue As String = chunks(1).Trim()

                'Remove semicolon
                If propValue.EndsWith(";") Then
                    propValue = propValue.Substring(0, propValue.Length - 1).Trim()
                End If

                'Add property to list
                Properties.Add(propName, propValue)

                'Register only if property checks with reflection
                If CssBox._properties.ContainsKey(propName) Then
                    PropertyValues.Add(CssBox._properties(propName), propValue)
                End If
            Next
        End Sub

#End Region

#Region "Props"

        ''' <summary>
        ''' Gets the properties and its values
        ''' </summary>
        Public ReadOnly Property Properties() As Dictionary(Of String, String)

        ''' <summary>
        ''' Gets the dictionary with property-ready values
        ''' </summary>
        Public ReadOnly Property PropertyValues() As Dictionary(Of PropertyInfo, String)

        ''' <summary>
        ''' Gets the block's source
        ''' </summary>
        Public ReadOnly Property BlockSource() As String
#End Region

#Region "Method"

        ''' <summary>
        ''' Updates the PropertyValues dictionary
        ''' </summary>
        Friend Sub UpdatePropertyValues()
            PropertyValues.Clear()

            For Each prop As String In Properties.Keys
                If CssBox._properties.ContainsKey(prop) Then
                    PropertyValues.Add(CssBox._properties(prop), Properties(prop))
                End If
            Next
        End Sub

        ''' <summary>
        ''' Asigns the style on this block o the specified box
        ''' </summary>
        ''' <param name="b"></param>
        Public Sub AssignTo(b As CssBox)
            For Each prop As PropertyInfo In PropertyValues.Keys
                Dim value As String = PropertyValues(prop)

                If value = CssConstants.Inherit AndAlso b.ParentBox IsNot Nothing Then
                    value = Convert.ToString(prop.GetValue(b.ParentBox, Nothing))
                End If

                prop.SetValue(b, value, Nothing)
            Next
        End Sub
#End Region
    End Class
End Namespace
