using UnityEngine;
using Unity.Netcode;

public class NetworkColorController : NetworkBehaviour, INetworkColorable
{
    private IObjectColorChangeService _objectColorChangeService;
    public IObjectColorChangeService ObjectColorChangeService
    {
        get
        {
            if (_objectColorChangeService == null)
                _objectColorChangeService = this.GetComponent<IObjectColorChangeService>();
            return _objectColorChangeService;
        }
    }
    
    public NetworkVariable<Color> NetColor { get; } = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    
    // Метод вызывается автоматически, когда объект успешно спавнится в сети
    public override void OnNetworkSpawn()
    {
        // 1. Подписываемся на событие изменения цвета. 
        // Когда сервер изменит переменную, этот метод выполнится у ВСЕХ клиентов для ЭТОГО куба.
        NetColor.OnValueChanged += OnColorChanged;

        // 2. Сразу красим куб в текущее значение (нужно для тех, кто зашел в игру позже)
        ObjectColorChangeService.ChangeColor(NetColor.Value);
    }

    public override void OnNetworkDespawn()
    {
        NetColor.OnValueChanged -= OnColorChanged;
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        ObjectColorChangeService.ChangeColor(newValue);
    }
}
