using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Tetrominoe
    {
        public static int[,] I = new int[1, 4] { { 1, 1, 1, 1 } };    //полоса
        public static int[,] O = new int[2, 2] { { 1, 1 }, { 1, 1 } };   //квадрат
        public static int[,] T = new int[2, 3] { { 0, 1, 0 }, { 1, 1, 1 } };   //T-образная
        public static int[,] S = new int[2, 3] { { 0, 1, 1 }, { 1, 1, 0 } };   //S-образная
        public static int[,] Z = new int[2, 3] { { 1, 1, 0 }, { 0, 1, 1 } };   //Z-образная
        public static int[,] J = new int[2, 3] { { 1, 0, 0 }, { 1, 1, 1 } };   //J-образная
        public static int[,] L = new int[2, 3] { { 0, 0, 1 }, { 1, 1, 1 } };   //L-образная

        public static List<int[,]> tetrominoes = new List<int[,]>() { I, O, T, S, Z, J, L };

        private bool isErect = false;
        private int[,] shape;
        private int[] pix = new int[2];
        public List<int[]> location = new List<int[]>();

        public const int HEIGHT_FIELD = 23;
        public const int WIDTH_FIELD = 10;
       

        public Tetrominoe()
        {
            Random rnd = new Random();

            shape = tetrominoes[rnd.Next(0, 7)];

            for (int i = HEIGHT_FIELD; i < 33; ++i)
            {
                for (int j = 3; j < WIDTH_FIELD; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("  ");
                }

            }

            GameBL.drawBorder();

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        Console.SetCursorPosition(((WIDTH_FIELD - shape.GetLength(1)) / 2 + j) * 2 + 20, i + 5);
                        Console.Write(GameBL.sqr);
                    }
                }
            }
        }

        public void Spawn()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        location.Add(new int[] { i, (WIDTH_FIELD - shape.GetLength(1)) / 2 + j });
                    }
                }
            }

            Update();
        }

        public void Drop()
        {

            if (IsSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] = 1;
                }

                GameBL.isDropped = true;


            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    location[numCount][0] += 1;
                }

                Update();
            }
        }

        public void Rotate()
        {
            List<int[]> templocation = new List<int[]>();

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (WIDTH_FIELD - shape.GetLength(1)) / 2 + j });
                    }
                }
            }

            if (shape == tetrominoes[0])
            {
                if (isErect == false)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                    }
                }
                else
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Counterclockwise");
                    }
                }
            }

            if (shape == tetrominoes[3])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[3], "Clockwise");
                }
            }

            if (shape == tetrominoes[1])
            {
                return;
            }
            else
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                }
            }


            for (int count = 0; IsOverlayLeft(templocation) != false | IsOverlayRight(templocation) != false | IsOverlayBelow(templocation) != false; count++)
            {
                if (IsOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }

                if (IsOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }
                if (IsOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }
                if (count == 3)
                {
                    return;

                }
            }

            location = templocation;

        }

        public int[] TransformMatrix(int[] coord, int[] axis, string dir)
        {
            int[] pcoord = { coord[0] - axis[0], coord[1] - axis[1] };

            if (dir == "Counterclockwise")
            {
                pcoord = new int[] { -pcoord[1], pcoord[0] };
            }
            if (dir == "Clockwise")
            {
                pcoord = new int[] { pcoord[1], -pcoord[0] };
            }

            return new int[] { pcoord[0] + axis[0], pcoord[1] + axis[1] };
        }

        public bool IsSomethingBelow()
        {
            bool result = false;

            for (int i = 0; i < 4; i++)
            {
                if (location[i][0] + 1 >= HEIGHT_FIELD)
                {
                    result = true;
                    break;
                }
                if (location[i][0] + 1 < HEIGHT_FIELD)
                {
                    if (GameBL.droppedtetrominoeLocationGrid[location[i][0] + 1, location[i][1]] == 1)
                    {
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result ;
        }
        public bool IsOverlayBelow(List<int[]> location)
        {

            bool result = false;

            List<int> ycoords = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                ycoords.Add(location[i][0]);
                if (location[i][0] >= HEIGHT_FIELD)
                {

                    result = true;
                    break;
                }

                if (location[i][0] < 0)
                {
                    result = false;
                    break;

                }

                if (location[i][1] < 0)
                {

                    result = false;
                    break;
                }

                if (location[i][1] > (WIDTH_FIELD - 1))
                {

                    result = false;
                    break;

                }
            }

            int yMax = ycoords.Max();
            int yMin = ycoords.Min();

            for (int i = 0; i < 4; i++)
            {
                if (yMax - yMin == 3)
                {
                    if (yMax == location[i][0] | yMax - 1 == location[i][0])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                            break;

                        }
                    }
                }
                else
                {
                    if (yMax == location[i][0])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result ;
        }


        public bool IsSomethingLeft()
        {
            bool result = false;

            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 0)
                {
                    result = true;
                    break;
                }
                else
                {
                    if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1] - 1] == 1)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        public bool IsOverlayLeft(List<int[]> location)
        {
            bool result = false;

            List<int> xcoords = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);

                if (location[i][1] < 0)
                {
                     result = true; 
                }
                if (location[i][1] > (WIDTH_FIELD - 1))
                {
                     result = false; 
                }
                if (location[i][0] >= HEIGHT_FIELD)
                {
                    result = false;
                }
                if (location[i][0] < 0)
                {
                    result = false;
                }
            }

            int xMax = xcoords.Max();
            int xMin = xcoords.Min();

            for (int i = 0; i < 4; i++)
            {
                if (xMax - xMin == 3)
                {
                    if (xMin == location[i][1] | xMin + 1 == location[i][1])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                        }
                    }

                }
                else
                {
                    if (xMin == location[i][1])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result ;
        }
        public bool IsSomethingRight()
        {
            bool result = false;

            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == (WIDTH_FIELD - 1))
                {
                    result = true;
                    break;
                }
                else if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1] + 1] == 1)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool IsOverlayRight(List<int[]> location)
        {
            bool result = false;

            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > (WIDTH_FIELD-1))
                {
                    {
                        result = true;
                        break;
                    }
                }
                if (location[i][1] < 0)
                {
                    {
                        result = false;
                        break;
                    }
                }
                if (location[i][0] >= HEIGHT_FIELD)
                {
                    result = false;
                    break;
                }
                if (location[i][0] < 0)
                {
                    result = false;
                    break;
                }
            }
            int xMax = xcoords.Max();
            int xMin = xcoords.Min();

            for (int i = 0; i < 4; i++)
            {
                if (xMax - xMin == 3)
                {
                    if (xMax == location[i][1] | xMax - 1 == location[i][1])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                            break;
                        }
                    }

                }
                else
                {
                    if (xMax == location[i][1])
                    {
                        if (GameBL.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result ;
        }
        public void Update()
        {
            for (int i = 0; i < HEIGHT_FIELD; i++)
            {
                for (int j = 0; j < WIDTH_FIELD; j++)
                {
                    GameBL.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                GameBL.grid[location[i][0], location[i][1]] = 1;
            }
            GameBL.Draw();
        }
    }
}
