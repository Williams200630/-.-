using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;


class Program
{
    static string filePath = "megalopolis_data.txt";

    static void Main()
    {
        InitializeFile();

        while (true)
        {
            Console.WriteLine("\nМеню управления данными о мегалополисах:");
            Console.WriteLine("1. Добавить запись в файл");
            Console.WriteLine("2. Просмотреть все записи");
            Console.WriteLine("3. Вывести информацию о самом населенном городе");
            Console.WriteLine("4. Выход");
            Console.Write("> ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddRecord();
                    break;
                case "2":
                    ViewAllRecords();
                    break;
                case "3":
                    AnalyzeData();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Ошибка: неверный выбор. Пожалуйста, введите число от 1 до 4.");
                    break;
            }
        }
    }
    static void InitializeFile()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                Console.WriteLine($"Файл для данных '{filePath}' успешно создан.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка при создании файла: {ex.Message}");
            Environment.Exit(1);
        }
    }
    static void AddRecord()
    {
        try
        {
            Console.Write("Введите страну: ");
            string country = Console.ReadLine();

            Console.Write("Введите название мегалополиса: ");
            string megalopolis = Console.ReadLine();

            Console.Write("Введите города в составе мегалополиса (через ';'): ");
            string citiesString = Console.ReadLine();
            string[] cities = citiesString.Split(';');

            List<string> populationsList = new List<string>();

            foreach (string city in cities)
            {
                while (true)
                {
                    Console.Write($"Введите население для города '{city.Trim()}': ");
                    string populationInput = Console.ReadLine();

                    if (BigInteger.TryParse(populationInput, out _))
                    {
                        populationsList.Add(populationInput);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка! Введите только цифры. Попробуйте еще раз.");
                    }
                }
            }

            string populations = string.Join(";", populationsList);

            Console.Write("Введите районы городов (можно списком через ';'): ");
            string districts = Console.ReadLine();

            string record = $"{country}|{megalopolis}|{citiesString}|{populations}|{districts}";

            File.AppendAllText(filePath, record + Environment.NewLine);
            Console.WriteLine("Запись успешно добавлена в файл!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при добавлении записи: {ex.Message}");
        }
    }

    static void ViewAllRecords()
    {
        string[] lines = ReadAndValidateFile();
       
        if (lines == null) return;

        Console.WriteLine("\n--- Все записи в файле ---");
        foreach (string line in lines)
        {
            Console.WriteLine(line.Replace("|", " | "));
        }
        Console.WriteLine("--- Конец файла ---");
    }
    static void AnalyzeData()
    {
        string[] allRecords = ReadAndValidateFile();
        // если файл пуст или какя нибидь ошибка на выход
        if (allRecords == null) return;

        try
        {
            string mostPopulousCityName = "";
            BigInteger maxPopulation = -1;
            string sourceMegalopolis = "";
            string sourceCountry = "";

            foreach (string record in allRecords)
            {
                string[] parts = record.Split('|');
                if (parts.Length != 5)
                {
                    Console.WriteLine($"Предупреждение: пропущена некорректная строка: {record}");
                    continue;
                }

                string[] cities = parts[2].Split(';');
                string[] populationsStr = parts[3].Split(';');

                if (cities.Length != populationsStr.Length)
                {
                    Console.WriteLine($"Предупреждение: несоответствие данных в строке: {record}");
                    continue;
                }

                for (int i = 0; i < cities.Length; i++)
                {
                    if (BigInteger.TryParse(populationsStr[i].Trim(), out BigInteger currentPopulation))
                    {
                        if (currentPopulation > maxPopulation)
                        {
                            maxPopulation = currentPopulation;
                            mostPopulousCityName = cities[i].Trim();
                            sourceCountry = parts[0].Trim();
                            sourceMegalopolis = parts[1].Trim();
                        }
                    }
                }
            }

            Console.WriteLine("\n--- Результаты анализа ---");
            if (maxPopulation > -1)
            {
                Console.WriteLine("Информация о самом населенном городе:");
                Console.WriteLine($"Город: {mostPopulousCityName}");
                Console.WriteLine($"Население: {maxPopulation}");
                Console.WriteLine($"Мегалополис: {sourceMegalopolis}");
                Console.WriteLine($"Страна: {sourceCountry}");
            }
            else
            {
                Console.WriteLine("Не удалось найти корректных данных для анализа.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при анализе данных: {ex.Message}");
        }
    }
    // НОВЫЙ МЕТОД: Вспомогательный метод для проверки и чтения файла
    private static string[] ReadAndValidateFile()
    {
        try
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                Console.WriteLine("\nФайл пуст или не существует. Операция невозможна.");
                return null; 
            }
            return File.ReadAllLines(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при доступе к файлу: {ex.Message}");
            return null; // Возвращаем null при ошибке
        }
    }
}