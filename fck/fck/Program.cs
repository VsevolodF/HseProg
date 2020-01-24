using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fck
{
    // Создание делегата.
    delegate double MathOperation(double a, double b);

    class Program
    {
        // Задание словаря с математическими операциями.
        static Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation>
        {
            { "+", (x, y) => x + y},
            { "-", (x, y) => x - y},
            { "*", (x, y) => x * y},
            { "/", (x, y) => x / y},
            { "^", (x, y) => Math.Pow(x, y)},
        };

        /// <summary>
        /// Метод, вычисляющий заданное строкой выражение.
        /// </summary>
        /// <param name="expr"> Выражение. </param>
        /// <returns> Возвращает результат выражения. </returns>
        public static double Calculate(string expr)
        {
            // Разбиение выражения на два аргумента и операцию.
            string[] expression = expr.Split();

            // Возврат результата.
            return operations[expression[1]](double.Parse(expression[0]), double.Parse(expression[2]));
        }

        /// <summary>
        /// Метод, считывающий данные из файла.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        /// <returns> Возвращает массив строк из файла. </returns>
        public static string[] FileRead(string path)
        {
            string[] res = { };

            try
            {
                // Считывание из файла.
                res = File.ReadAllLines(path);
            }
            // Улавливание возможных ошибок.
            catch (IOException e)
            {
                Console.WriteLine("IO Exception: ", e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unauthorized Access Exception: ", e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Security Exception: ", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ", e.Message);
            }

            return res;
        }
        /// <summary>
        /// Метод, записывающий данные в файл.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        public static void FileWrite(string path, string data)
        {
            try
            {
                // Запись в файл построчно.
                File.AppendAllText(path, data);
            }
            // Улавливание возможных ошибок.
            catch (IOException e)
            {
                Console.WriteLine("IO Exception: ", e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unauthorized Access Exception: ", e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Security Exception: ", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ", e.Message);
            }
        }
        /// <summary>
        /// Метод, удаляющий файл.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        public static void FileDelete(string path)
        {
            try
            {
                // Удаление файла, если он существует.
                if (File.Exists(path))
                    File.Delete(path);
            }
            // Улавливание возможных ошибок.
            catch (IOException e)
            {
                Console.WriteLine("IO Exception: ", e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unauthorized Access Exception: ", e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Security Exception: ", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ", e.Message);
            }
        }

        /// <summary>
        /// Метод, проверяющий совпадение посчитанных результатов с данными.
        /// </summary>
        /// <param name="dataPath"> Путь к файлу с вычисленными результатами. </param>
        /// <param name="ansPath"> Путь к файлу с данными результатами. </param>
        public static void ExpressionsChecker(string dataPath, string ansPath)
        {
            // Удаление старого отчета, если он существует.
            FileDelete("results.txt");

            // Счетчик несошедшихся ответов.
            int wrongAns = 0;

            // Получение данных из обоих файлов.
            string[] data = FileRead(dataPath);
            string[] answers = FileRead(ansPath);

            // Если кол-во строк в файлах не совпадает, то дозабить меньший файл пустыми строками.
            if (data.Length != answers.Length)
            {
                if (data.Length > answers.Length)
                    Array.Resize(ref answers, data.Length);
                else
                    Array.Resize(ref data, answers.Length);
            }

            // Сверка результатов.
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == answers[i])
                    FileWrite("results.txt", "OK" + Environment.NewLine);
                else
                {
                    wrongAns++;
                    FileWrite("results.txt", "Error" + Environment.NewLine);
                }
            }
        }
        static void Main(string[] args)
        {
            // Считывание выражений из файла.
            string[] res = FileRead("expressions.txt");

            // Удаление файла с ответами, если такой был.
            FileDelete("answers.txt");

            // Вычисление всех выражений.
            foreach (var expr in res)
            {
                FileWrite("answers.txt", $"{Calculate(expr):f3}" + Environment.NewLine);
            }

            // Сверка результатов.
            ExpressionsChecker("answers.txt", "expressions_checker.txt");
        }
    }
}
