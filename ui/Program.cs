using System.Diagnostics;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new InstallForm());
    }
}

internal sealed class InstallForm : Form
{
    private readonly Button _cancel;
    private readonly Button _minimize;
    private readonly Label _status;
    private Process? _setup;
    private bool _done;

    public InstallForm()
    {
        Text = "OEM Apps Installer";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        StartPosition = FormStartPosition.CenterScreen;
        Width = 460;
        Height = 180;
        ShowInTaskbar = true;

        _status = new Label
        {
            Left = 16,
            Top = 20,
            Width = 410,
            Height = 48,
            Text = "Installing to C:\\Windows\\Apps.\nCancel and Close minimize this window."
        };
        _minimize = new Button { Text = "Minimize", Left = 220, Top = 90, Width = 100 };
        _cancel = new Button { Text = "Cancel", Left = 330, Top = 90, Width = 100 };
        _minimize.Click += (_, _) => WindowState = FormWindowState.Minimized;
        _cancel.Click += (_, _) => MinimizeInsteadOfClose();

        Controls.Add(_status);
        Controls.Add(_minimize);
        Controls.Add(_cancel);

        Load += (_, _) => StartInstall();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_done)
        {
            base.OnFormClosing(e);
            return;
        }

        // Title-bar X, Alt+F4, and Cancel minimize; they do not exit.
        if (e.CloseReason is CloseReason.UserClosing)
        {
            e.Cancel = true;
            MinimizeInsteadOfClose();
            return;
        }

        base.OnFormClosing(e);
    }

    private void MinimizeInsteadOfClose()
    {
        WindowState = FormWindowState.Minimized;
    }

    private void StartInstall()
    {
        var dir = AppContext.BaseDirectory;
        var msi = Path.Combine(dir, "OEM-Apps-Installer.msi");
        if (!File.Exists(msi))
            msi = Path.Combine(dir, "OemApps.msi");

        if (!File.Exists(msi))
        {
            _status.Text = "OEM-Apps-Installer.msi not found next to this program.";
            _done = true;
            _cancel.Text = "Close";
            _cancel.Click -= (_, _) => MinimizeInsteadOfClose();
            _cancel.Click += (_, _) => { _done = true; Close(); };
            return;
        }

        _setup = Process.Start(new ProcessStartInfo
        {
            FileName = "msiexec.exe",
            Arguments = $"/i \"{msi}\" /qn /norestart /l*v \"{Path.Combine(Path.GetTempPath(), "OEM-Apps-Installer.log")}\"",
            UseShellExecute = false
        });

        if (_setup is null)
        {
            _status.Text = "Could not start msiexec.";
            return;
        }

        _setup.EnableRaisingEvents = true;
        _setup.Exited += (_, _) => BeginInvoke(OnInstallFinished);
    }

    private void OnInstallFinished()
    {
        _done = true;
        var code = _setup?.ExitCode ?? -1;
        _status.Text = code == 0
            ? "Install finished. Files are in C:\\Windows\\Apps."
            : $"Installer exited with code {code}.";
        _cancel.Text = "Close";
        _cancel.Click += (_, _) => Close();
    }
}
