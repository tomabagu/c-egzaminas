using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;

namespace Master
{
    internal class Program
    {
        static ConcurrentDictionary<string, int> wordIndex = new ConcurrentDictionary<string, int>();

        static void Main(string[] args)
        {
            // Nustatome procesoriaus branduolio, kuriame veiks ši programa, priskyrimą
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4;

            //Gija duomenų gavimo iš agentų
            Thread t1 = new Thread(() => Listen("agent1"));
            Thread t2 = new Thread(() => Listen("agent2"));

            // paleidžiame gijas
            t1.Start();
            t2.Start();

            // laukiame, kol gijos baigs darbą
            t1.Join();
            t2.Join();

            // Išvedame žodžių dažnius
            foreach (var entry in wordIndex)
            {
                Console.WriteLine(entry.Key + ":" + entry.Value);
            }
            Console.ReadKey();

        }

        static void Listen(string pipeName)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);

            Console.WriteLine($"Waiting for connection on pipe: {pipeName}");

            pipeServer.WaitForConnection();
            using var reader = new StreamReader(pipeServer);
            Console.WriteLine($"Connected to agent on pipe: {pipeName}");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "EOF")
                {
                    break;
                }

                var parts = line.Split(':');
                if (parts.Length == 3)
                {
                    string key = $"{parts[0]}:{parts[1]}";
                    int count = int.Parse(parts[2]);
                    wordIndex.AddOrUpdate(key, count, (_, old) => old + count);
                }
            }
        }
    }
}