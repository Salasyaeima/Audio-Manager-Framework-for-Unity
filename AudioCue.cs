using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAudioCue", menuName = "GameSeed/Audio/Audio Cue")]
public class AudioCue : ScriptableObject
{
    [Header("Data Suara")]
    [Tooltip("Masukkan satu atau beberapa variasi klip suara di sini. Sistem akan memilihnya secara acak agar suara tidak membosankan.")]
    public AudioClip[] clips;

    [Header("Pengaturan Routing")]
    [Tooltip("Grup Audio Mixer untuk klip ini (misal: SFX, BGM, atau UI).")]
    public AudioMixerGroup audioMixerGroup;

    [Header("Pengaturan Volume")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Pengaturan Pitch (Modulasi)")]
    [Tooltip("Centang ini agar suara seperti langkah kaki atau tembakan terdengar sedikit berbeda setiap kali diputar.")]
    public bool useRandomPitch = false;
    
    [Range(0.1f, 3f)]
    public float minPitch = 0.9f;
    
    [Range(0.1f, 3f)]
    public float maxPitch = 1.1f;

    /// <summary>
    /// Fungsi pembantu untuk mengambil klip acak dari array.
    /// </summary>
    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"[AudioCue] {name} tidak memiliki AudioClip yang terpasang!");
            return null;
        }

        return clips[Random.Range(0, clips.Length)];
    }

    /// <summary>
    /// Fungsi pembantu untuk mengkalkulasi pitch sebelum diputar.
    /// </summary>
    public float GetPitch()
    {
        if (useRandomPitch)
        {
            return Random.Range(minPitch, maxPitch);
        }
        return 1f; // Pitch standar
    }
}