using UnityEngine;

[CreateAssetMenu(fileName = "Service Container", menuName = "Service Container")]
public class ServiceContainerSO : ScriptableObject
{
    public static Color[] PlayerColors = new Color[]
    {
        new(0.2f, 0.5f, 1f),
        new(1f, 0.2f, 0.5f)
    };

    public MultiInputManager InputManager { get; set; }

    public PlayerInfoService PlayerInfoService { get; set; }

    public GameDirector GameDirector { get; set; }

    public EventManager EventManager { get; set; }
}