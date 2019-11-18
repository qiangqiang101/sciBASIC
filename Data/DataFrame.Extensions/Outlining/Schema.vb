﻿Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.ComponentModels

Namespace Outlining

    Public Class Builder

        ''' <summary>
        ''' 一个对象之中只允许出现一个复杂类型的数组属性
        ''' </summary>
        ''' <returns></returns>
        Public Property SubTableSchema As NamedValue(Of Builder)
        Public Property Builder As RowBuilder

        Public ReadOnly Property Type As Type

        Sub New(type As Type, headers As IEnumerable(Of String), strict As Boolean)
            Me.Type = type
            Me.Builder = type.createBuilderByHeaders(headers, strict)
            Me.SubTableSchema = GetNextIndentLevel(type)

            If SubTableSchema.IsEmpty Then
                Call $"We found that '{type.FullName}' is a normal 2D data table, consider using ``LoadCsv`` extension method for read data...".Warning
            End If
        End Sub

        Private Sub New(type As Type)
            Me.Type = type
            ' 暂时先不初始化
            Me.Builder = Nothing
            Me.SubTableSchema = GetNextIndentLevel(type)
        End Sub

        Private Shared Function GetNextIndentLevel(type As Type) As NamedValue(Of Builder)
            Dim subTable As PropertyInfo = type _
                .GetProperties(PublicProperty) _
                .FirstOrDefault(AddressOf IsSubIndentColumn)

            If Not subTable Is Nothing Then
                Return New NamedValue(Of Builder) With {
                    .Name = subTable.Name,
                    .Value = New Builder(subTable.PropertyType)
                }
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 数组类型，并且元素类型不是基础类型
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Shared Function IsSubIndentColumn(p As PropertyInfo) As Boolean
            Dim type As Type = p.PropertyType

            If Not type.IsArray Then
                Return False
            ElseIf DataFramework.IsPrimitive(type.GetElementType) Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function GetBuilder(indentLevel As Integer) As Builder
            If indentLevel = 0 Then
                Return Me
            Else
                Dim builder As Builder = Me

                For i As Integer = 0 To indentLevel - 1
                    builder = builder.SubTableSchema
                Next

                Return builder
            End If
        End Function

        ''' <summary>
        ''' 实际上是做初始化，因为对象已经创建了
        ''' </summary>
        ''' <param name="indent"></param>
        ''' <param name="headers"></param>
        ''' <param name="strict"></param>
        ''' <returns></returns>
        Public Function CreateBuilder(indent As Integer, headers As IEnumerable(Of String), strict As Boolean) As Builder
            If indent = 0 Then
                Throw New InvalidExpressionException
            End If


        End Function

    End Class
End Namespace