using UnityEngine;

[CreateAssetMenu(fileName = "Service Container", menuName = "Service Container")]
public class ServiceContainerSO : ScriptableObject
{
    public MultiInputManager InputManager { get; set; }

    public SceneController SceneController { get; set; }

    public PlayerInfoService PlayerInfoService { get; set; }

    public GameDirector GameDirector { get; set; }

    public EventManager EventManager { get; set; }
}