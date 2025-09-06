namespace Application.Common.Results
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Value { get; private set; }
        public List<string> Errors { get; private set; } = new();

        private Result(bool success, T? value, List<string>? errors)
        {
            Success = success;
            Value = value;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> SuccessResult(T value) =>
            new(true, value, null);

        public static Result<T> Failure(params string[] errors) =>
            new(false, default, errors.ToList());

        public static Result<T> Failure(IEnumerable<string> errors) =>
            new(false, default, errors.ToList());
    }
}
