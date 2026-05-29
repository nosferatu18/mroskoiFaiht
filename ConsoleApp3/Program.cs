using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Ship
    {
        public string Type;
        public int Length;
        public int StartX;
        public int StartY;
        public bool IsHorizontal;
        public int Hits;

        public Ship(string type, int length, int startX, int startY, bool isHorizontal)
        {
            Type = type;
            Length = length;
            StartX = startX;
            StartY = startY;
            IsHorizontal = isHorizontal;
            Hits = 0;
        }

        public bool Hit()
        {
            Hits++;
            return Hits == Length;
        }

        public string GetShipType()
        {
            return Type;
        }

        public void DisplayStatus()
        {
            Console.WriteLine(Type + ": " + Hits + " из " + Length + " палуб повреждено");
        }
    }

    class GameField
    {
        public int Size = 10;
        private Ship[,] grid;
        private Ship[] ships;
        private int shipCount;

        public GameField()
        {
            grid = new Ship[Size, Size];
            ships = new Ship[10];
            shipCount = 0;
        }

        public bool AddShip(Ship ship)
        {
            if (ship.StartX < 0 || ship.StartX >= Size) return false;
            if (ship.StartY < 0 || ship.StartY >= Size) return false;

            if (ship.IsHorizontal && ship.StartY + ship.Length > Size) return false;
            if (!ship.IsHorizontal && ship.StartX + ship.Length > Size) return false;

            for (int i = 0; i < ship.Length; i++)
            {
                int x = ship.StartX;
                int y = ship.StartY;
                if (ship.IsHorizontal) y = ship.StartY + i;
                else x = ship.StartX + i;

                if (grid[x, y] != null) return false;
            }

            for (int i = 0; i < ship.Length; i++)
            {
                int x = ship.StartX;
                int y = ship.StartY;
                if (ship.IsHorizontal) y = ship.StartY + i;
                else x = ship.StartX + i;

                grid[x, y] = ship;
            }

            ships[shipCount] = ship;
            shipCount++;
            return true;
        }

        public bool ReceiveShot(int x, int y)
        {
            x--; y--;

            if (x < 0 || x >= Size || y < 0 || y >= Size)
            {
                Console.WriteLine("Неверные координаты!");
                return false;
            }

            if (grid[x, y] == null)
            {
                Console.WriteLine("Промах!");
                return false;
            }

            Ship ship = grid[x, y];
            bool destroyed = ship.Hit();

            Console.WriteLine("Попадание в " + ship.Type + "!");

            if (destroyed)
            {
                Console.WriteLine(ship.Type + " уничтожен!");
            }

            return true;
        }

        public bool AllShipsDestroyed()
        {
            for (int i = 0; i < shipCount; i++)
            {
                if (ships[i].Hits < ships[i].Length) return false;
            }
            return true;
        }

        public void PrintField(bool hideShips)
        {
            Console.Write("  ");
            for (int i = 1; i <= Size; i++)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            for (int i = 0; i < Size; i++)
            {
                Console.Write(i + 1 + " ");

                for (int j = 0; j < Size; j++)
                {
                    char c = '~';
                    Ship ship = grid[i, j];

                    if (ship != null)
                    {
                        bool isHit = false;
                        for (int k = 0; k < ship.Hits; k++)
                        {
                            int checkX = ship.StartX;
                            int checkY = ship.StartY;
                            if (ship.IsHorizontal) checkY = ship.StartY + k;
                            else checkX = ship.StartX + k;

                            if (checkX == i && checkY == j)
                            {
                                isHit = true;
                                break;
                            }
                        }

                        if (isHit) c = '#';
                        else if (!hideShips) c = 'S';
                        else c = '~';
                    }

                    Console.Write(c + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main()
        {
            GameField field = new GameField();

            field.AddShip(new Ship("Линкор", 4, 0, 0, true));
            field.AddShip(new Ship("Крейсер", 3, 2, 2, false));
            field.AddShip(new Ship("Эсминец", 2, 6, 0, true));
            field.AddShip(new Ship("Эсминец", 2, 1, 7, false));

            Console.WriteLine("Начальное поле:");
            field.PrintField(false);
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();

            int shots = 0;

            while (!field.AllShipsDestroyed())
            {
                Console.Clear();
                Console.WriteLine("МОРСКОЙ БОЙ\n");
                field.PrintField(true);

                Console.Write("Выстрел " + (shots + 1) + ". Введите X Y: ");
                string[] input = Console.ReadLine().Split(' ');

                int x, y;

                // try parse сам проверяет числа или нет
                if (input.Length != 2 || !int.TryParse(input[0], out x) || !int.TryParse(input[1], out y))
                {
                    Console.WriteLine("Ошибка! Введите два числа (например: 3 5)");
                    Console.ReadKey();
                    continue;
                }

                shots++;
                field.ReceiveShot(x, y);
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }

            Console.Clear();
            Console.WriteLine("ПОБЕДА! Все корабли уничтожены!");
            Console.WriteLine("Выстрелов сделано: " + shots);
            Console.ReadKey();
        }
    }
}
