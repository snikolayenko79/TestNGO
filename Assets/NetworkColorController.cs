using UnityEngine;
using Unity.Netcode;

public class NetworkColorController : NetworkBehaviour, INetworkColorable
{
    public NetworkVariable<Color> NetColor { get; } = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    
    [SerializeField] private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }
    
    // Метод вызывается автоматически, когда объект успешно спавнится в сети
    public override void OnNetworkSpawn()
    {
        // 1. Подписываемся на событие изменения цвета. 
        // Когда сервер изменит переменную, этот метод выполнится у ВСЕХ клиентов для ЭТОГО куба.
        NetColor.OnValueChanged += OnColorChanged;

        // 2. Сразу красим куб в текущее значение (нужно для тех, кто зашел в игру позже)
        meshRenderer.material.color = NetColor.Value;
    }

    public override void OnNetworkDespawn()
    {
        NetColor.OnValueChanged -= OnColorChanged;
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        // На клиенте просто обновляем цвет материала
        meshRenderer.material.color = newValue;
    }
}
