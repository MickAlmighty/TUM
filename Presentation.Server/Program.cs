using Logic.File;

namespace Presentation.Server {
    internal class Program {
        private static void Main(string[] args) {
            Server server = new Server(new FileRepository());
            server.RunServer().GetAwaiter().GetResult();
        }
    }
}
