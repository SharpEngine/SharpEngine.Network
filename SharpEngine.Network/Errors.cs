namespace SharpEngine.Network;

/// <summary>
/// Exception which be thrown when Packet is unknown
/// </summary>
public class UnknownPacketException : Exception
{
    /// <summary>
    /// Create Unknown Packet Exception
    /// </summary>
    /// <param name="message">Exception Message</param>
    public UnknownPacketException(string? message)
        : base(message) { }
}

/// <summary>
/// Exception which be thrown when Type of Property is unknown
/// </summary>
public class UnknownPropertyTypeException : Exception
{
    /// <summary>
    /// Create Unknown Property Type Exception
    /// </summary>
    /// <param name="message">Exception Message</param>
    public UnknownPropertyTypeException(string? message)
        : base(message) { }
}

/// <summary>
/// Exception which be thrown when Type of Field is unknown
/// </summary>
public class UnknownFieldTypeException : Exception
{
    /// <summary>
    /// Create Unknown Field Type Exception
    /// </summary>
    /// <param name="message">Exception Message</param>
    public UnknownFieldTypeException(string? message)
        : base(message) { }
}

/// <summary>
/// Exception which be thrown when Property is unknown
/// </summary>
public class UnknownPropertyException : Exception
{
    /// <summary>
    /// Create Unknown Property Exception
    /// </summary>
    /// <param name="message">Exception Message</param>
    public UnknownPropertyException(string? message)
        : base(message) { }
}

/// <summary>
/// Exception which be thrown when Field is unknown
/// </summary>
public class UnknownFieldException : Exception
{
    /// <summary>
    /// Create Unknown Type Exception
    /// </summary>
    /// <param name="message">Exception Message</param>
    public UnknownFieldException(string? message)
        : base(message) { }
}
