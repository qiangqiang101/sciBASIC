﻿#Region "Microsoft.VisualBasic::03555bdc5901774f62a329ba643bf659, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\ApplicationServices\Debugger\DebuggerArgs.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.UnixBash.FileSystem

Namespace ApplicationServices.Debugging

    ''' <summary>
    ''' 调试器设置参数模块
    ''' </summary>
    Module DebuggerArgs

        ''' <summary>
        ''' 错误日志的文件存储位置，默认是在AppData里面
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ErrLogs As Func(Of String) = Nothing

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="log">日志文本</param>
        Public Sub SaveErrorLog(log As String)
            If Not ErrLogs Is Nothing Then
                Call log.SaveTo(_ErrLogs())
            End If
        End Sub

        ''' <summary>
        ''' Logging command shell history.
        ''' </summary>
        ''' <param name="args"></param>
        Private Sub __logShell(args As CommandLine.CommandLine)
            Dim CLI As String = App.ExecutablePath & " " & args.CLICommandArgvs
            Dim log As String = $"{PS1.Fedora12.ToString} {CLI}"
            Dim logFile As String = App.LogErrDIR.ParentPath & "/.shells.log"

            If InStr(logFile.ParentPath, "/sbin/") = 1 Then
                ' 当程序运行在apache+linux web服务器上面的时候，
                ' 对于apache用户，linux服务器上面得到的文件夹是/sbin，则会出现权限错误，这个时候重定向到应用程序自身的文件夹之中
                logFile = App.HOME & "/.logs/.shells.log"
            End If

            Try
                If FileHandles.Wait(file:=logFile) Then
                    Call FileIO.FileSystem.CreateDirectory(logFile.ParentPath)
                    Call FileIO.FileSystem.WriteAllText(logFile, log & vbCrLf, True)
                End If
            Catch ex As Exception
                ' 连日志记录都出错了，已经没有地方可以写日志了，则只能够直接将错误信息以警告的方式打印出来
                Call ex.Message.Warning

                '[ERROR 09/07/2017 1052:12] :System.Exception : LogException ---> System.Exception: Exception of type 'System.Exception' was thrown. ---> System.ArgumentNullException: Value cannot be null.
                'Parameter name: path
                '                at System.IO.DirectoryInfo.CheckPath(System.String path) [0x00003] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo..ctor(System.String path, System.Boolean simpleOriginalPath) [0x00006] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo..ctor(System.String path) [0x00000] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:.ctor(Of String)
                '                at Microsoft.VisualBasic.FileIO.FileSystem.GetDirectoryInfo(System.String directory) [0x00000] in <828807dda9f14f24a7db780c6c644162>:0 
                '  at Microsoft.VisualBasic.ProgramPathSearchTool.GetDirectoryFullPath(System.String dir) [0x00000] in :0 
                '   --- End of inner exception stack trace ---
                '   --- End of inner exception stack trace ---
                '[ERROR 09/07/2017 10:52:12] :System.Exception : GetDirectoryFullPath ---> System.Exception: Exception of type 'System.Exception' was thrown. ---> System.ArgumentNullException: Value cannot be null.
                'Parameter name: path
                '                at System.IO.DirectoryInfo.CheckPath(System.String path) [0x00003] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo..ctor(System.String path, System.Boolean simpleOriginalPath) [0x00006] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo..ctor(System.String path) [0x00000] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:.ctor(Of String)
                '                at Microsoft.VisualBasic.FileIO.FileSystem.GetDirectoryInfo(System.String directory) [0x00000] in <828807dda9f14f24a7db780c6c644162>:0 
                '  at Microsoft.VisualBasic.ProgramPathSearchTool.GetDirectoryFullPath(System.String dir) [0x00000] in :0 
                '   --- End of inner exception stack trace ---
                '   --- End of inner exception stack trace ---
                '[ERROR 09/07/2017 10:52:12] :System.Exception : InitDebuggerEnvir ---> System.UnauthorizedAccessException: Access to the path "/sbin/.local" Is denied.
                '  at System.IO.Directory.CreateDirectoriesInternal(System.String path) [0x0005e] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.Directory.CreateDirectory(System.String path) [0x0008f] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo.Create() [0x00000] in <902ab9e386384bec9c07fa19aa938869>: 0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:Create()
                '                at System.IO.Directory.CreateDirectoriesInternal(System.String path) [0x00036] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.Directory.CreateDirectory(System.String path) [0x0008f] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo.Create() [0x00000] in <902ab9e386384bec9c07fa19aa938869>: 0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:Create()
                '                at System.IO.Directory.CreateDirectoriesInternal(System.String path) [0x00036] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.Directory.CreateDirectory(System.String path) [0x0008f] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo.Create() [0x00000] in <902ab9e386384bec9c07fa19aa938869>: 0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:Create()
                '                at System.IO.Directory.CreateDirectoriesInternal(System.String path) [0x00036] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.Directory.CreateDirectory(System.String path) [0x0008f] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.DirectoryInfo.Create() [0x00000] in <902ab9e386384bec9c07fa19aa938869>: 0 
                '  at(wrapper remoting-invoke-With-check) System.IO.DirectoryInfo:Create()
                '                at System.IO.Directory.CreateDirectoriesInternal(System.String path) [0x00036] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at System.IO.Directory.CreateDirectory(System.String path) [0x0008f] in <902ab9e386384bec9c07fa19aa938869>:0 
                '  at Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(System.String directory) [0x00025] in <828807dda9f14f24a7db780c6c644162>:0 
                '  at Microsoft.VisualBasic.Debugging.DebuggerArgs.__logShell(Microsoft.VisualBasic.CommandLine.CommandLine args) [0x00056] in :0 
                '  at Microsoft.VisualBasic.Debugging.DebuggerArgs.InitDebuggerEnvir(Microsoft.VisualBasic.CommandLine.CommandLine args, System.String caller) [0x00018] in :0 
                '   --- End of inner exception stack trace ---
            End Try
        End Sub

        ''' <summary>
        ''' Some optional VisualBasic debugger parameter help information.(VisualBasic调试器的一些额外的开关参数的帮助信息)
        ''' </summary>
        Public Const DebuggerHelps As String =
        "Additional VisualBasic App debugger arguments:   --echo on/off/all/warn/error /mute /auto-paused --err <filename.log> /ps1 <bash_PS1> /@set ""var1='value1';var2='value2'""

    [--echo] The debugger echo options, it have 5 values:
             on     App will output all of the debugger echo message, but the VBDebugger.Mute option is enabled, disable echo options can be control by the program code;
             off    App will not output any debugger echo message, the VBDebugger.Mute option is disabled;
             all    App will output all of the debugger echo message, the VBDebugger.Mute option is disabled;
             warn   App will only output warning level and error level message;
             error  App will just only output error level message.

    [--err]  The error logs save copy:
             If there is an unhandled exception in your App, this will cause your program crashed, then the exception will be save to error log, 
             and by default the error log is saved in the AppData, then if this option is enabled, the error log will saved a copy to the 
             specific location at the mean time. 

    [/mute]  This boolean flag will mute all debugger output.

    [/auto-paused] This boolean flag will makes the program paused after the command is executed done. and print a message on the console:
                       ""Press any key to continute..."" 

    [/@set]  This option will be using for settings of the interval environment variable.

    ** Additionally, you can using ""/linux-bash"" command for generates the bash shortcuts on linux system.
"
        Public ReadOnly Property AutoPaused As Boolean

        ''' <summary>
        ''' Initialize the global environment variables in this App process.
        ''' </summary>
        ''' <param name="args">--echo on/off/all/warn/error --err &lt;path.log></param>
        <Extension> Public Sub InitDebuggerEnvir(args As CommandLine.CommandLine, <CallerMemberName> Optional caller As String = Nothing)
            If Not String.Equals(caller, "Main") Then
                Return  ' 这个调用不是从Main出发的，则不设置环境了，因为这个环境可能在其他的代码上面设置过了
            Else
                Try
                    Call __logShell(args)
                Catch ex As Exception
                    ' 因为只是进行命令行的调用历史的记录，所以实在不行的话就放弃这次的调用记录
                    Call ex.PrintException
                End Try
            End If

            Dim opt As String = args <= "--echo"
            Dim log As String = args <= "--err"

            If Not String.IsNullOrEmpty(log) Then
                _ErrLogs = Function() log
            Else

            End If

            Dim config As Config = Config.Load

            If String.IsNullOrEmpty(opt) Then ' 默认的on参数
                VBDebugger.__level = config.level
            Else
                Select Case opt.ToLower
                    Case "on"
                        VBDebugger.__level = DebuggerLevels.On
                    Case "off"
                        VBDebugger.__level = DebuggerLevels.Off
                    Case "all"
                        VBDebugger.__level = DebuggerLevels.All
                    Case "warn", "warning"
                        VBDebugger.__level = DebuggerLevels.Warning
                    Case "err", "error"
                        VBDebugger.__level = DebuggerLevels.Error
                    Case Else
                        VBDebugger.__level = DebuggerLevels.On
                        Call Console.WriteLine($"[INFO] The debugger argument value --echo:={opt} is invalid, using default settings.")
                End Select
            End If

            _AutoPaused = args.GetBoolean("/auto-paused")

            If args.GetBoolean("/mute") Then
                VBDebugger.Mute = True
            Else
                VBDebugger.Mute = config.mute
            End If

            Dim vars As Dictionary(Of String, String) = args.GetDictionary("/@set")
            Dim disableLoadOptions As Boolean = args.GetBoolean("--load_options.disable")

            ' --load_options.disable 开关将会禁止所有的环境项目的设置
            ' 但是环境变量任然会进行加载设置

            If Not disableLoadOptions AndAlso Not vars.IsNullOrEmpty Then
                Call App.JoinVariables(
                    vars _
                    .Select(Function(x)
                                Return New NamedValue(Of String) With {
                                    .Name = x.Key,
                                    .Value = x.Value
                                }
                            End Function).ToArray)

                If vars.ContainsKey("Proxy") Then
                    WebServiceUtils.Proxy = vars("Proxy")
                    Call $"[Config] webUtils_proxy={WebServiceUtils.Proxy}".__INFO_ECHO
                End If
                If vars.ContainsKey("setwd") Then
                    App.CurrentDirectory = vars("setwd")
                    Call $"[Config] current_work_directory={App.CurrentDirectory}".__INFO_ECHO
                End If
                If vars.ContainsKey("buffer_size") Then
                    Call App.SetBufferSize(vars!buffer_size)
                End If
            End If

            ' /@var=name "value"
            For Each var As NamedValue(Of String) In args.ParameterList
                With var
                    If InStr(.Name, "/@var=", CompareMethod.Text) = 1 Then
                        Dim name$ = .Name.GetTagValue("=").Value
                        Call App.JoinVariable(name, .Value)
                    End If
                End With
            Next
        End Sub
    End Module
End Namespace