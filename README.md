🔊 Unity Modular Audio Framework
Framework Audio yang sangat ringan, modular, dan berbasis Event-Driven untuk Unity (dioptimalkan untuk Unity 6). Dirancang untuk menghindari spaghetti code dan Missing References Exception. Kamu bisa memindahkan keseluruhan folder sistem ini ke proyek game apa pun (Platformer, RPG, Shooter) tanpa harus mengubah satu baris kode pun.

✨ Fitur Utama
100% Decoupled (Terpisah): Objek yang memutar suara tidak perlu tahu apakah AudioManager ada di scene atau tidak. Jika manajer audio dihapus, game tidak akan crash.

Data-Driven Design: Menggunakan ScriptableObject untuk membungkus pengaturan audio (Klip, Volume, Pitch). Desain audio langsung dari Project Window tanpa menyentuh kode.


Anti-Repetition & Pitch Randomization: Mendukung variasi array klip suara dan modulasi pitch otomatis agar efek suara (seperti langkah kaki atau tembakan) tidak terdengar repetitif.


Zero Allocation Object Pooling: Menggunakan sistem UnityEngine.Pool bawaan Unity 6 untuk mengelola "speaker" (pengeras suara hantu) secara dinamis, sehingga performa tetap stabil tanpa Garbage Collection Spikes.

🏗️ Arsitektur Inti (The 4 Pillars)
Sistem ini berdiri di atas 4 pilar utama yang saling melengkapi:


AudioCue (ScriptableObject): Berfungsi sebagai file "Kaset Data" murni. Menyimpan array AudioClip, rentang Pitch acak, volume, dan pengaturan Audio Mixer Group.

AudioEventChannel (ScriptableObject): Berfungsi sebagai "Frekuensi Radio" / Event Bus. Skrip lain cukup mengirim sinyal ke saluran ini.

AudioEmitter (MonoBehaviour): Prefab pengeras suara kosong yang ditarik dari Object Pool. Ia memutar AudioCue dan otomatis mengembalikan dirinya sendiri ke gudang setelah suara selesai berdering.

AudioSystem (MonoBehaviour): "Otak Sentral" yang duduk di scene. Ia mendengarkan sinyal dari AudioEventChannel dan memerintahkan AudioEmitter untuk memutar suara.

🚀 Cara Penggunaan (Quick Start Guide)
1. Setup Data di Editor
Klik kanan di Project Window -> Create -> GameSeed -> Audio -> Audio Event Channel. Beri nama (misal: SFX_EventChannel).

Klik kanan -> Create -> GameSeed -> Audio -> Audio Cue. Beri nama (misal: SFX_PistolShoot).

Di Inspector SFX_PistolShoot, masukkan klip audio kamu, lalu centang Use Random Pitch agar suaranya bervariasi.

2. Setup Prefab & Manajer
Buat GameObject kosong bernama AudioEmitter, tambahkan komponen AudioEmitter.cs (otomatis menambahkan AudioSource), lalu jadikan Prefab dan hapus dari scene.

Buat GameObject kosong bernama AudioSystem di scene dan tambahkan komponen AudioSystem.cs.

Di Inspector AudioSystem:


Audio Emitter Prefab: Masukkan Prefab AudioEmitter yang dibuat di langkah 1.


SFX Event Channel: Masukkan SFX_EventChannel yang dibuat di langkah 1.

3. Memutar Suara lewat Kode
Untuk memutar suara dari skrip pemain, senjata, atau UI, kamu cukup mereferensikan Event Channel dan Audio Cue-nya.

Contoh Penggunaan pada Skrip Senjata (WeaponController.cs):

C#
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Saluran radio untuk SFX")]
    public AudioEventChannel sfxChannel;     // Referensi ke Pilar 2
    [Tooltip("Data suara tembakan senjata ini")]
    public AudioCue shootAudioCue;           // Referensi ke Pilar 1

    public void FireWeapon()
    {
        // Logika menembak peluru di sini...
        Debug.Log("Duar! Peluru Ditembakkan!");

        // --- PEMICU AUDIO ---
        // Cukup panggil event ini. Jika AudioSystem ada di scene, suara akan berbunyi di posisi pemain.
        // Jika AudioSystem tidak ada (misal saat testing), game tidak akan error!
        if (sfxChannel != null && shootAudioCue != null)
        {
            sfxChannel.RaiseEvent(shootAudioCue, transform.position);
        }
    }
}
🛠️ Alur Kerja Sistem (Under the Hood)
Skrip WeaponController memanggil sfxChannel.RaiseEvent(...) lalu melupakan prosesnya.


AudioSystem yang sedang mendengarkan saluran tersebut akan menangkap sinyal.


AudioSystem mengambil satu AudioEmitter menganggur dari Unity Object Pool.


AudioEmitter dipindahkan ke lokasi tembakan, memutar suara secara spasial, dan mengembalikan dirinya ke gudang Pool secara mandiri menggunakan IEnumerator setelah status isPlaying selesai.
