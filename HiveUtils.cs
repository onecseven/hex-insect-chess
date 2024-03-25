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
    public static bool AreCellsAdjacent(Cell a, Cell B) => HiveUtils.getNeighbors(a).Contains(B);
    public static void PrettyPrint(Object obj)
    {
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
            string name = descriptor.Name;
            object value = descriptor.GetValue(obj);
            GD.Print(name, value);
            }
    }
    public static void GetPropertiesNameOfClass(string name, object pObject)
    {
        List<string> propertyList = new List<string>();
        GD.Print("===============");
        GD.Print(name);
        if (pObject != null)
        {
            foreach (var prop in pObject.GetType().GetProperties())
            {
                GD.Print(prop.Name, prop.GetValue(pObject).ToString());
            }
        }
        GD.Print("================");
    }
    public static void Unroll<T>(string title, IEnumerable<T> obj)
    {
        GD.Print(title);
        foreach (T item in obj)
        {
            GD.Print(item);
        }
        GD.Print("===");
    }

    public static Cell[] directions = new Cell[6] {
            new Cell(+1, 0, -1), new Cell(+1, -1, 0), new Cell(0, -1, +1),
            new Cell(-1, 0, +1), new Cell(-1, +1, 0), new Cell(0, +1, -1),
        };
    public static List<Cell> getNeighbors(Cell cell) => directions.Select(direction => new Cell(cell.x + direction.x, cell.y + direction.y, cell.z + direction.z)).ToList();
    public static Vector2 Vector3ToVector2 (Vector3 vector) => new Vector2(vector.X, vector.Y);

    public static Vector3 cel2vec(Cell cell) => new Vector3(cell.x,cell.y,cell.z);
    public static Cell vec2cel(Godot.Vector3 vector) => new Cell(System.Convert.ToInt32(vector.X), System.Convert.ToInt32(vector.Y), System.Convert.ToInt32(vector.Z));
    }
