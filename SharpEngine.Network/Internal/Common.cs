using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;

namespace SharpEngine.Network.Internal;

internal static class Common
{ 
    public static dynamic ReadPacket(NetDataReader reader, string packetType, Type type)
    {
        var numberProp = reader.GetInt();
        var numberField = reader.GetInt();
        dynamic? packet = Activator.CreateInstance(type);
        try
        {
            if (packet == null)
                throw new UnknownPacketException($"Packet : {packetType}");
        }
        catch (RuntimeBinderException)
        {
            // do nothing
        }

        for (var i = 0; i < numberProp; i++)
        {
            var propName = reader.GetString();
            var prop = type.GetProperty(propName) ?? throw new UnknownPropertyException($"Property : {propName} - Packet : {packetType}");
            object packetObj = packet!;

            SetPropertyValue(packetObj, prop, reader);

            packet = packetObj;
        }

        for (var i = 0; i < numberField; i++)
        {
            var fieldName = reader.GetString();
            var field = type.GetField(fieldName) ?? throw new UnknownFieldException($"Field : {fieldName} - Packet : {packetType}");
            object packetObj = packet!;

            SetFieldValue(packetObj, field, reader);

            packet = packetObj;
        }

        return packet!;
    }

    public static void SendPacket<T>(NetPeer peer, T packet)
        where T : notnull
    {
        var writer = new NetDataWriter(true, 256);

        var properties = packet.GetType().GetProperties();
        var fields = packet.GetType().GetFields();

        writer.Put(packet.GetType().Name);
        writer.Put(properties.Length);
        writer.Put(fields.Length);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(packet, null);
            if (value == null)
                continue;

            writer.Put(prop.Name);

            PutPropertyValue(value, prop, writer);
            
        }
        foreach (var field in fields)
        {
            var value = field.GetValue(packet);
            if (value == null)
                continue;

            writer.Put(field.Name);

            PutFieldValue(value, field, writer);
        }
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    private static void PutPropertyValue(object value, PropertyInfo prop, NetDataWriter writer)
    {
        if (prop.PropertyType == typeof(string))
            writer.Put((string)value);
        else if (prop.PropertyType == typeof(int))
            writer.Put((int)value);
        else if (prop.PropertyType == typeof(short))
            writer.Put((short)value);
        else if (prop.PropertyType == typeof(byte))
            writer.Put((byte)value);
        else if (prop.PropertyType == typeof(char))
            writer.Put((char)value);
        else if (prop.PropertyType == typeof(float))
            writer.Put((float)value);
        else if (prop.PropertyType == typeof(double))
            writer.Put((double)value);
        else if (prop.PropertyType == typeof(bool))
            writer.Put((bool)value);
        else if (prop.PropertyType == typeof(string[]))
            writer.PutArray((string[])value);
        else if (prop.PropertyType == typeof(int[]))
            writer.PutArray((int[])value);
        else if (prop.PropertyType == typeof(short[]))
            writer.PutArray((short[])value);
        else if (prop.PropertyType == typeof(float[]))
            writer.PutArray((float[])value);
        else if (prop.PropertyType == typeof(double[]))
            writer.PutArray((double[])value);
        else if (prop.PropertyType == typeof(bool[]))
            writer.PutArray((bool[])value);
        else
            throw new UnknownPropertyTypeException($"Type : {prop.PropertyType.Name}");
    }

    private static void PutFieldValue(object value, FieldInfo field, NetDataWriter writer)
    {
        if (field.FieldType == typeof(string))
            writer.Put((string)value);
        else if (field.FieldType == typeof(int))
            writer.Put((int)value);
        else if (field.FieldType == typeof(short))
            writer.Put((short)value);
        else if (field.FieldType == typeof(byte))
            writer.Put((byte)value);
        else if (field.FieldType == typeof(char))
            writer.Put((char)value);
        else if (field.FieldType == typeof(float))
            writer.Put((float)value);
        else if (field.FieldType == typeof(double))
            writer.Put((double)value);
        else if (field.FieldType == typeof(bool))
            writer.Put((bool)value);
        else if (field.FieldType == typeof(string[]))
            writer.PutArray((string[])value);
        else if (field.FieldType == typeof(int[]))
            writer.PutArray((int[])value);
        else if (field.FieldType == typeof(short[]))
            writer.PutArray((short[])value);
        else if (field.FieldType == typeof(float[]))
            writer.PutArray((float[])value);
        else if (field.FieldType == typeof(double[]))
            writer.PutArray((double[])value);
        else if (field.FieldType == typeof(bool[]))
            writer.PutArray((bool[])value);
        else
            throw new UnknownFieldTypeException($"Type : {field.FieldType.Name}");
    }

    private static void SetPropertyValue(object packetObj, PropertyInfo prop, NetDataReader reader)
    {
        if (prop.PropertyType == typeof(string))
            prop.SetValue(packetObj, reader.GetString());
        else if (prop.PropertyType == typeof(int))
            prop.SetValue(packetObj, reader.GetInt());
        else if (prop.PropertyType == typeof(short))
            prop.SetValue(packetObj, reader.GetShort());
        else if (prop.PropertyType == typeof(byte))
            prop.SetValue(packetObj, reader.GetByte());
        else if (prop.PropertyType == typeof(char))
            prop.SetValue(packetObj, reader.GetChar());
        else if (prop.PropertyType == typeof(float))
            prop.SetValue(packetObj, reader.GetFloat());
        else if (prop.PropertyType == typeof(double))
            prop.SetValue(packetObj, reader.GetDouble());
        else if (prop.PropertyType == typeof(bool))
            prop.SetValue(packetObj, reader.GetBool());
        else if (prop.PropertyType == typeof(string[]))
            prop.SetValue(packetObj, reader.GetStringArray());
        else if (prop.PropertyType == typeof(int[]))
            prop.SetValue(packetObj, reader.GetIntArray());
        else if (prop.PropertyType == typeof(short[]))
            prop.SetValue(packetObj, reader.GetShortArray());
        else if (prop.PropertyType == typeof(float[]))
            prop.SetValue(packetObj, reader.GetFloatArray());
        else if (prop.PropertyType == typeof(double[]))
            prop.SetValue(packetObj, reader.GetDoubleArray());
        else if (prop.PropertyType == typeof(bool[]))
            prop.SetValue(packetObj, reader.GetBoolArray());
        else
            throw new UnknownPropertyTypeException($"Type : {prop.PropertyType.Name}");
    }

    private static void SetFieldValue(object packetObj, FieldInfo field, NetDataReader reader)
    {
        if (field.FieldType == typeof(string))
            field.SetValue(packetObj, reader.GetString());
        else if (field.FieldType == typeof(int))
            field.SetValue(packetObj, reader.GetInt());
        else if (field.FieldType == typeof(short))
            field.SetValue(packetObj, reader.GetShort());
        else if (field.FieldType == typeof(byte))
            field.SetValue(packetObj, reader.GetByte());
        else if (field.FieldType == typeof(char))
            field.SetValue(packetObj, reader.GetChar());
        else if (field.FieldType == typeof(float))
            field.SetValue(packetObj, reader.GetFloat());
        else if (field.FieldType == typeof(double))
            field.SetValue(packetObj, reader.GetDouble());
        else if (field.FieldType == typeof(bool))
            field.SetValue(packetObj, reader.GetBool());
        else if (field.FieldType == typeof(string[]))
            field.SetValue(packetObj, reader.GetStringArray());
        else if (field.FieldType == typeof(int[]))
            field.SetValue(packetObj, reader.GetIntArray());
        else if (field.FieldType == typeof(short[]))
            field.SetValue(packetObj, reader.GetShortArray());
        else if (field.FieldType == typeof(float[]))
            field.SetValue(packetObj, reader.GetFloatArray());
        else if (field.FieldType == typeof(double[]))
            field.SetValue(packetObj, reader.GetDoubleArray());
        else if (field.FieldType == typeof(bool[]))
            field.SetValue(packetObj, reader.GetBoolArray());
        else
            throw new UnknownFieldTypeException($"Type : {field.FieldType.Name}");
    }
}
