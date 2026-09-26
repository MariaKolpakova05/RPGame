using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private AudioService audioServicePrefab;
    [SerializeField] private ScoreManager scoreManagerPrefab;

    private static GameBootstrapper _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterServices();
    }

    private void RegisterServices()
    {
        // Audio
        if (!ServiceLocator.TryGet<IAudioService>(out _))
        {
            var audio = Instantiate(audioServicePrefab);
            DontDestroyOnLoad(audio.gameObject);
            ServiceLocator.Register<IAudioService>(audio);
        }

        // Score
        if (!ServiceLocator.TryGet<ScoreManager>(out _))
        {
            var score = Instantiate(scoreManagerPrefab);
            DontDestroyOnLoad(score.gameObject);
            ServiceLocator.Register(score);
        }

        // Save
        if (!ServiceLocator.TryGet<SaveInteractor>(out _))
        {
            var interactor = new SaveInteractor(
                new JsonPlayerRepository(),
                new JsonEnemyRepository()
            );
            ServiceLocator.Register(interactor);
        }
    }
}