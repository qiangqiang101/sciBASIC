﻿#Region "Microsoft.VisualBasic::221dea0227f74273b85f02afa7c50459, Data\BinaryData\BinaryData\Stream\ByteOrder.vb"

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

' Enum ByteOrder
' 
' 
'  
' 
' 
' 
' Module ByteOrderHelper
' 
'     Properties: SystemByteOrder
' 
'     Function: NeedsReversion
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Represents the possible endianness of binary data.
''' </summary>
Public Enum ByteOrder As UShort
    ''' <summary>
    ''' The binary data is present in big endian.
    ''' </summary>
    BigEndian = &HFEFF

    ''' <summary>
    ''' The binary data is present in little endian.
    ''' </summary>
    LittleEndian = &HFFFE
End Enum

''' <summary>
''' Represents helper methods to handle data byte order.
''' </summary>
<HideModuleName> Public Module ByteOrderHelper

    Dim _systemByteOrder As ByteOrder
    Dim _networkByteOrderConvertor As Func(Of IEnumerable(Of Double), Byte())

    ''' <summary>
    ''' Gets the <see cref="ByteOrder"/> of the system executing the assembly.
    ''' </summary>
    Public ReadOnly Property SystemByteOrder() As ByteOrder
        Get
            If _systemByteOrder = 0 Then
                _systemByteOrder = If(BitConverter.IsLittleEndian, ByteOrder.LittleEndian, ByteOrder.BigEndian)
            End If
            Return _systemByteOrder
        End Get
    End Property

    Sub New()
        If BitConverter.IsLittleEndian Then
            _networkByteOrderConvertor = AddressOf networkByteOrderLittleEndian
        Else
            _networkByteOrderConvertor = AddressOf networkByteOrderBigEndian
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function NeedsReversion(order As ByteOrder) As Boolean
        Return order <> ByteOrderHelper.SystemByteOrder
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsNetworkByteOrderBuffer(data As IEnumerable(Of Double)) As Byte()
        Return _networkByteOrderConvertor(data)
    End Function

    Private Function networkByteOrderLittleEndian(d As IEnumerable(Of Double)) As Byte()
        Dim buffer As Byte() = d _
            .Select(AddressOf BitConverter.GetBytes) _
            .IteratesALL _
            .ToArray

        Call Array.Reverse(buffer)

        Return buffer
    End Function

    Private Function networkByteOrderBigEndian(d As IEnumerable(Of Double)) As Byte()
        Return d _
            .Select(AddressOf BitConverter.GetBytes) _
            .IteratesALL _
            .ToArray
    End Function
End Module
