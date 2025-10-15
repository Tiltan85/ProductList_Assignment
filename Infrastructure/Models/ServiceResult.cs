
namespace Infrastructure.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }
    public List<InputError> FieldErrors { get; set; } = [];
}
