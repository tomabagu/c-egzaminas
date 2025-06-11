using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;

namespace Agent
{
    internal class Program
    {
        static string pipeName = "agent2";
        static string directoryPath;
        static void Main(string[] args)
        {

            // Set CPU affinity to core 2
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x2;

            //Gija duomenų siuntimui
            Thread senderThread = new Thread(SendData);

            // paleidžiame gijas
            senderThread.Start();

            // laukiam, kol gijos baigs darbą
            senderThread.Join();
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
    }
}