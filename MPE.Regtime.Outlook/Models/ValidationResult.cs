namespace MPE.Regtime.Outlook.App.Models
{
    internal class ValidationResult<T>
    {
        public T Object { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}