using UnityEngine;
using UnityEngine.Pool;

public class AudioSystem : MonoBehaviour
{
    [Header("Pengaturan Gudang Speaker (Pool)")]
    [Tooltip("Masukkan Prefab AudioEmitter yang sudah kita buat sebelumnya.")]
    [SerializeField] private AudioEmitter audioEmitterPrefab;
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private int maxPoolSize = 30;

    [Header("Saluran Frekuensi Radio")]
    [Tooltip("Saluran untuk mendengarkan permintaan efek suara (SFX).")]
    [SerializeField] private AudioEventChannel sfxEventChannel;
    
    // Nanti kita bisa menambahkan bgmEventChannel atau uiEventChannel di sini

    private IObjectPool<AudioEmitter> emitterPool;

    private void Awake()
    {
        // Menciptakan gudang Object Pool bawaan Unity 6
        emitterPool = new ObjectPool<AudioEmitter>(
            createFunc: CreateEmitter,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
    }

    private void OnEnable()
    {
        // Mulai mendengarkan saluran radio saat sistem aktif
        if (sfxEventChannel != null)
        {
            sfxEventChannel.OnAudioRequested += PlayRequestedAudio;
        }
    }

    private void OnDisable()
    {
        // BERHENTI mendengarkan jika sistem mati agar tidak Memory Leak
        if (sfxEventChannel != null)
        {
            sfxEventChannel.OnAudioRequested -= PlayRequestedAudio;
        }
    }

    /// <summary>
    /// Fungsi utama yang bereaksi secara otomatis saat event radio terpanggil.
    /// </summary>
    private void PlayRequestedAudio(AudioCue cue, Vector3 position)
    {
        if (cue == null) return;

        // Ambil speaker nganggur dari gudang
        AudioEmitter emitter = emitterPool.Get();
        
        // Perintahkan speaker memutar suara
        emitter.PlayAudio(cue, position);
    }

    #region Logika Object Pool Unity
    
    private AudioEmitter CreateEmitter()
    {
        AudioEmitter emitter = Instantiate(audioEmitterPrefab, transform);
        emitter.Initialize(emitterPool);
        return emitter;
    }

    private void OnTakeFromPool(AudioEmitter emitter)
    {
        emitter.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(AudioEmitter emitter)
    {
        emitter.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(AudioEmitter emitter)
    {
        Destroy(emitter.gameObject);
    }
    
    #endregion
}