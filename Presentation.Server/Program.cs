using System;
using System.Threading.Tasks;

using Logic.File;

namespace Presentation.Server
{
    internal class Program
    {
        private static void Main()
        {
            Task serverTask;
            using (Server server = new Server(new FileRepository()))
            {
                serverTask = Task.Run(server.RunServer);
                string input;
                do
                {
                    input = Console.ReadLine();
                } while (input?.ToLowerInvariant() != "stop");
            }
            serverTask.Wait();
        }
    }
}
