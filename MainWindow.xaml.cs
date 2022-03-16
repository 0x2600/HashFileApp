using Microsoft.Win32;
using System;
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
                using (var stream = dig.OpenFile())
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
                        string result = string.Empty;
                        await Task.Run(() => result = BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", ""));
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
            }

        }
    }
}
