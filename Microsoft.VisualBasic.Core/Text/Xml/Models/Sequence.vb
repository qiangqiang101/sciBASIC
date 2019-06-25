﻿#Region "Microsoft.VisualBasic::f28a74732c7f6d4571048c1a013cdb91, Microsoft.VisualBasic.Core\ComponentModel\Ranges\Sequence.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
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



' /********************************************************************************/

' Summaries:

'     Class Sequence
' 
'         Properties: Max, Min, n, Range, Steps
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: (+2 Overloads) IsInside, IsOverlapping, ToArray, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Namespace Text.Xml.Models

    ''' <summary>
    ''' A numeric sequence model
    ''' </summary>
    <XmlType("sequence")>
    Public Class Sequence : Implements Enumeration(Of Double)

        ''' <summary>
        ''' [min, max]
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("range")>
        Public Property range As DoubleRange

        ''' <summary>
        ''' 将目标区间对象<see cref="range"/>平均划分为n个区间
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property n As Integer

        Public ReadOnly Property steps As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return range.Length / n
            End Get
        End Property

        Sub New(a#, b#, n%)
            Me.range = New DoubleRange(a, b)
            Me.n = n
        End Sub

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToArray() As Double()
            Return range.Enumerate(n)
        End Function

        Public Overrides Function ToString() As String
            Return range.ToString
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Double) Implements Enumeration(Of Double).GenericEnumerator
            For Each value As Double In range.Enumerate(n)
                Yield value
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Double).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace
