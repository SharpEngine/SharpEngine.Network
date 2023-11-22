namespace SharpEngine.Network;

/// <summary>
/// Exception which be thrown when Packet is unknown
/// </summary>
/// <param name="message">Exception Message</param>
public class UnknownPacketException(string? message) : Exception(message)
{
}

/// <summary>
/// Exception which be thrown when Type of Property is unknown
/// </summary>
/// <param name="message">Exception Message</param>
public class UnknownPropertyTypeException(string? message) : Exception(message)
{
}

/// <summary>
/// Exception which be thrown when Type of Field is unknown
/// </summary>
/// <param name="message">Exception Message</param>
public class UnknownFieldTypeException(string? message) : Exception(message)
{
}

/// <summary>
/// Exception which be thrown when Property is unknown
/// </summary>
/// <param name="message">Exception Message</param>
public class UnknownPropertyException(string? message) : Exception(message)
{
}

/// <summary>
/// Exception which be thrown when Field is unknown
/// </summary>
/// <param name="message">Exception Message</param>
public class UnknownFieldException(string? message) : Exception(message)
{
}
