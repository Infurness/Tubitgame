using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    private bool isNewPlayer = true;
    private void Start()
    {
        signalBus.Subscribe<AssetsLoadedSignal>((signal =>
        {
            if (isNewPlayer)
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);//Tutorial
            else
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
        } ));

        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            isNewPlayer = signal.NewPlayer;
        }));
    }
}
