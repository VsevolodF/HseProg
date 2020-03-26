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

    delegate void ErrorNotificationType(string message);

    class Calculator
    {
        string resultingFile;
        public Calculator(string resFilePath = "answers.txt")
        {
            resultingFile = resFilePath;
        }

        public event ErrorNotificationType ErrorNotification;

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
        public double Calculate(string expr)
        {
            // Разбиение выражения на два аргумента и операцию.
            string[] expression = expr.Split();

            // Возврат результата.
            return operations[expression[1]](double.Parse(expression[0]), double.Parse(expression[2]));
        }

        /// <summary>
        /// Метод, вычисляющий все выражения в файле.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        public void CalulateAllExpressions(string path)
        {
            string[] res = FileRead(path);
            FileDelete(resultingFile);

            // Вычисление всех выражений.
            foreach (var expr in res)
            {
                // Я правдла не понимаю зачем это все нужно, т.к у нас тип дабл, а значит
                // мы прекрасно можем и делить на 0, и получать бесконечность в качестве ответа.
                try
                {
                    // Если выражение задано некорректно, то в файл ничего не запишется, а,
                    // следовательно, все остальные ответы поедут. В условии ничего об этом 
                    // не сказано, поэтому ничего с этим и не сделал.
                    FileWrite(resultingFile, $"{Calculate(expr):f3}" + Environment.NewLine);
                }
                catch (KeyNotFoundException)
                {
                    ErrorNotification("неверный оператор" + Environment.NewLine);
                }
                catch (DivideByZeroException)
                {
                    ErrorNotification("bruh" + Environment.NewLine);
                }
                catch (NotFiniteNumberException)
                {
                    ErrorNotification("не число" + Environment.NewLine);
                }
                catch (Exception)
                {
                    ErrorNotification("Непредвиденная ошибка" + Environment.NewLine);
                }
            }
        }


        /// <summary>
        /// Метод, считывающий данные из файла.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        /// <returns> Возвращает массив строк из файла. </returns>
        public string[] FileRead(string path)
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
                ErrorNotification($"IO Exception: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification($"Unauthorized Access Exception: {e.Message}");
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification($"Security Exception: {e.Message}");
            }
            catch (Exception e)
            {
                ErrorNotification($"Exception: {e.Message}");
            }

            return res;
        }
        /// <summary>
        /// Метод, записывающий данные в файл.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        public void FileWrite(string path, string data)
        {
            try
            {
                // Запись в файл построчно.
                File.AppendAllText(path, data);
            }
            // Улавливание возможных ошибок.
            catch (IOException e)
            {
                ErrorNotification($"IO Exception: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification($"Unauthorized Access Exception: {e.Message}");
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification($"Security Exception: {e.Message}");
            }
            catch (Exception e)
            {
                ErrorNotification($"Exception: {e.Message}");
            }
        }
        /// <summary>
        /// Метод, удаляющий файл.
        /// </summary>
        /// <param name="path"> Путь к файлу. </param>
        public void FileDelete(string path)
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
                ErrorNotification($"IO Exception: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorNotification($"Unauthorized Access Exception: {e.Message}");
            }
            catch (System.Security.SecurityException e)
            {
                ErrorNotification($"Security Exception: {e.Message}");
            }
            catch (Exception e)
            {
                ErrorNotification($"Exception: {e.Message}");
            }
        }

        /// <summary>
        /// Метод, проверяющий совпадение посчитанных результатов с данными.
        /// </summary>
        /// <param name="dataPath"> Путь к файлу с вычисленными результатами. </param>
        /// <param name="ansPath"> Путь к файлу с данными результатами. </param>
        public void ExpressionsChecker(string ansPath, string logPath)
        {
            // Удаление старого отчета, если он существует.
            FileDelete(logPath);

            // Счетчик несошедшихся ответов.
            int wrongAns = 0;

            // Получение данных из обоих файлов.
            string[] data = FileRead(resultingFile);
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
                    FileWrite(logPath, "OK" + Environment.NewLine);
                else
                {
                    wrongAns++;
                    FileWrite(logPath, "Error" + Environment.NewLine);
                }
            }

            FileWrite(logPath, wrongAns.ToString());
        }
    }
    class Program
    {
        const string expressionsPath = "expressions.txt";

        static void Main(string[] args)
        {
            Calculator calc = new Calculator("answers.txt");

            calc.ErrorNotification += ConsoleErrorHandler;
            calc.ErrorNotification += ResultErrorHandler;

            // Подчсет всех выражений.
            calc.CalulateAllExpressions(expressionsPath);
            // Сверка результатов.
            calc.ExpressionsChecker("expressions_checker.txt", "results.txt");


            Console.ReadLine();        
        }


        static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine(message + $" {DateTime.Now.TimeOfDay}");
        }

        public static void ResultErrorHandler(string message)
        {
            try
            {
                // Этот файл нужно очищать при каждом перезапуске программы.
                // (есть выбор или делать еще один try-catch, либо оставить так)
                if (message == "не число" + Environment.NewLine)
                    File.AppendAllText("log.txt", "не число" + Environment.NewLine);
                if (message == "неверный оператор" + Environment.NewLine)
                    File.AppendAllText("log.txt", "неверный оператор" + Environment.NewLine);
                if (message == "bruh" + Environment.NewLine)
                    File.AppendAllText("log.txt", "bruh" + Environment.NewLine);
            }
            // Улавливание возможных ошибок.
            catch (IOException e)
            {
                Console.WriteLine($"IO Exception: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Unauthorized Access Exception: {e.Message}");
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine($"Security Exception: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }

        }
    }
}
