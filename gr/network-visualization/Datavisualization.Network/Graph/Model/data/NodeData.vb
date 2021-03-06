﻿#Region "Microsoft.VisualBasic::d0a5c14b2e39118368c428445fd86a09, gr\network-visualization\Datavisualization.Network\Graph\Model\data\NodeData.vb"

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

    '     Class NodeData
    ' 
    '         Properties: betweennessCentrality, color, force, initialPostion, mass
    '                     neighborhoods, neighbours, origID, size, weights
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Clone, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization

Namespace Graph

    Public Class NodeData : Inherits GraphData

        ''' <summary>
        ''' Get length of the <see cref="neighbours"/> index array
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property neighborhoods As Integer
            Get
                If neighbours Is Nothing Then
                    Return 0
                Else
                    Return neighbours.Length
                End If
            End Get
        End Property

        ''' <summary>
        ''' 这个主要是为了兼容圆形或者矩形之类的大小信息
        ''' </summary>
        ''' <returns></returns>
        Public Property size As Double()

        ''' <summary>
        ''' Mass weight
        ''' </summary>
        ''' <returns></returns>
        Public Property mass As Single

        ''' <summary>
        ''' For 2d layout <see cref="FDGVector2"/> / 3d layout <see cref="FDGVector3"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property initialPostion As AbstractVector
        Public Property origID As String
        Public Property force As Point

        ''' <summary>
        ''' 颜色<see cref="SolidBrush"/>或者绘图<see cref="TextureBrush"/>
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public Property color As Brush

        <DumpNode>
        Public Property weights As Double()

        ''' <summary>
        ''' 与本节点相连接的其他节点的<see cref="Node.Label">编号</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode>
        Public Property neighbours As Integer()

        Public Property betweennessCentrality As Double

        Public Sub New()
            MyBase.New()

            mass = 1.0F
            initialPostion = Nothing
            ' for merging the graph
            origID = ""
        End Sub

        Sub New(copy As NodeData)
            Me.color = copy.color
            Me.force = copy.force
            Me.initialPostion = copy.initialPostion
            Me.label = copy.label
            Me.mass = copy.mass
            Me.neighbours = copy.neighbours.SafeQuery.ToArray
            Me.origID = copy.origID
            Me.Properties = New Dictionary(Of String, String)(copy.Properties)
            Me.size = If(copy.size Is Nothing, {}, copy.size.ToArray)
            Me.weights = copy.weights.SafeQuery.ToArray
        End Sub

        Public Function Clone() As NodeData
            Return DirectCast(Me.MemberwiseClone, NodeData)
        End Function

        Public Overrides Function ToString() As String
            Return initialPostion.ToString
        End Function
    End Class
End Namespace
