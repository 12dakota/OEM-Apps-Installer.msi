using System.IO.Compression;

internal static class Program
{
    private const string InstallDir = @"C:\Windows\Apps";
    private const string RecoveryDir = @"C:\Recovery\OEM\Apps";
    private const string LogDir = @"C:\ProgramData\OEM";

    private static int Main(string[] args)
    {
        try
        {
            var zipPath = ResolveZip(args);
            var skipRecovery = args.Any(a => a.Equals("/no-recovery", StringComparison.OrdinalIgnoreCase));

            Log($"Zip: {zipPath}");
            Log($"Target: {InstallDir}");

            if (!File.Exists(zipPath))
            {
                Log($"Missing zip: {zipPath}");
                return 2;
            }

            Deploy(zipPath, InstallDir);

            if (!skipRecovery)
            {
                Deploy(zipPath, RecoveryDir);
            }

            var hook = Path.Combine(InstallDir, "install.cmd");
            if (File.Exists(hook))
            {
                Log($"Running {hook}");
                var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = hook,
                    WorkingDirectory = InstallDir,
                    UseShellExecute = false
                });
                p?.WaitForExit();
                Log($"install.cmd exit={p?.ExitCode}");
            }

            Log("Done.");
            return 0;
        }
        catch (Exception ex)
        {
            Log(ex.ToString());
            return 1;
        }
    }

    private static string ResolveZip(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals("/zip", StringComparison.OrdinalIgnoreCase))
                return Path.GetFullPath(args[i + 1]);
        }

        var beside = Path.Combine(AppContext.BaseDirectory, "apps.zip");
        if (File.Exists(beside))
            return beside;

        return Path.Combine(Environment.CurrentDirectory, "apps.zip");
    }

    private static void Deploy(string zipPath, string dest)
    {
        Directory.CreateDirectory(dest);
        using var zip = ZipFile.OpenRead(zipPath);
        foreach (var entry in zip.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name) && entry.FullName.EndsWith('/'))
            {
                Directory.CreateDirectory(Path.Combine(dest, entry.FullName));
                continue;
            }

            var target = Path.GetFullPath(Path.Combine(dest, entry.FullName));
            if (!target.StartsWith(Path.GetFullPath(dest) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(target, Path.GetFullPath(dest), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Blocked zip path: {entry.FullName}");
            }

            var dir = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (!string.IsNullOrEmpty(entry.Name))
                entry.ExtractToFile(target, overwrite: true);
        }

        Log($"Extracted to {dest}");
    }

    private static void Log(string message)
    {
        var line = $"{DateTime.Now:o} {message}";
        Console.WriteLine(line);
        try
        {
            Directory.CreateDirectory(LogDir);
            File.AppendAllText(Path.Combine(LogDir, "apps-deploy.log"), line + Environment.NewLine);
        }
        catch
        {
            // best-effort log
        }
    }
}
