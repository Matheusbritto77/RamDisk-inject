using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Renci.SshNet;
using System.Net;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Management;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;



namespace iBoy_RAM
{
    public partial class iBoy : Form
    {

        private Process _gasterProcess;
        private Process _sshconect;
        private System.Windows.Forms.Timer _timer;




        public iBoy()
        {
            InitializeComponent();
            Empty_text();


            // Inicializa o Timer
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 8000; // Define o intervalo do temporizador para 8 segundos
            _timer.Tick += Timer_Tick;

        }







        private void check_device(object sender, EventArgs e)
        {
            Empty_text();
            getDeviceInfo();
        }

        string path = Environment.CurrentDirectory;

        public void Empty_text()
        {
            txt_ECID.Text = "";
            txt_mode.Text = "";
            txt_CPID.Text = "";
            txt_model.Text = "";
            txt_status.Text = "";
            txt_Type.Text = "";
            progressBar1.Value = 0;
            txt_info.Text = "";
        }

        private bool getDeviceInfo(string argument = @"")
        {
            CheckForIllegalCrossThreadCalls = false;
            txt_info.Text = "Checking Device in DFU Mode";

            var ideviceinfo = new Process();
            ideviceinfo.StartInfo.FileName = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe"; // Altere o caminho conforme necessário
            ideviceinfo.StartInfo.Arguments = "-q";
            ideviceinfo.StartInfo.UseShellExecute = false;
            ideviceinfo.StartInfo.RedirectStandardOutput = true;
            ideviceinfo.StartInfo.CreateNoWindow = true;
            ideviceinfo.StartInfo.RedirectStandardError = true;
            ideviceinfo.Start();
            progressBar1.Value = 100;

            var error = ideviceinfo.StandardError.ReadToEnd();
            if (error.Length > 0)
            {
                txt_info.Text = "No Device Detected, Please check Driver/Cable";
                return false;
            }



            var lines = 0;

            while (!ideviceinfo.StandardOutput.EndOfStream)
            {
                lines++;
                string line = ideviceinfo.StandardOutput.ReadLine();
                var text2 = line.Replace("\r", "");

                if (text2.StartsWith("ECID: "))
                {
                    var ECID = text2.Replace("ECID: ", "");
                    txt_ECID.Text = ECID;
                }
                else if (text2.StartsWith("MODEL: "))
                {
                    var MODEL = text2.Replace("MODEL: ", "");
                    txt_CPID.Text = MODEL;
                }
                else if (text2.StartsWith("PRODUCT: "))
                {
                    var productType = text2.Replace("PRODUCT: ", "");
                    txt_Type.Text = productType;
                }
                else if (text2.StartsWith("MODE: "))
                {
                    var mode = text2.Replace("MODE: ", "");
                    txt_mode.Text = mode;
                    if (mode.ToLower() == "pwndfu") // Verifica se o dispositivo está em modo pwned DFU
                    {
                        txt_info.Text = txt_Type.Text + " Connected in " + txt_mode.Text + " Mode";
                    }
                }
                else if (text2.StartsWith("PWND: "))
                {
                    var status = text2.Replace("PWND: ", "");
                    txt_status.Text = status;
                }

                else if (text2.StartsWith("NAME: "))
                {
                    var NAME = text2.Replace("NAME: ", "");
                    txt_model.Text = NAME;
                }
            }
            if (lines <= 2) return false;



            return true;
        }


        public void SSH_check()
        {

        }

        public void Backup_act()
        {

        }

        public void Restore_act()
        {

        }






        public async void btn_boot1_Click(object sender, EventArgs e)
        {
            var commands = new List<string>
            {
                // Lista de comandos
                "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\iboot.img4\"",
                "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c go\""
            };

            progressBar1.Value = 10;
            txt_info.Text = "booting...";

            // Execute o comando de iboot duas vezes
            for (int i = 0; i < 2; i++)
            {
                await Task.Run(() => ExecutarComandoSilencioso(commands[0]));
                progressBar1.Value += 20;
            }

            // Execute o comando de go
            await Task.Run(() => ExecutarComandoSilencioso(commands[1]));
            progressBar1.Value += 20;

            txt_info.Text = "Verificando dispositivo Apple Mobile...";
            progressBar1.Value = 50;

            await Task.Run(() => VerificarDispositivoAppleMobile());

            txt_info.Text = "booting ramdisk completo";
            progressBar1.Value = 100;

            
        }

        private void ExecutarComandoSilencioso(string comando)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + comando)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }






        async void VerificarDispositivoAppleMobile()




        {
            string installFilterPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\install-filter.exe";

            string lsusbPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\lsusb.exe";

            progressBar1.Value = 60;
            txt_info.Text = "boting....";


            string lsusbOutput = await ExecuteCommandAsync(lsusbPath);

            if (lsusbOutput == null)
            {
                txt_info.Text = "Error executing lsusb.";
                return;
            }

            progressBar1.Value = 70;
            txt_info.Text = "boting....";

            string pattern = @"VendorID:(\w+) ProductID:(\w+).*""Apple Inc.""";
            Match match = Regex.Match(lsusbOutput, pattern);

            string vendorId = match.Success ? match.Groups[1].Value : null;
            string productId = match.Success ? match.Groups[2].Value : null;




            progressBar1.Value = 70;
            txt_info.Text = "boting....";

            if (vendorId != null && productId != null)
            {
                string[] filterArguments = new string[]
                {
                               "install",
                                $"--device={vendorId}:{productId}",
                                @"--inf=C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\USB\x64\usbaapl64.inf"
                };
                progressBar1.Value = 80;
                txt_info.Text = "Starting USB filter booting ramdisk......";
                string filterResult = await ExecuteCommandAsAdminAsync(installFilterPath, filterArguments);

                if (filterResult == null)
                {

                }

                else
                {
                    progressBar1.Value = 90;
                    txt_info.Text = "Driver revertido com sucesso booting ramdisk...";
                    Boot();

                }
            }
        }





        void Boot()
        {
            var commandList = new List<string>
        {
            // Lista de comandos com palavras variadas
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\iboot.img4\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c go\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\devicetree.img4\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c devicetree\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\ramdisk.img4\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c ramdisk\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\rdtrust.img4\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c firmware\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -f C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\HXQ6XBKX\\kernel.img4\"",
            "\"C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\irecovery.exe -c bootx\""
        };
            progressBar1.Value = 90;
            txt_info.Text = "boting....";

            int totalCommandss = commandList.Count;
            int currentCommandNumber = 0;

            foreach (var cmd in commandList)
            {
                currentCommandNumber++;
                ExecutarComandoo(cmd, currentCommandNumber, totalCommandss);
            }
        }

        void ExecutarComandoo(string cmd, int cmdNumber, int totalCmds)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmd}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();



                    process.WaitForExit();

                }

                // Check if all commands have been executed
                if (cmdNumber == totalCmds)
                {

                    {
                        progressBar1.Value = 100;
                        txt_info.Text = "booting ramdisk completo";
                        main();
                        txt_info.Text = "iniciando ssh conection";


                    }
                }
            }
            catch (Exception ex)

            {

            }
        }
    
            
        


        static void main()
        {
            string iproxyPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\iproxy.exe";
            string sshpassPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\sshpass.exe";

            // Iniciar o iproxy na porta 2222:22
            Process iproxyProcess = new Process();
            iproxyProcess.StartInfo.FileName = "cmd.exe";
            iproxyProcess.StartInfo.Arguments = $"/K \"{iproxyPath} 2222:22\"";
            iproxyProcess.StartInfo.UseShellExecute = true;
            iproxyProcess.Start();


            // Aguardar um pouco para garantir que o iproxy esteja em execução antes de iniciar o ssh
            System.Threading.Thread.Sleep(7000); // 2 segundos


            // Conectar via SSH usando o sshpass e a senha alpine
            Process sshProcess = new Process();
            sshProcess.StartInfo.FileName = "cmd.exe";
            sshProcess.StartInfo.Arguments = $"/K \"{sshpassPath} -p 'alpine'  ssh -p 2222 root@localhost\"";
            sshProcess.StartInfo.UseShellExecute = true;
            sshProcess.Start();

        }
    







             





                   
                    
    















        private void btn_backup_Click(object sender, EventArgs e)
        {
            // Realize a ação correspondente ao clique do botão aqui
        }

        private void txt_status_Click(object sender, EventArgs e)
        {



        }


        private void label1_Click(object sender, EventArgs e)
        {
            RunAsAdminAndExecute();
        }

        private async void RunAsAdminAndExecute()
        {
            progressBar1.Value = 0;
            txt_info.Text = "Checking for administrator privileges...";

            if (!IsUserAnAdmin())
            {
                txt_info.Text = "Restarting with administrator privileges...";
                RestartAsAdmin();
                return;
            }

            progressBar1.Value = 10;
            txt_info.Text = "Executing lsusb to find VendorID and ProductID...";

            string lsusbPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\lsusb.exe";
            string installFilterPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\install-filter.exe";
            string gasterPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\gaster\gaster0.exe";

            string lsusbOutput = await ExecuteCommandAsync(lsusbPath);

            if (lsusbOutput == null)
            {
                txt_info.Text = "Error executing lsusb.";
                return;
            }

            progressBar1.Value = 30;
            txt_info.Text = "Parsing lsusb output...";

            string pattern = @"VendorID:(\w+) ProductID:(\w+).*""Apple Inc.""";
            Match match = Regex.Match(lsusbOutput, pattern);

            string vendorId = match.Success ? match.Groups[1].Value : null;
            string productId = match.Success ? match.Groups[2].Value : null;

            if (vendorId != null)
            {
                txt_info.Text = "VendorID: " + vendorId;
            }
            else
            {
                txt_info.Text = "VendorID not found.";
            }

            if (productId != null)
            {
                txt_info.Text = "ProductID: " + productId;
            }
            else
            {
                txt_info.Text = "ProductID not found.";
            }

            progressBar1.Value = 50;

            if (vendorId != null && productId != null)
            {
                string[] filterArguments = new string[]
                {
                    "install",
                    $"--device={vendorId}:{productId}",
                    @"--inf=C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\SkynetUltra.inf"
                };

                txt_info.Text = "Starting USB filter...";
                string filterResult = await ExecuteCommandAsAdminAsync(installFilterPath, filterArguments);

                if (filterResult == null)
                {
                    txt_info.Text = "Error installing USB filter.";
                }
                else
                {
                    progressBar1.Value = 70;
                    txt_info.Text = "Executing gaster0.exe with the 'pwn' command...";
                    ExecuteGasterProcess(gasterPath);
                }
            }
            else
            {
                txt_info.Text = "Unable to install USB filter. VendorID or ProductID not found.";
            }

            
        }

        private bool IsUserAnAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RestartAsAdmin()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = System.Windows.Forms.Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Process.Start(processInfo);
            }
            catch (Exception)
            {
                // User refused the elevation
            }
            System.Windows.Forms.Application.Exit();
        }

        private async Task<string> ExecuteCommandAsync(string command, string arguments = "")
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = await Task.Run(() => process.StandardOutput.ReadToEnd());
                await Task.Run(() => process.WaitForExit());
                return output;
            }
            catch (Exception ex)
            {
                txt_info.Text = "Error executing command: " + ex.Message;
                return null;
            }
        }

        private async Task<string> ExecuteCommandAsAdminAsync(string command, string[] arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command} {string.Join(" ", arguments)}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };
                process.Start();
                string output = await Task.Run(() => process.StandardOutput.ReadToEnd());
                await Task.Run(() => process.WaitForExit());
                return output;
            }
            catch (Exception ex)
            {
                txt_info.Text = "Error executing command as admin: " + ex.Message;
                return null;
            }
        }


        // Função para abrir o processo Gaster0.exe
        private void ExecuteGasterProcess(string gasterPath)
        {
            try
            {
                _gasterProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gasterPath,
                        Arguments = "pwn", // Adicionando o argumento 'pwn'
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                // Inicia o processo
                _gasterProcess.Start();

                // Inicia o temporizador
                StartTimer(_gasterProcess);
            }
            catch (Exception ex)
            {
                // Exibe mensagem de erro se ocorrer uma exceção ao iniciar o processo
                MessageBox.Show("Erro ao iniciar o processo: " + ex.Message);
            }
        }

        private void StartTimer(Process process)
        {
            try
            {
                // Configuração do Timer
                _timer.Tag = process; // Salva a referência ao processo no Tag do Timer
                _timer.Start();
            }
            catch (Exception ex)
            {
                // Exibe mensagem de erro se ocorrer uma exceção ao iniciar o temporizador
                MessageBox.Show("Erro ao iniciar o temporizador: " + ex.Message);
            }
        }

        // Manipulador de evento para o temporizador
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string processName = "gaster0";

                // Obter todos os processos com o nome especificado
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process process in processes)
                {
                    try
                    {
                        // Tente fechar a janela principal do processo
                        if (!process.CloseMainWindow())
                        {
                            // Se a janela principal não puder ser fechada, tente fechar o processo diretamente
                            process.Kill();
                            txt_info.Text = "PWNDFU successful, remove and connect the cable and check the device.";
                            progressBar1.Value = 100;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao tentar fechar o processo: " + ex.Message);
                    }
                    finally
                    {
                        // Para o temporizador após tentar fechar o processo
                        _timer.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                // Exibe mensagem de erro se ocorrer uma exceção
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

    
            
        







        private async void btn_fixdrivers_Click(object sender, EventArgs e)

        {
            progressBar1.Value = 10;
            txt_info.Text = "Executing lsusb to find VendorID and ProductID...";

            string lsusbPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\lsusb.exe";
            string installFilterPath = @"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\LibusbK\install-filter.exe";


            string lsusbOutput = await ExecuteCommandAsync(lsusbPath);

            if (lsusbOutput == null)
            {
                txt_info.Text = "Error executing lsusb.";
                return;
            }

            progressBar1.Value = 30;
            txt_info.Text = "Parsing lsusb output...";

            string pattern = @"VendorID:(\w+) ProductID:(\w+).*""Apple Inc.""";
            Match match = Regex.Match(lsusbOutput, pattern);

            string vendorId = match.Success ? match.Groups[1].Value : null;
            string productId = match.Success ? match.Groups[2].Value : null;

            if (vendorId != null)
            {
                txt_info.Text = "VendorID: " + vendorId;
            }
            else
            {
                txt_info.Text = "VendorID not found.";
            }

            if (productId != null)
            {
                txt_info.Text = "ProductID: " + productId;
            }
            else
            {
                txt_info.Text = "ProductID not found.";
            }

            progressBar1.Value = 50;

            if (vendorId != null && productId != null)
            {
                string[] filterArguments = new string[]
                {
                    "install",
                    $"--device={vendorId}:{productId}",
                    @"--inf=C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\Drivers\USB\x64\usbaapl64.inf"
                };

                txt_info.Text = "Starting USB filter...";
                string filterResult = await ExecuteCommandAsAdminAsync(installFilterPath, filterArguments);

                if (filterResult == null)
                {
                    txt_info.Text = "Error installing USB filter.";


                }

                {
                    progressBar1.Value = 100;
                    txt_info.Text = "Driver revertido com sucesso";
                }

                {

                    

                }

            }
        }








    }
}

        

        
    

       