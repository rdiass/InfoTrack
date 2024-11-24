using System.Text.Json.Serialization;

namespace InfoTrack.Contracts.ResultPattern;


/// <summary>
/// Represents a result with a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public sealed class Result<TValue>
{
    private readonly TValue? _value;
    public bool IsSuccess { get; }

    private Result(TValue value)
    {
        IsSuccess = true;
        Error = default;
        _value = value;
    }
    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
        _value = default;
    }
        
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; }

    /// <summary>
    /// Gets the value of the result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is not successful.</exception>
    public TValue Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a <see cref="Result{TValue}"/> with a failed result.
    /// </summary>
    /// <param name="error">The error.</param>
    public static implicit operator Result<TValue>(Error error) =>
        new(error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/> with a successful result.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Result<TValue>(TValue value) =>
        new(value);

    /// <summary>
    /// Creates a new <see cref="Result{TValue}"/> with a successful result and a value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static Result<TValue> Success(TValue value) =>
        new(value);
}
