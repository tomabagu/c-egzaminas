using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;

namespace AgentB
{
    internal class Program
    {
        static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        static string pipeName = "agent2";
        static void Main(string[] args)
        {

            // Nustatome procesoriaus branduolio, kuriame veiks ši programa, priskyrimą
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x2;

            //Gija duomenų siuntimui
            Thread senderThread = new Thread(SendData);
            Thread readerThread = new Thread(ReadFiles);

            // paleidžiame gijas
            senderThread.Start();
            readerThread.Start();

            // laukiam, kol gijos baigs darbą
            senderThread.Join();
            readerThread.Join();
        }

        static void SendData()
        {
            // sukuriame NamedPipeClientStream, kuris prisijungs prie serverio
            using var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            // prisijungiame prie NamedPipeServerStream
            pipeClient.Connect();
            // sukuriame StreamWriter, kuris rašys į NamedPipe
            using var writer = new StreamWriter(pipeClient) { AutoFlush = true };

            writer.WriteLine("test message from agent 2");
        }

        static void ReadFiles()
        {
            // iteruojame per visus .txt failus kataloge "files/"
            foreach (var file in Directory.GetFiles("files/", "*.txt"))
            {
                var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                // skaitome failo turinį
                var lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    // padalijame eilutę į žodžius atskirtais tarpais
                    var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        if (!string.IsNullOrWhiteSpace(word))

                            // skaičiuojame žodžių dažnį
                            wordCounts[word] = wordCounts.GetValueOrDefault(word, 0) + 1;
                    }
                }

                foreach (var kvp in wordCounts)
                {
                    // į eilę įrašome žinutę su failo pavadinimu, žodžiu ir jo dažniu
                    messageQueue.Enqueue($"{Path.GetFileName(file)}:{kvp.Key}:{kvp.Value}");
                }
            }

            // į eilę įrašome "EOF" žinutę, kad signalizuotume, jog failų skaitymas baigtas
            messageQueue.Enqueue("EOF");
        }
    }
}