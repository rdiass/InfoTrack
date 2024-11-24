namespace InfoTrack.Contracts.ResultPattern;

/// <summary>
/// Represents an error in the application.
/// </summary>
public class Error
{
    private Error(
        string code,
        string description,
        ErrorType errorType
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType ErrorType { get; }

    /// <summary>
    /// Creates a new instance of <see cref="Error"/> with the specified code and description, representing a conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A new instance of <see cref="Error"/>.</returns>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates a new instance of <see cref="Error"/> with the specified code and description, representing an access unauthorized error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A new instance of <see cref="Error"/>.</returns>
    public static Error AccessUnAuthorized(string code, string description) =>
        new(code, description, ErrorType.AccessUnAuthorized);
}
