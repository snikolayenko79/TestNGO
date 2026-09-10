using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private MeshRenderer meshRenderer;

    // Сделали переменную PUBLIC. Сервер по-прежнему единственный, кто имеет право в неё писать.
    public readonly NetworkVariable<Color> NetColor = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

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
        
        // Если это не наш персонаж (а другого игрока), выключаем ему камеру или локальную логику
        if (!IsOwner)
        {
            // Например, здесь можно отключить локальный AudioListener или скрипты инпута,
            // чтобы они не конфликтовали с вашими.
            return;
        }
    }

    public override void OnNetworkDespawn()
    {
        NetColor.OnValueChanged -= OnColorChanged;
    }
    
    // Метод генерации случайного цвета на сервере
    public void AssignRandomColor()
    {
        if (!IsServer) return;
        
        // Запись в Value автоматически запустит синхронизацию по сети
        NetColor.Value = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        // На клиенте просто обновляем цвет материала
        meshRenderer.material.color = newValue;
    }
    
    private void Update()
    {
        // Ключевая проверка для сетевой игры: 
        // Логика ввода должна работать ТОЛЬКО на том клиенте, который управляет этим персонажем.
        if (!IsOwner) return;

        // Считываем ввод игрока
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            // Перемещаем персонажа локально для мгновенного отклика (Client-Side Prediction)
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            // Отправляем запрос на сервер, чтобы он обновил нашу позицию для всех остальных
            SubmitPositionRequestServerRpc(transform.position);
        }
    }

    // ServerRpc означает: метод вызывается на клиенте, но выполняется исключительно на СЕРВЕРЕ.
    // Имя метода обязательно должно заканчиваться на ServerRpc.
    [ServerRpc]
    private void SubmitPositionRequestServerRpc(Vector3 newPosition)
    {
        // Здесь сервер может провести валидацию (проверить на читы, скорость движения, телепорты)
        // Если всё в порядке, сервер обновляет позицию объекта у себя.
        transform.position = newPosition;
    }
}