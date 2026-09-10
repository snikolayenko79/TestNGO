using UnityEngine;
using Unity.Netcode;

public interface  INetworkColorable
{
    NetworkVariable<Color> NetColor { get; }
}
