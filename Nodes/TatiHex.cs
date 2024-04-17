using Godot;
using HexDemo3;
using Sylves;
using System.Collections.Generic;

public partial class TatiHex : Node2D
{
    public readonly Cell nullCell = new Cell(-12, -12, -12);

    [Export]
    GameWrapper gameWrapper = null;
    [Export]
    Font defaultFont = ThemeDB.FallbackFont;
    [Export]
    int defaultFontSize = ThemeDB.FallbackFontSize;
    [Export]
    public Color ColorBase { get; set; } = Colors.Red;
    [Export]
    public bool DrawGrid = true;
    [Export]
    public int Hexsize
    {
        get => _hexsize; set
        {
            _hexsize = value;
            updateGrid();
        }
    }
    [Export]
    public int Rows
    {
        get => _rows; set
        {
            _rows = value;
            updateGrid();
        }
    }
    [Export]
    public int Cols
    {
        get => _cols; set
        {
            _cols = value;
            updateGrid();
        }
    }
    [Export]
    public HexOrientation orientation { get; set; }

    public Dictionary<Cell, Vector3> hexes = new Dictionary<Cell, Vector3>();

    public int _hexsize = 25;
    public int _rows = 36;
    public int _cols = 24;
    public override void _Ready()
    {
        updateGrid();
    }
    public bool isPointInCell(Vector2 point, Cell cell)
    {
        Vector3 center = gameWrapper.machine.board.grid.GetCellCenter(cell);
        Vector2[][] triangles = HexUtils.SliceHexIntoTriangles(new Godot.Vector2(center.X, center.Y), _hexsize / 2, orientation);
        foreach (Vector2[] triangle in triangles)
        {
            if (HexUtils.IsInside(triangle[0], triangle[1], triangle[2], point)) return true;
        }
        return false;
    }
    public Vector2[] GetHexCornersFromCenter(Vector2 center) => HexUtils.HexCornersFromCenter(center, _hexsize / 2, orientation);
    public Vector2[] GetHexCornersFromCenter(Vector3 center) => GetHexCornersFromCenter(new Vector2(center.X, center.Y));

    Sylves.HexGrid grid = new Sylves.HexGrid(25, HexOrientation.PointyTopped);

    public void updateGrid()
    {
        grid = new Sylves.HexGrid(_hexsize, orientation);
        var cells = HexUtils.HexGen(Rows, Cols, orientation);
        hexes.Clear();
        foreach (Cell cell in cells)
        {
            hexes.TryAdd(cell, grid.GetCellCenter(cell));
        }
        QueueRedraw();
    }
    public override void _Draw()
    {
        if (!DrawGrid) return;
        foreach ((Cell cell, Vector3 center) in hexes)
        {
            Vector2 fontCenter = new Vector2(center.X - 14, center.Y);
            var super_shapes = HexUtils.HexCornersFromCenter(center, _hexsize/2, orientation);
            for (int i = 0; i < super_shapes.Length; i++)
            {
                DrawString(defaultFont, fontCenter, $"({cell.x}, {cell.y})", HorizontalAlignment.Left, -1, defaultFontSize);
                int next = i + 1 > super_shapes.Length - 1 ? 0 : i + 1;
                DrawLine(super_shapes[i], super_shapes[next], ColorBase, 1);
            }
        }
    }

    public Cell mouseToCell(InputEventMouse ev)
    {
        Vector2 currentPosition = ((InputEventMouse)ev).GlobalPosition - this.Position;
        var lic = FindCell(new Vector2(currentPosition.X, currentPosition.Y));
        if (lic != null) return (Cell)lic; 
        else return nullCell;
    }

    public Cell? FindCell(Vector2 loc)
    {
        if (grid.FindCell(new Vector3(loc.X, loc.Y, 0), out Cell lic))
        {
            return lic;
        } else
        {
            return null;
        }
    }
}
