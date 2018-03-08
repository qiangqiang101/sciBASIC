﻿#Region "Microsoft.VisualBasic::4ea800f3fe95e2883ca33dcbdd679aac, docs\guides\Example\PipelineTest\Program.vb"

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

' Module Program
' 
'     Function: JustStdDevice, Main, OnlySupportsFile, SupportsBothFileAndPipeline
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/file", Usage:="/file /in <file.txt> [/out <out.txt>]")>
    Public Function OnlySupportsFile(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".output.json")
        Return [in].ReadAllLines.GetJson.SaveTo(out).CLICode
    End Function

    <ExportAPI("/std", Usage:="/std <input> <output>")>
    Public Function JustStdDevice() As Integer
        Using input = Console.OpenStandardInput, output = New StreamWriter(Console.OpenStandardOutput)
            Call output.Write(New StreamReader(input).ReadToEnd.lTokens.GetJson)
        End Using

        Return 0
    End Function

    <ExportAPI("/pipe.Test", Usage:="/pipe.Test /in <file.txt/std_in> [/out <out.txt/std_out>]")>
    Public Function SupportsBothFileAndPipeline(args As CommandLine) As Integer
        Using out = args.OpenStreamOutput("/out")
            Dim inData As String() = args.OpenStreamInput("/in").ReadToEnd.lTokens
            Call out.Write(inData.GetJson)
        End Using

        Return 0
    End Function

    <ExportAPI("/throw.ex")>
    <Usage("/throw.ex /type <name> /message <text>")>
    Public Function CreateException(args As CommandLine) As Integer
        Dim type$ = args("/type")
        Dim message$ = args("/message")
        Dim ex As Exception = Activator.CreateInstance(System.Type.GetType(type, throwOnError:=True), {message})

        Throw ex
    End Function

    <ExportAPI("/test.catch")>
    <Usage("/test.catch")>
    Public Function TestException(args As CommandLine) As Integer
        Dim app As New InteropService(Microsoft.VisualBasic.App.ExecutablePath)
        Dim child = app.RunDotNetApp($"/throw.ex /type {GetType(NotImplementedException).FullName} /message ""Hello world!""")

        Call child.Run()

        Dim err$ = InteropService.GetLastError(child)


        Throw New Exception(err)
    End Function
End Module
