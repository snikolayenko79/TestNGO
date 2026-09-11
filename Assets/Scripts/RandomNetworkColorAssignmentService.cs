using UnityEngine;

public class RandomNetworkColorAssignmentService : INetworkColorAssigmentService
{
    public void AssignColor(INetworkColorable colorableTarget)
    {
        if (colorableTarget == null) return;

        // Генерируем случайный цвет
        Color randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        // Назначаем его через интерфейс (изменение запишется в NetworkVariable на сервере)
        colorableTarget.NetColor.Value = randomColor;
    }
}