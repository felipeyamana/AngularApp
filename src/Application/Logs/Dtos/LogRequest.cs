namespace Application.Logs.Dtos
{
    public class LogRequest
    {
        public bool? Success { get; set; }
        public int? Page { get; set; }
        public readonly int PageSize = 20;
    }
}
