﻿#Region "Microsoft.VisualBasic::a3fbc603226f39a59953581345b9fa2b, ..\sciBASIC#\Data_science\Graph\API\PageRank\PageRank.vb"

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

Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Analysis.PageRank

    ''' <summary>
    ''' https://github.com/jeffersonhwang/pagerank
    ''' </summary>
    Public Class PageRank

#Region "Private Fields"

        Dim _incomingLinks As List(Of Integer)(), _leafNodes As List(Of Integer)
        Dim _numLinks As Vector
        Dim _alpha As Double, _convergence As Double
        Dim _checkSteps As Integer

#End Region

#Region "Constructor"

        ''' <summary>
        ''' ``outGoingLinks(i)`` contains the indices of the pages pointed to by page i.
        ''' (每一行都是指向第i行的页面的index值的集合)
        ''' </summary>
        ''' <param name="linkMatrix"><see cref="GraphMatrix"/></param>
        ''' <param name="alpha"></param>
        ''' <param name="convergence"></param>
        ''' <param name="checkSteps"></param>
        Public Sub New(linkMatrix As List(Of Integer)(), Optional alpha# = 0.85, Optional convergence# = 0.0001, Optional checkSteps% = 10)
            With TransposeLinkMatrix(linkMatrix)
                _incomingLinks = .incomingLinks
                _numLinks = .numLinks
                _leafNodes = .leafNodes
            End With

            _alpha = alpha
            _convergence = convergence
            _checkSteps = checkSteps
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Convenience wrap for the link matrix transpose and the generator.
        ''' See <see cref="PageRankGenerator"/> method for parameter descriptions
        ''' </summary>
        Public Function ComputePageRank() As Vector
            Dim final As Vector = Nothing

            For Each generator As Vector In PageRankGenerator(
                _incomingLinks,
                _numLinks,
                _leafNodes,
                _alpha,
                _convergence,
                _checkSteps)

                final = generator
            Next

            Return final
        End Function

        ''' <summary>
        ''' Transposes the link matrix which contains the links from each page. 
        ''' Returns a Tuple of:  
        ''' 
        ''' + 1) pages pointing to a given page, 
        ''' + 2) how many links each page contains, and
        ''' + 3) which pages contain no links at all. 
        ''' 
        ''' We want to know is which pages
        ''' </summary>
        ''' <param name="outGoingLinks">``outGoingLinks(i)`` contains the indices of the pages pointed to by page i</param>
        ''' <returns>A tuple of (incomingLinks, numOutGoingLinks, leafNodes)</returns>
        Protected Function TransposeLinkMatrix(outGoingLinks As List(Of Integer)()) As (incomingLinks As List(Of Integer)(), numLinks As Vector, leafNodes As List(Of Integer))
            Dim nPages As Integer = outGoingLinks.Length

            ' incomingLinks[i] will contain the indices jj of the pages
            ' linking to page i
            Dim incomingLinks As New List(Of List(Of Integer))(nPages)

            For i As Integer = 0 To nPages - 1
                incomingLinks.Add(New List(Of Integer)())
            Next

            ' the number of links in each page
            Dim numLinks As Vector = New Vector(nPages)
            ' the indices of the leaf nodes
            Dim leafNodes As New List(Of Integer)

            For i As Integer = 0 To nPages - 1
                Dim values As List(Of Integer) = outGoingLinks(i)

                If values.Count = 0 Then
                    leafNodes.Add(i)
                Else
                    numLinks(i) = values.Count
                    ' transpose the link matrix
                    For Each j As Integer In values
                        Dim list As List(Of Integer) = incomingLinks(j)
                        list.Add(i)
                        incomingLinks(j) = list
                    Next
                End If
            Next

            Return (incomingLinks.ToArray, numLinks, leafNodes)
        End Function

        ''' <summary>
        ''' Computes an approximate page rank vector of N pages to within some convergence factor.
        ''' </summary>
        ''' <param name="at">At a sparse square matrix with N rows. At[i] contains the indices of pages jj linking to i</param>
        ''' <param name="leafNodes">contains the indices of pages without links</param>
        ''' <param name="numLinks">iNumLinks[i] is the number of links going out from i.</param>
        ''' <param name="alpha">a value between 0 and 1. Determines the relative importance of "stochastic" links.</param>
        ''' <param name="convergence">a relative convergence criterion. Smaller means better, but more expensive.</param>
        ''' <param name="checkSteps">check for convergence after so many steps</param>
        Protected Iterator Function PageRankGenerator(at As List(Of Integer)(), numLinks As Vector, leafNodes As List(Of Integer), alpha#, convergence#, checkSteps%) As IEnumerable(Of Vector)
            Dim N As Integer = at.Length
            Dim M As Integer = leafNodes.Count

            Dim iNew As Vector = Vector.Ones(N) / N
            Dim iOld As Vector = Vector.Ones(N) / N

            Dim done As Boolean = False

            While Not done
                ' normalize every now and then for numerical stability
                iNew /= iNew.Sum

                For i As Integer = 0 To checkSteps - 1
                    ' swap arrays
                    Call iOld.SwapWith(iNew)

                    ' an element in the 1 x I vector. 
                    ' all elements are identical.
                    Dim oneIv As Double = (1 - alpha) * iOld.Sum / N

                    ' an element of the A x I vector.
                    ' all elements are identical.
                    Dim oneAv As Double = 0.0
                    If M > 0 Then
                        oneAv = alpha * iOld.Take(leafNodes).Sum / N
                    End If

                    ' the elements of the H x I multiplication
                    For j As Integer = 0 To N - 1
                        Dim page As List(Of Integer) = at(j)
                        Dim h As Double = 0

                        If page.Count > 0 Then
                            ' .DotProduct
                            h = alpha * iOld.Take(page).DotProduct(1.0 / numLinks.Take(page))
                        End If

                        iNew(j) = h + oneAv + oneIv
                    Next
                Next

                Dim diff As Vector = iNew - iOld

                Yield iNew

                done = diff.SumMagnitude < convergence
            End While
        End Function
#End Region
    End Class
End Namespace
