namespace MPE.Pinger.Models
{
    internal class Connection
    {
        public string Alias { get; set; }
        public string Target { get; set; }
        public int Port { get; set; }
        public string Type { get; set; }
        public int[] Response { get; set; }
    }
}