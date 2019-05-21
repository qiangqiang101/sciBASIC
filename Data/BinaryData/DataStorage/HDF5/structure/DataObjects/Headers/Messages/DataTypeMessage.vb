﻿#Region "Microsoft.VisualBasic::731a6e80238e7d72970534c210739b66, mime\application%netcdf\HDF5\structure\DataObjects\Headers\Messages\DataTypeMessage.vb"

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

'     Class DataTypeMessage
' 
'         Properties: address, byteSize, isBigEndian, isLittleEndian, structureMembers
'                     type
' 
'         Constructor: (+1 Overloads) Sub New
'         Sub: printValues
' 
' 
' /********************************************************************************/

#End Region

'
' * Mostly copied from NETCDF4 source code.
' * refer : http://www.unidata.ucar.edu
' * 
' * Modified by iychoi@email.arizona.edu
' 

Imports System.Text
Imports Microsoft.VisualBasic.Data.IO.HDF5.IO
Imports BinaryReader = Microsoft.VisualBasic.Data.IO.HDF5.IO.BinaryReader

Namespace HDF5.[Structure]

    Public Enum DataTypes
        ''' <summary>
        ''' Fixed-Point
        ''' </summary>
        DATATYPE_FIXED_POINT = 0
        ''' <summary>
        ''' Floating-point
        ''' </summary>
        DATATYPE_FLOATING_POINT = 1
        ''' <summary>
        ''' Time
        ''' </summary>
        DATATYPE_TIME = 2
        ''' <summary>
        ''' String
        ''' </summary>
        DATATYPE_STRING = 3
        ''' <summary>
        ''' Bit field
        ''' </summary>
        DATATYPE_BIT_FIELD = 4
        ''' <summary>
        ''' Opaque
        ''' </summary>
        DATATYPE_OPAQUE = 5
        ''' <summary>
        ''' Compound
        ''' </summary>
        DATATYPE_COMPOUND = 6
        ''' <summary>
        ''' Reference
        ''' </summary>
        DATATYPE_REFERENCE = 7
        ''' <summary>
        ''' Enumerated
        ''' </summary>
        DATATYPE_ENUMS = 8
        ''' <summary>
        ''' Variable-Length
        ''' </summary>
        DATATYPE_VARIABLE_LENGTH = 9
        ''' <summary>
        ''' Array
        ''' </summary>
        DATATYPE_ARRAY = 10
    End Enum

    ''' <summary>
    ''' The datatype message defines the datatype for each element of a dataset or 
    ''' a common datatype for sharing between multiple datasets. A datatype can 
    ''' describe an atomic type like a fixed- or floating-point type or more complex 
    ''' types like a C struct (compound datatype), array (array datatype), or C++ 
    ''' vector (variable-length datatype).
    '''
    ''' Datatype messages that are part Of a dataset Object Do Not describe how 
    ''' elements are related To one another; the dataspace message Is used For that 
    ''' purpose. Datatype messages that are part Of a committed datatype (formerly 
    ''' named datatype) message describe a common datatype that can be Shared by 
    ''' multiple datasets In the file.
    ''' </summary>
    Public Class DataTypeMessage

        ''' <summary>
        ''' 当前的这个对象在文件之中的起始位置
        ''' </summary>
        Private m_address As Long
        Private m_type As DataTypes
        Private m_version As Integer
        Private m_flags As Byte()
        Private m_byteSize As Integer
        Private m_littleEndian As Boolean

        Private m_unsigned As Boolean
        Private m_timeTypeByteSize As Integer
        Private m_opaqueDesc As String
        Private m_referenceType As Integer
        Private m_members As List(Of StructureMember)
        Private m_isOK As Boolean

        Private m_base As DataTypeMessage

        Dim encoding As Encoding

        Public Sub New([in] As BinaryReader, sb As Superblock, address As Long)
            [in].offset = address

            Me.m_address = address

            ' common base constructor

            Dim tandv As Byte = [in].readByte()
            Me.m_type = CType(tandv And &HF, DataTypes)
            Me.m_version = ((tandv And &HF0) >> 4)

            Me.m_flags = [in].readBytes(3)
            Me.m_byteSize = [in].readInt()
            Me.m_littleEndian = ((Me.m_flags(0) And &H1) = 0)
            Me.m_timeTypeByteSize = 4

            Me.m_isOK = True

            If Me.m_type = DataTypes.DATATYPE_FIXED_POINT Then
                Me.m_unsigned = ((Me.m_flags(0) And &H8) = 0)

                Dim bitOffset As Short = [in].readShort()
                Dim bitPrecision As Short = [in].readShort()

                Me.m_isOK = (bitOffset = 0) AndAlso (bitPrecision Mod 8 = 0)
            ElseIf Me.m_type = DataTypes.DATATYPE_FLOATING_POINT Then
                Dim bitOffset As Short = [in].readShort()
                Dim bitPrecision As Short = [in].readShort()
                Dim expLocation As Byte = [in].readByte()
                Dim expSize As Byte = [in].readByte()
                Dim manLocation As Byte = [in].readByte()
                Dim manSize As Byte = [in].readByte()
                Dim expBias As Integer = [in].readInt()
            ElseIf Me.m_type = DataTypes.DATATYPE_TIME Then
                Dim bitPrecision As Short = [in].readShort()
                Me.m_timeTypeByteSize = bitPrecision \ 8
            ElseIf Me.m_type = DataTypes.DATATYPE_STRING Then
                Dim ptype As Integer = Me.m_flags(0) And &HF
            ElseIf Me.m_type = DataTypes.DATATYPE_BIT_FIELD Then
                Dim bitOffset As Short = [in].readShort()
                Dim bitPrecision As Short = [in].readShort()
            ElseIf Me.m_type = DataTypes.DATATYPE_OPAQUE Then
                Dim len As Byte = Me.m_flags(0)
                Me.m_opaqueDesc = If((len > 0), [in].readASCIIString(len).Trim(), Nothing)
            ElseIf Me.m_type = DataTypes.DATATYPE_COMPOUND Then
                Dim nmembers As Integer = (Me.m_flags(1) * 256) + Me.m_flags(0)
                Me.m_members = New List(Of StructureMember)()

                For i As Integer = 0 To nmembers - 1
                    Me.m_members.Add(New StructureMember([in], sb, [in].offset, Me.m_version, Me.m_byteSize))
                Next
            ElseIf Me.m_type = DataTypes.DATATYPE_REFERENCE Then
                Me.m_referenceType = Me.m_flags(0) And &HF
            ElseIf Me.m_type = DataTypes.DATATYPE_ENUMS Then
                ' throw new IOException( "data type enums is not implemented" );

                Dim nmembers As Integer = ReadHelper.bytesToUnsignedInt(Me.m_flags(1), Me.m_flags(0))
                Me.m_base = New DataTypeMessage([in], sb, [in].offset)
                ' base type
                ' read the enums
                Dim enumName As [String]() = New [String](nmembers - 1) {}
                For i As Integer = 0 To nmembers - 1
                    If Me.m_version < 3 Then
                        enumName(i) = ReadHelper.readString8([in])
                    Else
                        ' padding
                        enumName(i) = [in].readASCIIString()
                        ' no padding
                    End If
                Next

                ' read the values; must switch to base byte order (!)
                If Not Me.m_base.m_littleEndian Then
                    [in].setBigEndian()
                End If

                Dim enumValue As Integer() = New Integer(nmembers - 1) {}
                For i As Integer = 0 To nmembers - 1
                    enumValue(i) = CInt(ReadHelper.readVariableSizeUnsigned([in], Me.m_base.m_byteSize))
                Next
                ' assume size is 1, 2, or 4

                'enumTypeName = objectName;
                'map = new TreeMap<Integer, String>();
                'for (int i = 0; i < nmembers; i++)
                '    map.put(enumValue[i], enumName[i]);

                [in].setLittleEndian()
            ElseIf Me.m_type = DataTypes.DATATYPE_VARIABLE_LENGTH Then
                ' Throw New Exception("data type variable length is not implemented")


                Dim paddingType = [in].readInt
                Dim charEncoding = [in].readInt

                If charEncoding = 0 Then
                    encoding = Encoding.ASCII
                ElseIf charEncoding = 1 Then
                    encoding = Encoding.UTF8
                Else
                    Throw New NotImplementedException
                End If

            ElseIf Me.m_type = DataTypes.DATATYPE_ARRAY Then
                Throw New Exception("data type array is not implemented")
            End If
        End Sub

        Public Overridable ReadOnly Property address() As Long
            Get
                Return Me.m_address
            End Get
        End Property

        Public Overridable ReadOnly Property isLittleEndian() As Boolean
            Get
                Return Me.m_littleEndian
            End Get
        End Property

        Public Overridable ReadOnly Property isBigEndian() As Boolean
            Get
                Return Not Me.m_littleEndian
            End Get
        End Property

        Public Overridable ReadOnly Property type() As DataTypes
            Get
                Return Me.m_type
            End Get
        End Property

        Public Overridable ReadOnly Property byteSize() As Integer
            Get
                Return Me.m_byteSize
            End Get
        End Property

        Public Overridable ReadOnly Property structureMembers() As List(Of StructureMember)
            Get
                Return Me.m_members
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return type.ToString
        End Function

        Public Overridable Sub printValues()
            Console.WriteLine("DataTypeMessage >>>")
            Console.WriteLine("address : " & Me.m_address)
            Console.WriteLine("data type : " & Me.m_type)
            Console.WriteLine("byteSize : " & Me.m_byteSize)

            If Me.m_members IsNot Nothing Then
                For Each mem As StructureMember In Me.m_members
                    mem.printValues()
                Next
            End If
            Console.WriteLine("DataTypeMessage <<<")
        End Sub

    End Class

End Namespace
