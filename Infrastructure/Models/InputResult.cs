
namespace Infrastructure.Models;

public class InputResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }
    public List<InputError> FieldErrors { get; set; } = [];
}

public class InputError
{
    public string? Field { get; set; }
    public string? Message { get; set; }
}
