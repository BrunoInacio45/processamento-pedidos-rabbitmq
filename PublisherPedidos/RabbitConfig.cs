namespace RabbitLib
{
    public class RabbitConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public string NextQueue { get; set; }
    }
}
