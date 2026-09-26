using System.IO.Compression;

internal static class Program
{
    private const string RequiredDir = @"C:\Windows\Apps";

    private static int Main(string[] args)
    {
        try
        {
            _ = args;
            var dest = Path.GetFullPath(RequiredDir);
            var zipPath = Path.Combine(dest, "apps.zip");

            if (!File.Exists(zipPath))
                throw new FileNotFoundException("apps.zip not found.", zipPath);

            Directory.CreateDirectory(dest);

            using var zip = ZipFile.OpenRead(zipPath);
            foreach (var entry in zip.Entries)
            {
                var target = Path.GetFullPath(Path.Combine(dest, entry.FullName));
                if (!IsUnder(target, dest))
                    throw new InvalidOperationException("Blocked zip path: " + entry.FullName);

                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(target);
                    continue;
                }

                var dir = Path.GetDirectoryName(target);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                entry.ExtractToFile(target, overwrite: true);
            }

            File.WriteAllText(
                Path.Combine(dest, ".oem-install-complete"),
                DateTime.UtcNow.ToString("o") + Environment.NewLine);

            return 0;
        }
        catch (Exception ex)
        {
            try
            {
                Directory.CreateDirectory(@"C:\ProgramData\OEM");
                File.AppendAllText(
                    @"C:\ProgramData\OEM\apps-msi-extract.log",
                    DateTime.Now.ToString("o") + " " + ex + Environment.NewLine);
            }
            catch { }
            return 1;
        }
    }

    private static string NormalizeDest(string dest)
    {
        dest = Path.GetFullPath(dest);
        var required = Path.GetFullPath(RequiredDir);
        if (!string.Equals(dest, required, StringComparison.OrdinalIgnoreCase))
            dest = required;
        return dest;
    }

    private static bool IsUnder(string path, string root)
    {
        var r = Path.GetFullPath(root).TrimEnd('\\') + "\\";
        var p = Path.GetFullPath(path);
        return p.StartsWith(r, StringComparison.OrdinalIgnoreCase)
               || string.Equals(p, Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase);
    }
}
