using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct NetworkTransformData : INetworkSerializable, System.IEquatable<NetworkTransformData>
{
    public NetworkTransformData(Vector3 vector)
    {
        Position = vector;
        Scale = vector;
        Rotation = vector;

        SceneSequenceNumber = 0;
    }

    public NetworkTransformData(Vector3 position, Vector3 scale, Vector3 rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;

        SceneSequenceNumber = 0;
    }

    public void SetSceneSequenceNumber(ushort value)
    {
        SceneSequenceNumber = value;
    }

    public Vector3 Position;
    public Vector3 Scale;       // localScale
    public Vector3 Rotation;

    public ushort SceneSequenceNumber;  // sender가 이 패킷을 보냈을 때 sender의 SceneSequenceNumber

    public static NetworkTransformData Zero = new NetworkTransformData(Vector3.zero);

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Position);
            reader.ReadValueSafe(out Scale);
            reader.ReadValueSafe(out Rotation);
            reader.ReadValueSafe(out SceneSequenceNumber);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Position);
            writer.WriteValueSafe(Scale);
            writer.WriteValueSafe(Rotation);
            writer.WriteValueSafe(SceneSequenceNumber);
        }
    }

    bool System.IEquatable<NetworkTransformData>.Equals(NetworkTransformData other)
    {
        return
            (Position == other.Position) &&
            (Scale == other.Scale) &&
            (Rotation == other.Rotation) &&
            (SceneSequenceNumber == other.SceneSequenceNumber);
    }
}