using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    // Метод вызывается автоматически, когда объект успешно спавнится в сети
    public override void OnNetworkSpawn()
    {
        // Если это не наш персонаж (а другого игрока), выключаем ему камеру или локальную логику
        if (!IsOwner)
        {
            // Например, здесь можно отключить локальный AudioListener или скрипты инпута,
            // чтобы они не конфликтовали с вашими.
            return;
        }
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