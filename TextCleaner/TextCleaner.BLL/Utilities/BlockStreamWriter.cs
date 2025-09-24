using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCleaner.BLL.Utilities;

/// <summary>
/// Блочная писалка, антипод блочной читалки <see cref="BlockStreamReader"/>
/// </summary>
public class BlockStreamWriter(Stream stream)
{
    public void WriteBlock(string block)
    {
        var bytes = Encoding.UTF8.GetBytes(block);
        stream.Write(bytes, 0, bytes.Length);
    }
}