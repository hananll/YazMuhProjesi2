using UnityEngine;

public class SesYoneticisi : MonoBehaviour
{
    public static SesYoneticisi Ornek { get; private set; } // Singleton �rne�i

    [Tooltip("Sesleri �alacak olan AudioSource component'i")]
    public AudioSource sesKaynagi;

    [Tooltip("Butonlar�n �zerine gelindi�inde �alacak ses klibi")]
    public AudioClip butonUzerineGelmeSesi;

    void Awake()
    {
        // Singleton deseni kurulumu
        if (Ornek == null)
        {
            Ornek = this;
            // Sahne ge�i�lerinde bu objenin yok olmamas�n� isterseniz a�a��daki sat�r� aktif edin:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Zaten bir �rnek varsa, bu yenisini yok et (Singleton'�n tek olmas�n� garantiler)
            Destroy(gameObject);
            return;
        }

        // AudioSource referans�n� kontrol et ve ayarla
        if (sesKaynagi == null)
        {
            sesKaynagi = GetComponent<AudioSource>(); // Ayn� GameObject �zerinde bir AudioSource ara
            if (sesKaynagi == null)
            {
                // AudioSource bulunamazsa, bir tane ekle (veya hata ver)
                Debug.LogWarning("SesYoneticisi: AudioSource component'i atanmam�� veya bulunamad�. Bir tane ekleniyor.");
                sesKaynagi = gameObject.AddComponent<AudioSource>();
                sesKaynagi.playOnAwake = false; // Otomatik �almay� kapat
            }
        }

        // Ses klibinin atan�p atanmad���n� kontrol et
        if (butonUzerineGelmeSesi == null)
        {
            Debug.LogError("HATA: SesYoneticisi - 'Buton �zerine Gelme Sesi' (butonUzerineGelmeSesi) Inspector'da ATANMAMI�! Ses �al�namayacak.");
        }
    }

    /// <summary>
    /// Inspector'dan atanan 'butonUzerineGelmeSesi' klibini �alar.
    /// </summary>
    public void OynatButonUzerineGelmeSesi()
    {
        if (!this.enabled) return; // Script devred���ysa i�lem yapma

        if (sesKaynagi != null && butonUzerineGelmeSesi != null && sesKaynagi.isActiveAndEnabled)
        {
            sesKaynagi.PlayOneShot(butonUzerineGelmeSesi); // Tek seferlik sesler i�in ideal
        }
        else
        {
            if (sesKaynagi == null || !sesKaynagi.isActiveAndEnabled)
                Debug.LogWarning("SesYoneticisi: AudioSource yok, aktif de�il veya d�zg�n ayarlanmam��. �zerine gelme sesi oynat�lamad�.");
            if (butonUzerineGelmeSesi == null)
                Debug.LogWarning("SesYoneticisi: Buton �zerine gelme sesi (AudioClip) atanmam��. Ses oynat�lamad�.");
        }
    }

    // �leride farkl� sesleri �almak i�in bu metodu geni�letebilirsiniz:
    // public void SesCal(AudioClip calinacakSes)
    // {
    //     if (!this.enabled) return;
    //     if (sesKaynagi != null && calinacakSes != null && sesKaynagi.isActiveAndEnabled)
    //     {
    //         sesKaynagi.PlayOneShot(calinacakSes);
    //     }
    // }
}