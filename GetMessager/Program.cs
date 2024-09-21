namespace GetMessager
{
    class Program
    {
        static void Main(string[] args)
        {
            string? messagePath = null;
            string? stegoPath = null;

            // Парсинг аргументов командной строки
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
                            Console.Error.WriteLine("Ошибка: отсутствует путь к файлу для записи сообщения.");
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
                            Console.Error.WriteLine("Ошибка: отсутствует путь к файлу стегоконтейнера.");
                            return;
                        }
                        break;
                    case "-h":
                    case "--help":
                        ShowHelp();
                        return;
                    default:
                        Console.Error.WriteLine($"Неизвестный параметр: {args[i]}");
                        ShowHelp();
                        return;
                }
            }

            try
            {
                string[] lines;

                // Чтение стегоконтейнера
                if (string.IsNullOrEmpty(stegoPath))
                {
                    // Чтение из стандартного ввода
                    using (var reader = new StreamReader(Console.OpenStandardInput()))
                    {
                        lines = reader.ReadToEnd().Split(new[] { '\n' }, StringSplitOptions.None);
                    }
                }
                else
                {
                    if (!File.Exists(stegoPath))
                    {
                        Console.Error.WriteLine("Файл стегоконтейнера не найден.");
                        return;
                    }
                    lines = File.ReadAllLines(stegoPath);
                }

                // Извлечение длины сообщения
                if (lines.Length < 16)
                {
                    Console.Error.WriteLine("Недостаточно строк в стегоконтейнере для извлечения длины сообщения.");
                    return;
                }

                int messageLength = ExtractMessageLength(lines);

                // Извлечение самого сообщения
                byte[] messageBytes = ExtractMessage(lines, messageLength);

                // Запись сообщения
                if (!string.IsNullOrEmpty(messagePath))
                {
                    File.WriteAllBytes(messagePath, messageBytes);
                }
                else
                {
                    Console.OpenStandardOutput().Write(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static int ExtractMessageLength(string[] lines)
        {
            int length = 0;
            for (int i = 0; i < 16; i++)
            {
                if (lines[i].EndsWith(" "))
                {
                    // 0 бит
                    length = (length << 1) | 0;
                }
                else
                {
                    // 1 бит
                    length = (length << 1) | 1;
                }
            }
            return length;
        }

        static byte[] ExtractMessage(string[] lines, int messageLength)
        {
            using (MemoryStream messageStream = new MemoryStream())
            {
                int bitIndex = 0;
                byte currentByte = 0;
                int messageBitsToRead = messageLength * 8; // Количество битов, которые нужно считать

                // Начинаем чтение с 17-й строки (индекс 16)
                for (int i = 16; i < lines.Length && messageBitsToRead > 0; i++)
                {
                    if (lines[i].EndsWith(" "))
                    {
                        currentByte = (byte)(currentByte << 1); // 0 бит
                    }
                    else
                    {
                        currentByte = (byte)((currentByte << 1) | 1); // 1 бит
                    }

                    bitIndex++;
                    messageBitsToRead--;

                    if (bitIndex == 8)
                    {
                        messageStream.WriteByte(currentByte);
                        bitIndex = 0;
                        currentByte = 0;
                    }
                }

                return messageStream.ToArray();
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Использование: GetMessager.exe -s <путь к стегоконтейнеру> -m <путь для сообщения>");
            Console.WriteLine("Параметры:");
            Console.WriteLine("-m, --message     Путь к файлу, в который нужно записать сообщение. Если не указан, выводится в стандартный поток вывода.");
            Console.WriteLine("-s, --stego       Путь к стегоконтейнеру. Если не указан, читается из стандартного потока ввода.");
            Console.WriteLine("-h, --help        Показать справку.");
        }
    }
}
