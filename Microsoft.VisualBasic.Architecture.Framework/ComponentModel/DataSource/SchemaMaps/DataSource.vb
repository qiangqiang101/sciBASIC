﻿#Region "Microsoft.VisualBasic::54310639710c166e8666c3c8b63602a9, ..\VisualBasic_AppFramework\Microsoft.VisualBasic.Architecture.Framework\ComponentModel\DataSource\SchemaMaps\DataSource.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.DataSourceModel.SchemaMaps

    ''' <summary>
    ''' <see cref="DataFrameColumnAttribute"/>属性的别称
    ''' </summary>
    Public Class Field : Inherits DataFrameColumnAttribute

        ''' <summary>
        ''' Initializes a new instance by name.
        ''' </summary>
        ''' <param name="FieldName">The name.</param>
        Public Sub New(FieldName As String)
            Call MyBase.New(FieldName)
        End Sub
    End Class

    ''' <summary>
    ''' Represents a column of certain data frames. The mapping between to schema is also can be represent by this attribute. 
    ''' (也可以使用这个对象来完成在两个数据源之间的属性的映射，由于对于一些列名称的属性值缺失的映射而言，
    ''' 其是使用属性名来作为列映射名称的，故而在修改这些没有预设的列名称的映射属性的属性名的时候，请注意
    ''' 要小心维护这种映射关系)
    ''' </summary>
    <AttributeUsage(AttributeTargets.[Property] Or AttributeTargets.Field,
                    Inherited:=True,
                    AllowMultiple:=False)>
    Public Class DataFrameColumnAttribute : Inherits Attribute

        Protected Shared ReadOnly __emptyIndex As String() = New String(-1) {}

        ''' <summary>
        ''' Gets the index.
        ''' </summary>
        Public ReadOnly Property Index() As Integer

        ''' <summary>
        ''' Gets the name.
        ''' </summary>
        Public ReadOnly Property Name() As String

        Public Property Description As String

        ''' <summary>
        ''' Initializes a new instance by name.
        ''' </summary>
        ''' <param name="FieldName">The name.</param>
        Public Sub New(FieldName As String)
            If String.IsNullOrEmpty(FieldName) Then
                Throw New ArgumentNullException("name")
            End If
            Me._Name = FieldName
            Me._Index = -1
        End Sub

        ''' <summary>
        ''' Initializes a new instance by index.
        ''' </summary>
        ''' <param name="index">The index.</param>
        Public Sub New(index As Integer)
            If index < 0 Then
                Throw New ArgumentOutOfRangeException("index")
            End If
            Me._Name = Nothing
            Me._Index = index
        End Sub

        ''' <summary>
        ''' 会默认使用目标对象的反射的Name属性作为映射的名称
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _Index = -1
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">列名称，假若本参数为空的话，则使用属性名称</param>
        ''' <param name="index">从1开始的下标，表示为第几列</param>
        ''' <remarks></remarks>
        Sub New(Optional Name As String = "", Optional index As Integer = -1)
            _Name = Name
            _Index = index
        End Sub

        Public Overrides Function ToString() As String
            Return _Name
        End Function

        Const NameException As String = "Name must not be null when Index is not defined."

        Public Function SetNameValue(value As String) As DataFrameColumnAttribute
            If Me._Index < 0 AndAlso String.IsNullOrEmpty(value) Then
                Throw New ArgumentNullException(NameOf(value), NameException)
            End If
            Me._Name = value
            Return Me
        End Function

        Public Function GetIndex(names As String()) As Integer
            Return If(Index >= 0, Index, Array.IndexOf(If(names, __emptyIndex), Name))
        End Function

        ''' <summary>
        ''' 没有名称属性的映射使用属性名来表述
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadMapping(Of T)(
                      Optional ignores As String() = Nothing,
                      Optional mapsAll As Boolean = False) _
                                       As Dictionary(Of BindProperty(Of DataFrameColumnAttribute))

            Return LoadMapping(GetType(T), ignores, mapsAll)
        End Function

        ''' <summary>
        ''' Load the mapping property, if the custom attribute <see cref="DataFrameColumnAttribute"></see> 
        ''' have no name value, then the property name will be used as the mapping name.
        ''' (这个函数会自动给空名称值进行属性名的赋值操作的)
        ''' </summary>
        ''' <param name="typeInfo">The type should be a class type or its properties should have the 
        ''' mapping option which was created by the custom attribute <see cref="DataFrameColumnAttribute"></see>
        ''' </param>
        ''' <param name="ignores">这个是大小写敏感的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadMapping(
                               typeInfo As Type,
                      Optional ignores As String() = Nothing,
                      Optional mapsAll As Boolean = False) _
                                       As Dictionary(Of BindProperty(Of DataFrameColumnAttribute))

            Dim source As IEnumerable(Of KeyValuePair(Of DataFrameColumnAttribute, PropertyInfo)) =
                __source(typeInfo, If(ignores Is Nothing, {}, ignores), mapsAll)
            Dim LQuery As BindProperty(Of DataFrameColumnAttribute)() =
                LinqAPI.Exec(Of BindProperty(Of DataFrameColumnAttribute)) <=
                    From pInfo As KeyValuePair(Of DataFrameColumnAttribute, PropertyInfo)
                    In source
                    Let Mapping As DataFrameColumnAttribute =
                        If(String.IsNullOrEmpty(pInfo.Key.Name),  ' 假若名称是空的，则会在这里自动的使用属性名称进行赋值
                        pInfo.Key.SetNameValue(pInfo.Value.Name),
                        pInfo.Key)
                    Select New BindProperty(Of DataFrameColumnAttribute)(
                        Mapping,
                        pInfo.Value) ' 补全名称属性
            Dim out As New Dictionary(Of BindProperty(Of DataFrameColumnAttribute))(LQuery)
            Return out
        End Function

        Private Shared Iterator Function __source(
                                         type As Type,
                                         ignores As String(),
                                         mapsAll As Boolean) _
                                                 As IEnumerable(Of KeyValuePair(Of DataFrameColumnAttribute, PropertyInfo))

            Dim props As IEnumerable(Of PropertyInfo) =
                From p As PropertyInfo
                In type.GetProperties(BindingFlags.Public + BindingFlags.Instance)
                Where Array.IndexOf(ignores, p.Name) = -1
                Select p

            If Not mapsAll Then
                For Each x In From pInfo As PropertyInfo
                              In props
                              Let attrs As Object() =
                                  pInfo.GetCustomAttributes(GetType(DataFrameColumnAttribute), True)
                              Where Not attrs.IsNullOrEmpty
                              Let attr = DirectCast(attrs.First, DataFrameColumnAttribute)
                              Select New KeyValuePair(Of DataFrameColumnAttribute, PropertyInfo)(attr, pInfo)
                    Yield x
                Next
            Else
                For Each x In From pInfo As PropertyInfo
                              In props
                              Let attr = New DataFrameColumnAttribute
                              Select New KeyValuePair(Of DataFrameColumnAttribute, PropertyInfo)(attr, pInfo)
                    Yield x
                Next
            End If
        End Function
    End Class

    Public MustInherit Class DataFrameIO(Of TAttributeType As DataFrameColumnAttribute)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function InitializeSchema(Of TEntityType As Class)() As TAttributeType()
    End Class
End Namespace
