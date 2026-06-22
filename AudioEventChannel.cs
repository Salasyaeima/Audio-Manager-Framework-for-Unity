using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewAudioEventChannel", menuName = "GameSeed/Audio/Audio Event Channel")]
public class AudioEventChannel : ScriptableObject
{
    [Header("Frekuensi Radio Audio")]
    [Tooltip("Event ini akan dipanggil ketika ada objek yang meminta suara diputar.")]
    public UnityAction<AudioCue, Vector3> OnAudioRequested;

    /// <summary>
    /// Panggil fungsi ini dari skrip pemain, musuh, atau UI untuk memutar suara.
    /// </summary>
    /// <param name="audioCue">Data suara yang ingin diputar.</param>
    /// <param name="position">Kordinat lokasi suara (gunakan Vector3.zero untuk BGM/UI).</param>
    public void RaiseEvent(AudioCue audioCue, Vector3 position)
    {
        if (audioCue == null)
        {
            Debug.LogWarning($"[{name}] Permintaan audio ditolak karena AudioCue kosong!");
            return;
        }

        // Jika ada yang berlangganan (mendengarkan) frekuensi ini, pancarkan sinyalnya!
        if (OnAudioRequested != null)
        {
            OnAudioRequested.Invoke(audioCue, position);
        }
        else
        {
            // Peringatan ini sangat berguna untuk debugging jika kamu lupa menaruh AudioManager di scene
            Debug.LogWarning($"[{name}] Sinyal audio dipancarkan, tetapi tidak ada AudioManager yang mendengarkan!");
        }
    }
}