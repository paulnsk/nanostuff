using ConsoleTools;

namespace RandomFileGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var howMany = int.Parse(Konsole.ReadString("Сколько хотите файлов? ", "20", 3));
            var minSize = int.Parse(Konsole.ReadString("Минимальный размер, байт: ", "1000000", 9));
            var maxSize = int.Parse(Konsole.ReadString("Максимальный размер, байт: ", "100000000", 9));
            var dir = Konsole.ReadString("Куда их? ", @"k:\nanofiles\", 9);


            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    Konsole.WriteLine($"Директория ♣gсоздана♣=: ♣m{dir}");
                }
                catch (Exception ex)
                {
                    Konsole.WriteLine($"♣RОшибка при создании директории '{dir}': {ex.Message}");
                    return; 
                }
            }

            var rnd = new Random();
            for (var i = 0; i < howMany; i++)
            {
                var randomSize = rnd.Next(minSize, maxSize);
                var filePath = Path.Combine(dir, $"{RandomDataGenerator.RandomName(3, 8, RandomDataGenerator.CapMode.NoCaps)}_{i + 1}.txt");

                Konsole.WriteLine($"♣=Создание файла ♣g{i + 1}♣=/♣g{howMany}♣=: ♣b{filePath}♣= (♣g{randomSize / 1024:N0}♣= KB)♣y...");

                RandomDataGenerator.GenerateRandomFile(filePath, randomSize, true);
            }

            Konsole.WriteLine("\n♣gГотово! Все файлы успешно созданы.♣=");
        }



    }
}
