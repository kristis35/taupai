namespace SaskaituAgentas.Api.Services;

public enum ServiceError
{
    None,
    NotFound,
    Conflict,
    Validation,
}

/// <summary>Paslaugos sluoksnio rezultatas, kurį kontroleris paverčia HTTP atsakymu.</summary>
public record ServiceResult<T>(T? Value, ServiceError Error = ServiceError.None, string? Message = null)
{
    public bool IsSuccess => Error == ServiceError.None;

    public static ServiceResult<T> Ok(T value) => new(value);
    public static ServiceResult<T> Fail(ServiceError error, string message) => new(default, error, message);
}
