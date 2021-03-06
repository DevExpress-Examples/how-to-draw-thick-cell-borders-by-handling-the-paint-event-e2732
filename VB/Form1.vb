Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Utils.Drawing

Namespace WindowsApplication1
	Partial Public Class Form1
		Inherits Form

				Private Function CreateTable(ByVal RowCount As Integer) As DataTable
			Dim tbl As New DataTable()
			tbl.Columns.Add("Name", GetType(String))
			tbl.Columns.Add("ID", GetType(Integer))
			tbl.Columns.Add("Number", GetType(Integer))
			tbl.Columns.Add("Date", GetType(Date))
			For i As Integer = 0 To RowCount - 1
				tbl.Rows.Add(New Object() { String.Format("Name{0}", i), i, 3 - i, Date.Now.AddDays(i) })
			Next i
			Return tbl
				End Function


		Public Sub New()
			InitializeComponent()
			gridControl1.DataSource = CreateTable(20)
			Dim TempHotTrackCellHelper As HotTrackCellHelper = New HotTrackCellHelper(gridView1)
		End Sub
	End Class


	Public Class HotTrackCellHelper

		Public Sub New(ByVal view As GridView)
			_View = view
			AddHandler view.GridControl.PaintEx, AddressOf GridControl_PaintEx
			AddHandler view.MouseMove, AddressOf view_MouseMove
		End Sub

		Private Sub GridControl_PaintEx(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.PaintExEventArgs)
			DrawHotTrackedCell(e.Cache)
		End Sub

		Private _BorderWidth As Integer = 4
		Private _HotTrackedCell As GridCell
		Private ReadOnly _View As GridView
		Public Property HotTrackedCell() As GridCell
			Get
				Return _HotTrackedCell
			End Get
			Set(ByVal value As GridCell)
				RefreshCell(_HotTrackedCell)
				_HotTrackedCell = value
			End Set
		End Property

		Public Function GetCellBounds(ByVal cell As GridCell) As Rectangle
			If cell Is Nothing Then
				Return Rectangle.Empty
			End If
			Dim info As GridViewInfo = TryCast(_View.GetViewInfo(), GridViewInfo)
			Dim cellInfo As GridCellInfo = info.GetGridCellInfo(cell.RowHandle, cell.Column)
			Return cellInfo.Bounds
		End Function


		Private Sub UpdateHotTrackedCell(ByVal location As Point)
			Dim hi As GridHitInfo = _View.CalcHitInfo(location)
			If hi.HitTest = GridHitTest.Row OrElse hi.HitTest = GridHitTest.RowEdge Then
				Return
			End If
			If hi.InRowCell Then
				If _View.IsRowVisible(hi.RowHandle) = RowVisibleState.Visible Then
					HotTrackedCell = New GridCell(hi.RowHandle, hi.Column)
					Return
				End If
			End If
			HotTrackedCell = Nothing
		End Sub


		Private Sub view_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			UpdateHotTrackedCell(e.Location)
		End Sub


		Private Sub RefreshCell(ByVal cell As GridCell)
			If cell Is Nothing Then
				Return
			End If
			Dim rect As Rectangle = GetCellBounds(cell)
			rect.Inflate(_BorderWidth, _BorderWidth)
			_View.InvalidateRect(rect)
		End Sub

		Private Sub DrawHotTrackedCell(ByVal cache As GraphicsCache)
			Dim bounds As Rectangle = GetCellBounds(HotTrackedCell)
			cache.DrawRectangle(New Pen(Brushes.Black, _BorderWidth), bounds)
		End Sub
	End Class
End Namespace