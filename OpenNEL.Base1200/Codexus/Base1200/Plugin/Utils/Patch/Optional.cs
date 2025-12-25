using System;

namespace Codexus.Base1200.Plugin.Utils.Patch;

public class Optional<T>
{
	private readonly T? _value;

	private readonly bool _isPresent;

	private Optional(T value)
	{
		_value = value;
		_isPresent = true;
	}

	private Optional()
	{
		_value = default(T);
		_isPresent = false;
	}

	public static Optional<T> Of(T value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		return new Optional<T>(value);
	}

	public static Optional<T> Empty()
	{
		return new Optional<T>();
	}

	public bool IsPresent()
	{
		return _isPresent;
	}

	public T Get()
	{
		if (!_isPresent)
		{
			throw new InvalidOperationException("No value present");
		}
		return _value;
	}

	public T? OrElse(T? defaultValue)
	{
		if (!_isPresent)
		{
			return defaultValue;
		}
		return _value;
	}
}
