using System.Text;

namespace RandomFileGenerator
{
    public static class RandomDataGenerator
    {

        #region private
        private static string[] Vowels => new[]
        {
            "a", "e", "i", "o",  "u", "y"
        };

        private static string[] Consonants => new[]
        {
            "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n",  "p", "q", "r", "s", "t", "v", "w", "x",  "z"
        };


        //private static string[] Vowels => new[]
        //{
        //    "а", "е", "ё", "и", "о", "у", "ы", "э", "ю", "я"
        //};

        //private static string[] Consonants => new[]
        //{
        //    "б", "в", "г", "д", "ж", "з", "й", "к", "л", "м", "н", "п", "р", "с", "т", "ф", "х", "ц", "ч", "ш", "щ",
        //};


        private const int MaxVowelsInARow = 1;
        private const int MaxConsonantsInARow = 1;

        private static readonly Random _rnd = new Random();

        private static string RandomVowel => Vowels[_rnd.Next(Vowels.Length)];
        private static string RandomConsonant => Consonants[_rnd.Next(Consonants.Length)];

        #endregion private


        #region Public



        #endregion Public

        /// <summary>
        /// Produces a string containing random digits
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomNumber(int length)
        {
            var result = "";
            for (var i = 0; i < length; i++)
            {
                result += _rnd.Next(0, 9).ToString();
            }

            return result;
        }


        /// <summary>
        /// Produces a readable and memorizable random word
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="capMode"></param> 
        /// <returns></returns>
        public static string RandomName(int min, int max, CapMode capMode)
        {
            var result = "";
            var lastCharKindAndCount = 0; //vowels are positive, consonants are negative
            for (int i = 0; i < _rnd.Next(max - min + 1) + min; i++) //at least one char
            {
                var nextChar = "?";
                if (lastCharKindAndCount >= MaxVowelsInARow) //was a vowel, must be consonant
                {
                    nextChar = RandomConsonant;
                    lastCharKindAndCount = -1;
                }
                else if (lastCharKindAndCount <= -MaxConsonantsInARow) //two consonants in a row, need a vowel
                {
                    nextChar = RandomVowel;
                    lastCharKindAndCount = 1;
                }
                else //pick randomly
                {
                    if (_rnd.Next(2) == 0)
                    {
                        nextChar = RandomVowel;
                        if (lastCharKindAndCount > 0) lastCharKindAndCount++; //already had vowel
                        else lastCharKindAndCount = 1;

                    }
                    else
                    {
                        nextChar = RandomConsonant;
                        if (lastCharKindAndCount < 0) lastCharKindAndCount--; //already had consonant
                        else lastCharKindAndCount = -1;
                    }
                }

                result += nextChar;
            }

            switch (capMode)
            {
                case CapMode.NoCaps:
                    break;
                case CapMode.AllCaps:
                    result = result.ToUpper();
                    break;
                case CapMode.FirstCap:
                    result = result[0].ToString().ToUpper()[0] + result.Substring(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(capMode), capMode, null);
            }

            return result;
        }

        public enum CapMode
        {
            NoCaps, AllCaps, FirstCap
        }


        /// <summary>
        /// Generates a paragraph of random characters which resembles a meaningful text
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string RandomText(int minLength, int maxLength)
        {
            var sb = new StringBuilder();
            var mode = CapMode.FirstCap;

            var length = minLength + _rnd.Next(maxLength - minLength + 1);

            while (sb.Length < length)
            {
                var word = RandomName(2, 12, mode);

                sb.Append(word);
                if (_rnd.Next(15) == 1)
                {
                    sb.Append(". ");
                    mode = CapMode.FirstCap;
                }
                else if (_rnd.Next(50) == 1)
                {
                    sb.Append("! ");
                    mode = CapMode.FirstCap;
                }
                else if (_rnd.Next(8) == 1)
                {
                    sb.Append(", ");
                    mode = CapMode.NoCaps;
                }
                else if (_rnd.Next(70) == 1)
                {
                    sb.Append(" - ");
                    mode = CapMode.NoCaps;
                }
                else if (_rnd.Next(70) == 1)
                {
                    sb.Append(": ");
                    mode = CapMode.NoCaps;
                }
                else
                {
                    sb.Append(" ");
                    mode = CapMode.NoCaps;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Produces a readable and memorizable string containing one capital letter, up to 8 total letters and 3 digits
        /// </summary>
        /// <returns></returns>
        public static string RandomPassword()
        {
            return RandomName(5, 8, CapMode.FirstCap) + RandomNumber(3);
        }

        public static DateTime RandomDate(DateTime startDate, DateTime endDate)
        {
            var timeSpan = endDate - startDate;
            var newSpan = new TimeSpan(0, _rnd.Next(0, (int)timeSpan.TotalMinutes), 0);
            var newDateTime = startDate + newSpan;
            return newDateTime.Date;
        }


        /// <summary>
        /// Generates a file with random text content of a specified size.
        /// The file is written to disk in chunks, avoiding high memory usage.
        /// The final file size will be approximately the specified size.
        /// </summary>
        /// <param name="filePath">The full path to the file to be created.</param>
        /// <param name="fileSizeInBytes">The desired approximate size of the file in bytes.</param>
        /// <param name="addSomeFanciness">If true, adds random empty lines, paragraphs and indented lines.</param>
        public static void GenerateRandomFile(string filePath, long fileSizeInBytes, bool addSomeFanciness = false)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                if (!addSomeFanciness)
                {
                    while (writer.BaseStream.Length < fileSizeInBytes)
                    {
                        var chunk = RandomText(500, 1000);
                        writer.Write(chunk);
                    }
                }
                else
                {
                    while (writer.BaseStream.Length < fileSizeInBytes)
                    {
                        var chunk = RandomText(100, 400); // Генерируем более короткими порциями для форматирования
                        writer.Write(chunk);

                        // С некоторой вероятностью добавляем "украшения"
                        var chance = _rnd.Next(25);
                        switch (chance)
                        {
                            case 0: // Шанс ~4% на разрыв абзаца
                                writer.WriteLine();
                                writer.WriteLine();
                                break;
                            case 1: // Шанс ~4% на новую строку с отступом
                                writer.WriteLine();
                                writer.Write(new string(' ', _rnd.Next(2, 10)));
                                break;
                            case 2:
                            case 3:
                            case 4: // Шанс ~12% просто на пустую строку
                                writer.WriteLine();
                                break;
                                // В остальных случаях (~80%) просто продолжаем писать текст в ту же строку
                        }
                    }
                }
            }
        }


    }

}
