using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;

/// <summary>
/// Helper methods to create instances of <see cref="Optional{T}"/>
/// </summary>
public static class Optional
{
    /// <summary>
    /// Create an <see cref="Optional{T}"/> with no value and invalid state.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>An <see cref="Optional{T}"/> with no value.</returns>
    public static Optional<T> FromNoValue<T>()
        => default;

    /// <summary>
    /// Create a <see cref="Optional{T}"/> from a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to assign as an optional.</param>
    /// <returns>An <see cref="Optional{T}"/> with a value.</returns>
    public static Optional<T> FromValue<T>(T value)
        => new(value);
}

public readonly struct Optional<T>
{
    public bool HasValue { get; init; }
    public T Value => HasValue ? _value : throw new InvalidCastException("Value is not set.");
    private readonly T _value;

    public Optional(T value)
    {
        HasValue = true;
        _value = value;
    }
}
