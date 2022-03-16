using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

namespace HashFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnSelFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            var ok = dig.ShowDialog();
            if (ok.HasValue && ok == true)
            {
                TbSelectedFile.Text = dig.FileName;
                try
                {
                    using (var stream = dig.OpenFile())
                    {
                        await HandleStream(stream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

        }
        Task<string> GetHashString(HashAlgorithm hash, System.IO.Stream stream)
        {
            return Task.Run(() => BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
        }
        private async Task HandleStream(System.IO.Stream stream)
        {
            string hashStr = CbHashAlgorithm.SelectionBoxItem.ToString() ?? "MD5";
            HashAlgorithm hash = MD5.Create();
            if (hashStr == "SHA1")
            {
                hash = SHA1.Create();
            }
            else if (hashStr == "SHA256")
            {
                hash = SHA256.Create();
            }
            else if (hashStr == "MD5")
            {
                hash = MD5.Create();
            }
            using (hash)
            {
                string result = await GetHashString(hash, stream);
                TbComputedHash.Text = result;
                if (string.Compare(result, TbExpectedHash.Text.Trim(), true) == 0)
                {
                    LblCompareResult.Content = "OK";
                }
                else
                {
                    LblCompareResult.Content = "FAIL";
                }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                {
                    if (files.Length > 0)
                    {
                        TbSelectedFile.Text = files[0];
                        HandleFile(files[0]);
                    }
                }
            }
        }

        private async void HandleFile(string v)
        {
            if (File.GetAttributes(v).HasFlag(FileAttributes.Directory))
            {
                MessageBox.Show("Please Drag File To This Window");
                return;
            }
            try
            {
                using (System.IO.Stream stream = new System.IO.FileStream(v, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    await HandleStream(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
