using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI; // Обязательный неймспейс для работы с UI-компонентами

public class NetworkMenuUI : MonoBehaviour
{
    // Ссылки на наши кнопки из инспектора
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        // Подписываемся на события клика программно (это надежнее, чем настраивать в инспекторе)
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            HideMenu();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            HideMenu();
        });
    }

    private void HideMenu()
    {
        // Скрываем меню после нажатия, чтобы оно не мешало игре
        gameObject.SetActive(false);
    }
}