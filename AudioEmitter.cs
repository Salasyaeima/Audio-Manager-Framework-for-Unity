using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    private AudioSource audioSource;
    private IObjectPool<AudioEmitter> pool;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Pastikan tidak memutar suara apa pun saat baru di-spawn
        audioSource.playOnAwake = false; 
    }

    /// <summary>
    /// Dipanggil oleh AudioManager untuk mendaftarkan emitter ini ke gudang pool-nya.
    /// </summary>
    public void Initialize(IObjectPool<AudioEmitter> poolReference)
    {
        pool = poolReference;
    }

    /// <summary>
    /// Memutar data AudioCue dan mengurus pengembalian ke Pool secara otomatis.
    /// </summary>
    public void PlayAudio(AudioCue cue, Vector3 position)
    {
        if (cue == null) return;

        AudioClip clipToPlay = cue.GetRandomClip();
        if (clipToPlay == null) return;

        // Atur posisi speaker
        transform.position = position;

        // Terapkan data dari AudioCue ke komponen AudioSource
        audioSource.clip = clipToPlay;
        audioSource.outputAudioMixerGroup = cue.audioMixerGroup;
        audioSource.volume = cue.volume;
        audioSource.pitch = cue.GetPitch();

        // Cerdas: Jika posisi (0,0,0) misal untuk UI/BGM, jadikan suara 2D murni. Jika ada kordinat, jadikan 3D spasial.
        audioSource.spatialBlend = (position == Vector3.zero) ? 0f : 1f;

        audioSource.Play();

        // Mulai pantau kapan suara selesai agar bisa kembali ke Pool
        StartCoroutine(ReturnToPoolRoutine());
    }

    private IEnumerator ReturnToPoolRoutine()
    {
        // Rahasia anti-bug: Kita menggunakan pengecekan isPlaying alih-alih WaitForSeconds.
        // Ini memastikan jika game masuk ke mode "Time Stop" dan suara di-pause sementara, 
        // speaker tidak akan hilang prematur!
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Setelah benar-benar selesai (atau dihentikan paksa), kembalikan ke gudang
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}