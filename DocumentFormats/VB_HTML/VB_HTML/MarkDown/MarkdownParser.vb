﻿Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace MarkDown

    Public Module MarkdownParser

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">The file path to the markdown text document.</param>
        ''' <returns></returns>
        Public Function MarkdownParser(path As String) As Markup
            Return path.ReadAllText.SyntaxParser
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="md">The markdown file text content, not file path</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function SyntaxParser(md As String) As Markup
            Dim lines As String() = md.lTokens

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <![CDATA[
        ''' <h3 id = "header" > Headers</h3>
        '''
        ''' Markdown supports two styles Of headers, [Setext] [1] And [atx] [2].
        '''
        ''' Setext-style headers are "underlined" using equal signs (for first-level
        ''' headers) And dashes (for second-level headers). For example:
        '''
        '''     This Is an H1
        '''     =============
        '''
        '''     This Is an H2
        '''     -------------
        '''
        ''' Any number Of underlining `=`'s or `-`'s will work.
        '''
        ''' Atx-style headers use 1-6 hash characters at the start of the line,
        ''' corresponding to header levels 1-6. For example:
        '''
        '''     # This Is an H1
        '''
        '''     ## This Is an H2
        '''
        '''     ###### This Is an H6
        '''
        ''' Optionally, you may "close" atx-style headers. This Is purely
        ''' cosmetic -- you can use this if you think it looks better. The
        ''' closing hashes don't even need to match the number of hashes
        ''' used to open the header. (The number of opening hashes
        ''' determines the header level.) 
        '''
        '''     # This Is an H1 #
        ''' 
        '''     ## This Is an H2 ##
        '''
        '''     ### This Is an H3 ######
        ''' ]]>
        Private Function IsHeader(lines As String(), i As Integer) As Header
            Dim s As String = lines(i)
            Dim m As String = Regex.Match(s, "^#+\s", RegexICMul).Value

            If Not String.IsNullOrEmpty(m) Then
                Dim level As Integer = m.Count("#"c)
                If level > 6 Then
                    level = 6
                End If
                s = Regex.Replace(s, "^#+", "").Trim
                Return New Header With {
                    .Level = level,
                    .Text = s
                }
            Else
                Dim sNext As String = lines(i + 1)
                If Regex.Match(sNext, "^[+-]+$", RegexICMul).Success Then
                    Return New Header With {
                        .Level = 1,
                        .Text = s
                    }
                Else
                    Return Nothing
                End If
            End If
        End Function
    End Module
End Namespace