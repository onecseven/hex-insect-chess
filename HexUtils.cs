using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Sylves;

namespace HexDemo3
{

    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
        => self.Select((item, index) => (item, index));
    }

    public class HexUtils
    {
        static public List<Cell> HexGen(int col, int row, HexOrientation orientation)
        {
            var right = col;
            var left = 0;
            var bottom = row;
            var top = 0;
            List<Cell> cells = new List<Cell>();
            switch (orientation)
            {
                case HexOrientation.PointyTopped:
                    for (int r = top; r <= bottom; r++)
                    {
                        int r_offset = System.Convert.ToInt32(Math.Floor(r / 2.0));
                        for (int q = left - r_offset; q <= right - r_offset; q++)
                        {
                            cells.Add(new Cell(q, r, -q - r));
                        }
                    }
                    break;
                case HexOrientation.FlatTopped:
                    {
                        for (int q = left; q <= right; q++)
                        {
                            int q_offset = System.Convert.ToInt32(Math.Floor(q / 2.0));
                            for (int r = top - q_offset; r <= bottom - q_offset; r++)
                            {
                                cells.Add(new Cell(q, r, -q - r));
                            }
                        }
                        break;
                    }
            }
            return cells;
        }
        #region triangle stuff
        // https://www.geeksforgeeks.org/check-whether-a-given-point-lies-inside-a-triangle-or-not/
        static public double area(Vector2 A, Vector2 B, Vector2 C)
        {
            return Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0);
        }

        static public bool IsInside(Vector2 A, Vector2 B, Vector2 C, Vector2 X) {
            double Triangle = area(A,B,C);
            double XBC = area(X,B,C);
            double AXC = area(A,X,C);
            double ABX = area(A,B,X);
            return (Triangle == XBC + AXC + ABX);
        }

        static public Vector2[][] SliceHexIntoTriangles(Vector2 center, int radius, HexOrientation orientation)
        {

            Vector2[] HexCorners = HexCornersFromCenter(center, radius, orientation);
            Vector2[] ABP = new Vector2[3] { center, HexCorners[0], HexCorners[1] };
            Vector2[] BCP = new Vector2[3] { center, HexCorners[1], HexCorners[2] };
            Vector2[] CDP = new Vector2[3] { center, HexCorners[2], HexCorners[3] };
            Vector2[] DEP = new Vector2[3] { center, HexCorners[3], HexCorners[4] };
            Vector2[] EFP = new Vector2[3] { center, HexCorners[4], HexCorners[5] };
            Vector2[] AFP = new Vector2[3] { center, HexCorners[5], HexCorners[6] };

            // WE COULD DO ABC, ACF, CDF, AFE
            return new Vector2[6][] {ABP, BCP, CDP, DEP, EFP, AFP};
        }

        static public Vector2[][] SliceHexIntoTriangles(Vector3 center, int radius, HexOrientation orientation) => SliceHexIntoTriangles(new Godot.Vector2(center.X, center.Y), radius, orientation);
    
    #endregion
    static public Vector2[] HexCornersFromCenter(Vector3 origin, int radius, HexOrientation orientation)
        {
            Vector2[] result = new Vector2[7];
            if (orientation is HexOrientation.PointyTopped)
            {
                for (int a = 0; a < 6; a++)
                {
                    var y = origin.Y + radius * Math.Cos(a * 60 * Math.PI / 180f);
                    var x = origin.X + radius * Math.Sin(a * 60 * Math.PI / 180f);
                    result[a] = new Godot.Vector2(System.Convert.ToSingle(x), System.Convert.ToSingle(y));
                }

            } else {
                for (int a = 0; a < 6; a++) {
                    var y = origin.Y + radius * Math.Sin(a * 60 * Math.PI / 180f);
                    var x = origin.X + radius * Math.Cos(a * 60 * Math.PI / 180f);
                    result[a] = new Godot.Vector2(System.Convert.ToSingle(x), System.Convert.ToSingle(y));
                }
            }

            result[6] = result[0];
            return result;
        }
        static public Vector2[] HexCornersFromCenter(Vector2 origin, int radius, HexOrientation orientation) => HexCornersFromCenter(new Vector3(origin.X, origin.Y, 0), radius, orientation);
    }
}
