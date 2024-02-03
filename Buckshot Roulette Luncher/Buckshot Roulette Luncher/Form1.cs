using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buckshot_Roulette_Luncher
{
    public partial class Form1 : Form
    {
        bool canlunch = true;
        //  
        bool dowmloading = false;

        public Form1()
        {
           
            InitializeComponent();

            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // 組合檔案路徑
            string filePath = Path.Combine(appDataFolder, "ReLoad.data");
            try
            {
                // 檢查檔案是否存在
                if (File.Exists(filePath))
                {
                    // 讀取檔案內容
                    string content = File.ReadAllText(filePath);
                    bool isContentTrue = content.Trim() == "true";
                    if (isContentTrue == true)
                    {
                        string filePath2 = System.IO.Path.Combine(yourFolderPath, "Buckshot_Roulette.exe");
                        if (File.Exists(filePath2))
                        {
                            File.Delete(filePath2);
                        }
                    }
                    
                }
                else
                {
                    Console.WriteLine($"檔案不存在: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"存取檔案時發生錯誤: {ex.Message}");
            }
        }



        static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        // 以管理员权限重新运行
        static void RunAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Verb = "runas"; // 请求管理员权限
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;

            try
            {
                Process.Start(startInfo);

                Application.Exit();
            }
            catch
            {
                Application.Exit();
            }

            Application.Exit();
        }








        string yourFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Buckshot Roulette Launcher");
        string uninstallKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "正在檢查驅動";
            await Task.Delay(1000);
            check("{038E31C6-56FA-4A22-AFFB-FD0191290D6C}");
            check("{E900C16E-16BC-3334-C580-C397ADF00392}");
            check("{37B8F9C7-03FB-3253-8781-2517C99D7C00}");
            if (canlunch == true)
            {
              

                // 在 AppData 資料夾下創建你的資料夾路徑
                
                if (!Directory.Exists(yourFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(yourFolderPath);
                        Console.WriteLine($"資料夾已創建: {yourFolderPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"無法創建資料夾: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"資料夾已存在: {yourFolderPath}");
                }
                dowmloading = true;
                progressBar1.Maximum = 100;
                progressBar1.Minimum = 0;
                string filePath = System.IO.Path.Combine(yourFolderPath, "Buckshot_Roulette.exe");
                if (File.Exists(filePath))
                {
                    try
                    {

                        System.Diagnostics.Process.Start(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("錯誤" + ex + "\n\n建議重新開啟此應用", "錯誤");
                        File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ReLoad.data"), "true");
                    }
                    button1.Text = "啟動遊戲/下載遊戲";
                    dowmloading = false;
                    button1.Enabled = true;
                }
                else
                {
                    Lunch();
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    button1.Text = "正在下載";
                }
                   
                
            }
            else
            {
                button1.Text = "啟動遊戲/下載遊戲";
                button1.Enabled = true;
            }
        }

        private void check(string targetKeyName)
        {
            // 開啟指定路徑的註冊表機碼
            using (RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(uninstallKeyPath))
            {
                if (uninstallKey != null)
                {
                    // 開啟特定機碼
                    using (RegistryKey subKey = uninstallKey.OpenSubKey(targetKeyName))
                    {

                        // 確保機碼存在並包含 DisplayName 屬性
                        Console.WriteLine($"已找到機碼:{targetKeyName}");
                    }
                }
                else
                {
                    Console.WriteLine("指定的註冊表路徑不存在。");
                    canlunch = false;
                    MessageBox.Show("請下載遊戲驅動", "Luncher");
                }
            }
        }

        private async void Lunch()
        {
            string downloadUrl = "https://github.com/Nickyangtpe/Buckshot-Roulette-Launcher/releases/download/%E9%81%8A%E6%88%B2%E6%9C%AC%E9%AB%94(%E9%9D%9E%E5%95%9F%E5%8B%95%E5%99%A8)/Buckshot.Roulette.exe";
            string targetFolder = yourFolderPath;

            try
            {
                using (WebClient webClient = new WebClient())
                {

                    // 設定下載進度事件
                    webClient.DownloadProgressChanged += (sender, e) => WebClient_DownloadProgressChanged(e);

                    // 下載檔案到目標資料夾 (使用異步方法)
                    await DownloadFileAsync(webClient, downloadUrl, System.IO.Path.Combine(targetFolder, "Buckshot_Roulette.exe"));
                }

                Console.WriteLine("下載完成");

                System.Diagnostics.Process.Start(System.IO.Path.Combine(targetFolder, "Buckshot_Roulette.exe"));

                progressBar1.Style = ProgressBarStyle.Marquee;
            }
            catch (Exception ex)
            {
                // 下載失敗，顯示錯誤訊息
                MessageBox.Show($"下載時發生錯誤: \n{ex.Message}\n\n\n請稍後重試", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
                Close();
                // 重置按鈕狀態
                button1.Text = "啟動遊戲/下載遊戲";
                button1.Enabled = true;

            }
        }



        private async Task DownloadFileAsync(WebClient webClient, string url, string destination)
        {
            using (var stream = await webClient.OpenReadTaskAsync(new Uri(url)))
            using (var fileStream = new System.IO.FileStream(destination, System.IO.FileMode.Create))
            {
                byte[] buffer = new byte[8192];
                int bytesRead;
                long totalBytesRead = 0;
                long totalBytes = webClient.ResponseHeaders["Content-Length"] != null ? long.Parse(webClient.ResponseHeaders["Content-Length"]) : 0;

                DateTime startTime = DateTime.Now;
                DateTime lastUpdateTime = DateTime.Now;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (dowmloading == true)
                    {
                        totalBytesRead += bytesRead;

                        // 每次迴圈開頭即時更新進度條
                        int progressPercentage = (int)((double)totalBytesRead / totalBytes * 100);
                        UpdateProgressBar(progressPercentage);

                        // 每秒更新一次預估剩餘時間
                        DateTime currentTime = DateTime.Now;
                        if ((currentTime - lastUpdateTime).TotalMilliseconds >= 700)
                        {
                            lastUpdateTime = currentTime;

                            // 計算剩餘時間
                            TimeSpan elapsedTime = currentTime - startTime;
                            TimeSpan remainingTime = TimeSpan.FromSeconds((totalBytes - totalBytesRead) / (totalBytesRead / elapsedTime.TotalSeconds));

                            label1.Text = $"預估剩餘時間: {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                        }

                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
        }






        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        private void WebClient_DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            // 在此可以處理下載進度，例如更新 ProgressBar1
            int progressPercentage = e.ProgressPercentage;
            UpdateProgressBar(progressPercentage);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Runi()
        {
            if (IsAdmin())
            {
                Console.WriteLine("已以管理员权限运行。");

            }
            else
            {
                RunAsAdmin();
                Hide();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            dowmloading = false;
            progressBar1.Value = 0;
            label1.Text = "預估剩餘時間:";
            button1.Enabled = true;
            button2.Enabled = false;
            button1.Text = "啟動遊戲/下載遊戲";
            InitializeComponent();
        
        }

        private void reload_Tick(object sender, EventArgs e)
        {
            if (dowmloading == true)
            {
                button2.Enabled = true;
                button1.Enabled = false;

            }
            else if(dowmloading == false)
            {
                progressBar1.Value = 0;
                label1.Text = "預估剩餘時間:";
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else
            {
                dowmloading = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            Runi();
        }
    }
}
