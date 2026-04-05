using System.ComponentModel.DataAnnotations;

namespace BL.Dto.Response;

public class ApiResponse
{
    [property: Required] public int Status { get; set; }
    public string? Message { get; set; }
    
    public Object? Data { get; set; }
}