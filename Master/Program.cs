using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;

namespace Master
{
    internal class Program
    {

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

        }

        static void Listen(string pipeName)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);

            Console.WriteLine($"Waiting for connection on pipe: {pipeName}");

            pipeServer.WaitForConnection();
            using var reader = new StreamReader(pipeServer);
            Console.WriteLine($"Connected to agent on pipe: {pipeName}");
            Console.WriteLine(reader.ReadLine());
            Console.ReadKey();

        }
    }
}