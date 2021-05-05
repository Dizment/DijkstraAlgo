using System;
using System.IO;

namespace Rover
{
    public class Program
    {
        private static int row;
        private static int col;
        private static int fuel;

        public static void CalculateRoverPath(int[,] map)
        {
            row = map.GetLength(0);
            col = map.GetLength(1);
            int[,] matrixAdjacency = MatrixAdjacency(map); // получение матрицы смежности
            int[] path = DijkstraAlgo(matrixAdjacency); // получение кратчайшего пути, проложенным по номерам вершин
            int count = 1; // количество вершин, которые мы перебрали
            int node = 0; // количество пройденных узлов, шагов, пройденных ровером
            string str = "";

            while (path[path.Length - 2] != -1) // цикл получает координаты вершин в map
            {
                for (int i = 0; i < row; i++)
                {
                    for (int k = 0; k < col; k++)
                    {
                        if (count == path[node] && count != path[path.Length - 1])
                        {
                            str = str + $"[{i}][{k}]->";
                            path[node] = -1;
                            node++;
                        }

                        count++;
                    }
                }

                count = 1;
            }
           
            str = str + $"[{row - 1}][{col - 1}]";
            WritingToFile(str);                         // запись в файл
            WritingToFile($"steps: {node}");
            WritingToFile($"fuel: {fuel}");
        }

        public static void WritingToFile(string text) // метод записи в файл
        {
            string PathToFile = @"C:\Users\Терминатор\source\repos\Rover\Rover.txt";
            using (StreamWriter file = new StreamWriter(PathToFile, true, System.Text.Encoding.Default))
            {
                file.WriteLine(text);
            }
        }

        public static int[] GetEasyPath(int[] parents, int lenght)
        {
            int[] temp = new int[lenght];
            int j = 0;

            for (int i = lenght - 1; i != -1; i = parents[i]) // получение кратчайшего пути
            {
                temp[j] = i + 1;
                j++;
            }

            int[] easyPath = new int[0];

            for (int i = 0; i < temp.Length; i++) // преобразование в массив без нулей
            {
                if (temp[i] == 0)
                {
                    easyPath = temp[..i];
                    break;
                }
            }

            for (int i = 0; i < easyPath.Length / 2; i++) // реверс массива
            {
                int tmp = easyPath[i];
                easyPath[i] = easyPath[easyPath.Length - i - 1];
                easyPath[easyPath.Length - i - 1] = tmp;
            }

            return easyPath;
        }

        public static int[] DijkstraAlgo(int[,] matrix) // в параметрах алгоритма "matrix" - матрица смежности
        {
            int lenght = matrix.GetLength(0);
            int big_mun = int.MaxValue;
            int[] pos = new int[lenght]; // содержит длину от старта до определенной вершины
            bool[] node = new bool[lenght]; // указывает на пройденные(true)/непройденные(false) вершины
            int[] parents = new int[lenght]; // содержит историю пути от левой верхней вершины до правой нижней
            int min;
            int index_min = 0;

            for (int i = 0; i < lenght; i++)
            {
                parents[i] = -1;
            }

            for (int i = 0; i < lenght; i++)
            {
                pos[i] = big_mun; // инициализируем длинну, как бесконечность, потому что изначально длина неизвестна
                node[i] = false; // инициализуруем все вершины, как непройденные
            }

            pos[0] = 0; // длина от старта до старта равно нулю

            for (int i = 0; i < lenght - 1; i++)
            {
                min = big_mun; // инициализация неизвестного расстояния до вершины
                for (int j = 0; j < lenght; j++) // находим вершину с минимальным к ней расстоянием
                {
                    if (!node[j] && pos[j] < min)
                    {
                        min = pos[j];
                        index_min = j;
                    }
                }

                node[index_min] = true;

                for (int j = 0; j < lenght; j++)
                {
                    if (!node[j] && matrix[index_min, j] > 0 && pos[index_min] != big_mun && pos[index_min] + matrix[index_min, j] < pos[j]) // если вершина считается непройденной и смежна с выбранной 
                    {                                                                                                                        // и сумма веса выбранной вершины и ребра к текущей будет меньше, чем вес текущей на данный момент,
                        pos[j] = pos[index_min] + matrix[index_min, j];                                                                      // то меняем значени веса текущей вершины
                        parents[j] = index_min; //  запоминаем предков вершины
                    }
                }
            }

            fuel = pos[lenght - 1]; // топливо затраченное, чтобы достичь последней вершины (правая нижния вершина карты)

            return GetEasyPath(parents, lenght);
        }

        public static int abs(int digit) // получение модуля от числа
        {
            return digit > 0 ? digit : digit * (-1);
        }

        public static int[,] MatrixAdjacency(int[,] matrix) // перевод карты в матрицу смежности
        {
            row = matrix.GetLength(0);
            col = matrix.GetLength(1);

            int[,] mas = new int[row * col, row * col]; // матрица смежности

            int k = 0;

            for (int i = 0; i < row; i++) // цикл перебора карты
            {
                for (int j = 0; j < col; j++)
                {
                    if (i == 0 && j == 0) // нахождение связей у углов карты (2 связи)
                    {
                        mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1;
                        mas[k, col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (i == row - 1 && j == col - 1)
                    {
                        mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1;
                        mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (i == 0 && j == col - 1)
                    {
                        mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1; 
                        mas[k, k + col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (i == row - 1 && j == 0)
                    {
                        mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                        mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }

                    if (i == 0) // нахождение связей у границ карты (3 связи)
                    {
                        mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1;
                        mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1;
                        mas[k, k + col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (j == 0)
                    {
                        mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                        mas[k, k + col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                        mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (i == row - 1)
                    {
                        mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1;
                        mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1;
                        mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }
                    else if (j == col - 1)
                    {
                        mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                        mas[k, k + col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                        mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1;
                        k++;
                        continue;
                    }

                    mas[k, k + 1] = abs(matrix[i, j + 1] - matrix[i, j]) + 1; // нахождение остальных связей у элементов матриц (4 связи)
                    mas[k, k - 1] = abs(matrix[i, j - 1] - matrix[i, j]) + 1;
                    mas[k, k - col] = abs(matrix[i - 1, j] - matrix[i, j]) + 1;
                    mas[k, k + col] = abs(matrix[i + 1, j] - matrix[i, j]) + 1;
                    k++;
                }
            }

            return mas;
        }

        static void Main(string[] args)
        {
            int[,] map = {
            { 99, 0,  99, 99, 99},
            { 99, 0,  99,  0, 99},
            { 99, 0,  99,  0, 99},
            { 99, 99, 99,  0, 99},
            };

            CalculateRoverPath(map);
        }
    }
}
