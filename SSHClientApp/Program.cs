using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSHClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = ConfigurationManager.AppSettings["Host"].ToString();
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            string userName = ConfigurationManager.AppSettings["UserName"].ToString();
            string password = ConfigurationManager.AppSettings["Password"].ToString();

            try
            {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(host, port, userName, password);
                SshClient client = new SshClient(connectionInfo);
                client.Connect();
                if (client.IsConnected)
                {
                    Console.WriteLine($"Sunucuya SSH bağlantısı başarılı");

                    #region 1.Yöntem
                    SshCommand sshCommand = client.CreateCommand("pwd");
                    sshCommand.Execute();
                    if (sshCommand.ExitStatus == 0)
                    {
                        string commandResult = sshCommand.Result;
                        Console.WriteLine(commandResult);
                    }
                    else
                    {
                        Console.WriteLine(sshCommand.Error);
                    }

                    #endregion

                    #region 2.Yöntem

                    //SSH bağlantısı üzerinden yeni bir stream session oluşturulur.
                    ShellStream sshShellStreamForLinux = client.CreateShellStream("xterm", 80, 60, 800, 400, 65536);
                    //Session nesnesi üzerinden Writeline metodu ile sunucuya komut gönderilir.
                    sshShellStreamForLinux.WriteLine("pwd");

                    string resultCommand = string.Empty; //Komut cevabının atanacağı değişken

                    //Propmt değerlerine göre komutun bittiği yere kadar cevabın okunması işlemi
                    sshShellStreamForLinux.Expect(TimeSpan.FromSeconds(15), new ExpectAction("> ",
                       (output) => resultCommand = output),
                       new ExpectAction("]$ ",
                       (output) => resultCommand = output));

                    Console.WriteLine(resultCommand);


                    #endregion

                    client.Disconnect();
                }
                else
                {
                    Console.WriteLine($"Sunucuya SSH bağlantısı yapılamadı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
