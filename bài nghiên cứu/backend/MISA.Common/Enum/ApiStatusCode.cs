namespace MISA.Common.Enum;

public enum ApiStatusCode : int
{
    Success = 200,
    BadRequest = 400,
    Created = 201,
    NoContent = 204,
    NotFound = 404,
    UnAuthorized = 401,
    Forbidden = 403,
    InternalServerError = 500,
}