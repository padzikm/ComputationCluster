using System;
using System.IO;
using System.Windows.Forms;
using DvrpUtils;


namespace ComputationalClient
{
    class Program
    {
        private const String DefaultValuesInfo = "Default values used: \n\n" +
                                                 "Address IP:           localhost\n" +
                                                 "Port:                 12345\n" +
                                                 "Problem name:         DVRP\n" +
                                                 "Computation timeout:  100000\n";

        static string _addressIp;
        static int _port;
        static string _name;
        static ulong _timeout;
        static byte[] _problemBytes;
        static bool _registered;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            _registered = false;
            Client client = null;
       
            while (!_registered)
            {
                WriteLine(@"Do you want to use default values? (y/n)");

                if (Console.ReadLine() == "y")
                {
                    Console.WriteLine(DefaultValuesInfo);
                    try
                    {
                        ChooseFile();
                        client = new Client("localhost", 12345, "DVRP", 100000, _problemBytes);
                        _registered = true;
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(@"Error ocurred while creating client. Propably wrong path to file.");
                    }
                }

                try
                {
                    WriteLine(@"Type correct values:");

                    _registered = true;

                    ReadData();
                    client = new Client(_addressIp, _port, _name, _timeout, _problemBytes);
                }
                catch (Exception)
                {
                    _registered = false;
                }
            }

            WriteLine(@"Client created. Start working...");

            if (client == null) return;

            client.Start();

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }

            WriteLine(@"Client's work ended. Closing program.");

            client.Stop();
        }

        static void WriteLine(string content)
        {
            Console.WriteLine();
            Console.WriteLine(content);
        }

        static void ReadData()
        {
            try
            {
                Console.Write(@"IP Address:     ");
                _addressIp = Console.ReadLine();
                Console.Write(@"Port number:    ");
                _port = int.Parse(Console.ReadLine());
                Console.Write(@"Problem name:   ");
                _name = Console.ReadLine();
                Console.Write(@"Timeout:        ");
                _timeout = ulong.Parse(Console.ReadLine());
                ChooseFile();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error while reading input data... " + e.Message);
                _registered = false;
            }
        }

        static byte[] GetBytesFromVrp(string name)
        {
            var streamReader = new StreamReader(name);
            var problemString = streamReader.ReadToEnd();
            streamReader.Close();
            Console.WriteLine(_problemBytes);
            return DataSerialization.GetBytes(problemString);
        }

        static void ChooseFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = @"Select XML Document",
            };

            using (dialog)
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;
                _problemBytes = GetBytesFromVrp(dialog.FileName);
                Console.WriteLine(@"You have chosen {0} to solve", dialog.FileName);
            }
        }
    }
}
