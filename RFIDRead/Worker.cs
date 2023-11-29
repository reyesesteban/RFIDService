using RFIDRead.Dtos;
using System.IO.Ports;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RFIDRead
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static HttpClient client = new HttpClient();


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                SerialPort port = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                port.Open();
                Console.WriteLine("Presione cualquier tecla para salir.");
                Console.ReadKey();
                port.Close();
                await Task.Delay(5000, stoppingToken);

            //}
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine(getIdCard(indata));
            sp.DiscardInBuffer();
        }

        private static string getIdCard(string data)
        {
            try
            {
                return data.Replace(" ", "").Replace("\r","").Replace("\n","");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private bool SendIdCard(TagReader card)
        {
            try
            {
                using StringContent jsonContent = new(
                        JsonSerializer.Serialize(card),
                        Encoding.UTF8,
                        "application/json");

                var responce = client.PostAsync("api/algo", jsonContent).Result;
                if (responce.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace + " --> " +e.Message);
                return false;
            }


        }
    }
}