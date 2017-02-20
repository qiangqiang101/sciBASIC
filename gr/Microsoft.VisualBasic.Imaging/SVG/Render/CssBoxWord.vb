Imports System.Collections.Generic
Imports System.Text

''' <summary>
''' Represents a word inside an inline box
''' </summary>
''' <remarks>
''' Because of performance, words of text are the most atomic 
''' element in the project. It should be characters, but come on,
''' imagine the performance when drawing char by char on the device.
''' 
''' It may change for future versions of the library
''' </remarks>
Friend Class CssBoxWord
	Inherits CssRectangle
	#Region "Fields"


	Private _word As String
	Private _lastMeasureOffset As PointF
	Private _ownerBox As CssBox
	Private _spacesAfter As Integer
	Private _breakAfter As Boolean
	Private _spacesBefore As Integer
	Private _breakBefore As Boolean
	Private _image As Image


	#End Region

	#Region "Ctor"

	Friend Sub New(owner As CssBox)
		_ownerBox = owner
		_word = String.Empty
	End Sub

	''' <summary>
	''' Creates a new BoxWord which represents an image
	''' </summary>
	''' <param name="owner"></param>
	''' <param name="image"></param>
	Public Sub New(owner As CssBox, image__1 As Image)
		Me.New(owner)
		Image = image__1
	End Sub

	#End Region

	#Region "Properties"

	''' <summary>
	''' Gets the width of the word including white-spaces
	''' </summary>
	Public ReadOnly Property FullWidth() As Single
		'get { return OwnerBox.ActualWordSpacing * (SpacesBefore + SpacesAfter) + Width; }
		Get
			Return Width
		End Get
	End Property

	''' <summary>
	''' Gets the image this words represents (if one)
	''' </summary>
	Public Property Image() As Image
		Get
			Return _image
		End Get
		Set
			_image = value

			If value IsNot Nothing Then
				Dim w As New CssLength(OwnerBox.Width)
				Dim h As New CssLength(OwnerBox.Height)
				If w.Number > 0 AndAlso w.Unit = CssLength.CssUnit.Pixels Then
					Width = w.Number
				Else
					Width = value.Width
				End If

				If h.Number > 0 AndAlso h.Unit = CssLength.CssUnit.Pixels Then

					Height = h.Number
				Else
					Height = value.Height
				End If


				Height += OwnerBox.ActualBorderBottomWidth + OwnerBox.ActualBorderTopWidth + OwnerBox.ActualPaddingTop + OwnerBox.ActualPaddingBottom
			End If
		End Set
	End Property

	''' <summary>
	''' Gets if the word represents an image.
	''' </summary>
	Public ReadOnly Property IsImage() As Boolean
		Get
			Return Image IsNot Nothing
		End Get
	End Property

	''' <summary>
	''' Gets a bool indicating if this word is composed only by spaces.
	''' Spaces include tabs and line breaks
	''' </summary>
	Public ReadOnly Property IsSpaces() As Boolean
		Get
			Return String.IsNullOrEmpty(Text.Trim())
		End Get
	End Property

	''' <summary>
	''' Gets if the word is composed by only a line break
	''' </summary>
	Public ReadOnly Property IsLineBreak() As Boolean
		Get
			Return Text = vbLf
		End Get
	End Property

	''' <summary>
	''' Gets if the word is composed by only a tab
	''' </summary>
	Public ReadOnly Property IsTab() As Boolean
		Get
			Return Text = vbTab
		End Get
	End Property

	''' <summary>
	''' Gets the Box where this word belongs.
	''' </summary>
	Public ReadOnly Property OwnerBox() As CssBox
		Get
			Return _ownerBox
		End Get
	End Property

	''' <summary>
	''' Gets the text of the word
	''' </summary>
	Public ReadOnly Property Text() As String
		Get
			Return _word
		End Get
	End Property

	''' <summary>
	''' Gets or sets an offset to be considered in measurements
	''' </summary>
	Friend Property LastMeasureOffset() As PointF
		Get
			Return _lastMeasureOffset
		End Get
		Set
			_lastMeasureOffset = value
		End Set
	End Property

	#End Region

	#Region "Methods"

	''' <summary>
	''' Removes line breaks and tabs on the text of the word,
	''' replacing them with white spaces
	''' </summary>
	Friend Sub ReplaceLineBreaksAndTabs()
		_word = _word.Replace(ControlChars.Lf, " "C)
		_word = _word.Replace(ControlChars.Tab, " "C)
	End Sub

	''' <summary>
	''' Appends the specified char to the word's text
	''' </summary>
	''' <param name="c"></param>
	Friend Sub AppendChar(c As Char)
		_word += c
	End Sub

	''' <summary>
	''' Represents this word for debugging purposes
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String

		Return String.Format("{0} ({1} char{2})", Text.Replace(" "C, "-"C).Replace(vbLf, "\n"), Text.Length, If(Text.Length <> 1, "s", String.Empty))
	End Function

	#End Region
End Class
