namespace PutMessager
{
    class Program
    {
        static void Main(string[] args)
        {
            string? messagePath = null;
            string? stegoPath = null;
            string? containerPath = null;
            bool showHelp = false;

            // Разбор аргументов командной строки
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-m":
                    case "--message":
                        if (i + 1 < args.Length)
                        {
                            messagePath = args[++i];
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: отсутствует путь к файлу сообщения.");
                            return;
                        }
                        break;
                    case "-s":
                    case "--stego":
                        if (i + 1 < args.Length)
                        {
                            stegoPath = args[++i];
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: отсутствует путь для стегоконтейнера.");
                            return;
                        }
                        break;
                    case "-c":
                    case "--container":
                        if (i + 1 < args.Length)
                        {
                            containerPath = args[++i];
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: отсутствует путь к файлу контейнера.");
                            return;
                        }
                        break;
                    case "-h":
                    case "--help":
                        showHelp = true;
                        break;
                    default:
                        Console.WriteLine($"Неизвестный параметр: {args[i]}");
                        ShowHelp();
                        return;
                }
            }

            if (showHelp || string.IsNullOrEmpty(containerPath))
            {
                ShowHelp();
                return;
            }

            try
            {
                // Чтение сообщения
                byte[] messageBytes;
                if (!string.IsNullOrEmpty(messagePath))
                {
                    if (!File.Exists(messagePath))
                    {
                        Console.WriteLine("Файл с сообщением не найден.");
                        return;
                    }
                    messageBytes = File.ReadAllBytes(messagePath);
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        Console.OpenStandardInput().CopyTo(ms);
                        messageBytes = ms.ToArray();
                    }
                }

                // Количество байтов в сообщении
                int messageLength = messageBytes.Length;

                // Конвертация сообщения в биты
                List<int> bits = new List<int>();
                foreach (byte b in messageBytes)
                {
                    for (int i = 7; i >= 0; i--)
                    {
                        bits.Add((b >> i) & 1);
                    }
                }

                // Конвертация длины сообщения в биты (16 бит)
                List<int> lengthBits = new List<int>();
                for (int i = 15; i >= 0; i--)
                {
                    lengthBits.Add((messageLength >> i) & 1);
                }

                // Чтение контейнера
                if (!File.Exists(containerPath))
                {
                    Console.WriteLine("Файл контейнера не найден.");
                    return;
                }
                var lines = File.ReadAllLines(containerPath);

                // Проверка, достаточно ли строк для встраивания длины и сообщения
                if (lines.Length < lengthBits.Count + bits.Count)
                {
                    Console.WriteLine("Недостаточно строк в контейнере для встраивания длины и всего сообщения.");
                    return;
                }

                // Встраивание длины сообщения в контейнер
                int bitIndex = 0;
                for (int i = 0; i < lengthBits.Count; i++)
                {
                    lines[i] = lines[i].TrimEnd();
                    if (lengthBits[i] == 0)
                    {
                        lines[i] += " "; // 0 бит - добавляем пробел
                    }
                    // 1 бит - ничего не добавляем
                }

                // Встраивание сообщения
                bitIndex = 0;
                for (int i = lengthBits.Count; i < lines.Length; i++)
                {
                    if (bitIndex < bits.Count)
                    {
                        lines[i] = lines[i].TrimEnd();

                        if (bits[bitIndex] == 0)
                        {
                            lines[i] += " "; // 0 бит - добавляем пробел
                        }
                        // Если бит равен 1, ничего не добавляем
                        bitIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (bitIndex < bits.Count)
                {
                    Console.WriteLine("Недостаточно строк в контейнере для встраивания всего сообщения.");
                    return;
                }

                // Запись стегоконтейнера
                if (!string.IsNullOrEmpty(stegoPath))
                {
                    File.WriteAllLines(stegoPath, lines);
                }
                else
                {
                    foreach (var line in lines)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Использование: PutMessager.exe -m <файл_сообщения> -s <файл_стегоконтейнера> -c <файл_контейнера>");
            Console.WriteLine("Параметры:");
            Console.WriteLine("-m, --message      Путь к файлу с сообщением. Если не указан, сообщение читается из стандартного потока ввода.");
            Console.WriteLine("-s, --stego        Путь для записи стегоконтейнера. Если не указан, результат выводится в стандартный поток вывода.");
            Console.WriteLine("-c, --container    Путь к файлу-контейнеру. Обязательный параметр!");
            Console.WriteLine("-h, --help         Показать справку.");
        }
    }
}
