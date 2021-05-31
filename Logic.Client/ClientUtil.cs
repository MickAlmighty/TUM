namespace Logic.Client
{
    public static class ClientUtil
    {
        public static string CreateConnectionString(string remoteAddress, uint port)
        {
            return $"ws://{remoteAddress}:{port}";
        }

        public static string CreateLocalConnectionString(uint port)
        {
            return CreateConnectionString("localhost", port);
        }
    }
}
