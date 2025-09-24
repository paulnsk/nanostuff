using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCleaner.BLL.Utilities;


/// <summary>
/// Блочная читалка текстовых файлов, позволяющая получать их для обработки в виде коротких фрагментов, без полной загрузки в память, гарантирующая, что
/// каждый фрагмент будет содержать слова целиком (резка по пробелам)
/// </summary>
public class BlockStreamReader(Stream stream, int bufferSize = 65536) : IDisposable
{
    // Читает по буквам а не по байтам
    private readonly StreamReader _reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);


    public IEnumerable<string> ReadBlocks(CancellationToken token)
    {
        var buffer = new char[bufferSize];
        var tail = string.Empty;
        int charsRead;

        while ((charsRead = _reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            token.ThrowIfCancellationRequested();
            var currentText = tail + new string(buffer, 0, charsRead);

            if (_reader.EndOfStream)
            {
                yield return currentText;
                tail = string.Empty;
                break;
            }

            var lastSpaceIndex = currentText.LastIndexOf(' ');

            if (lastSpaceIndex == -1)
            {
                tail = currentText;
            }
            else
            {
                // Включаем пробел в текущий блок, на случай если это пробел, идущий за коротким словом - мы удалим его вместе с этим словом
                yield return currentText[..(lastSpaceIndex + 1)];
                // Все что после пробела - хвост, приклеим к следующему блоку
                tail = currentText[(lastSpaceIndex + 1)..];
            }
        }

        if (!string.IsNullOrEmpty(tail))
        {
            // Хвост после цикла теперь может быть только если весь файл - одно слово без пробелов
            yield return tail;
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}