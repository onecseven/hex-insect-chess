using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HiveUtils
{
    public static void PrettyPrint(Object obj)
    {
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
            string name = descriptor.Name;
            object value = descriptor.GetValue(obj);
            GD.Print(name, value);
            }
    }
    public static (int, int, int)[] directions = new (int, int, int)[6] {
            (+1, 0, -1), (+1, -1, 0), (0, -1, +1),
            (-1, 0, +1), (-1, +1, 0), (0, +1, -1),
        };
    public static List<Cell> getNeighbors(Cell cell) => directions.Select(direction => new Cell(cell.x + direction.Item1, cell.y + direction.Item2, cell.z + direction.Item3)).ToList();
    public static Vector2 Vector3ToVector2 (Vector3 vector) => new Vector2(vector.X, vector.Y);

    }
