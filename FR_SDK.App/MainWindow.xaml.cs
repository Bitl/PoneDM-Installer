using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace PoneDM_Installer.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window Logic
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
        }

        private void Window_init(object sender, EventArgs e)
        {
        }

        private void window_closing(object sender, CancelEventArgs e)
        {
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion

        #region Launcher Logic
        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minmize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public static void CreateMessageBox(string text)
        {
            CustomMessageBox box = new CustomMessageBox(text);
            box.ShowDialog();
        }

        private void install_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey localKey;

            if (Environment.Is64BitOperatingSystem)
            {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\\Wow6432Node\\Valve\\Steam");
            }
            else
            {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey(@"SOFTWARE\\Valve\\Steam");
            }

            string SteamInstallDir = localKey.GetValue("InstallPath").ToString();

            string SourceModsDir = SteamInstallDir + @"\steamapps\sourcemods";
            string PoneDMInstallDir = SourceModsDir + @"\ponedm";

            if (Directory.Exists(PoneDMInstallDir))
            {
                CreateMessageBox("PoneDM has been detected in your SourceMods folder. The installer will update PoneDM to the latest version by deleting the old version and extracting the new one. MAKE SURE YOU HAVE BACKED UP ANY ADDONS OR CUSTOM CONTENT.");
                Directory.Delete(PoneDMInstallDir, true);
            }

            string ModZipDir = AppDomain.CurrentDomain.BaseDirectory + @"\ponedm.zip";

            ZipFile.ExtractToDirectory(ModZipDir, SourceModsDir);

            while (!Directory.Exists(PoneDMInstallDir))
            {
                Task.Delay(5);
                if (Directory.Exists(PoneDMInstallDir))
                {
                    break;
                }
            }

            CreateMessageBox("PoneDM has been installed to " + PoneDMInstallDir + "! Please restart Steam to see it in your library!\n\n" +
                "NOTE: Please make sure to install the Source SDK 2013 Multiplayer before launching. To do so, install it from the 'TOOLS' category in the Steam library, and then re-open the installer.");

            Close();
        }
        #endregion
    }
}
