namespace TextCleaner.BLL.Utilities;

public static class Utils
{
    public static void EnsureDir(string filePath)
    {
        var dirPath = Path.GetDirectoryName(filePath);
        if (dirPath == null) throw new Exception($"{nameof(dirPath)} null in {nameof(EnsureDir)}");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
    }
}