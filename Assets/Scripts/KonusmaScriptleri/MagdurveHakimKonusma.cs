using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// KonusmaMetniData ScriptableObject'i ayr� bir dosyada tan�ml� oldu�u i�in,
// bu script i�inde tekrar TANIMLANMAMALI.

public class MagdurveHakimKonusma : MonoBehaviour
{
    // Ma�dur'un a��z animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAnimationControl;
    // Hakim i�in de animasyon kontrol� ekleyecekseniz (e�er ona da a��z yap�s� kurduysan�z):
    // public KarakterKonusmaAnimasyonu hakimAnimationControl;


    // Hakim Konu�ma UI Elemanlar�
    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    // Ma�dur Konu�ma UI Elemanlar�
    public GameObject magdurKonusmaPanel;
    public TextMeshProUGUI magdurMetinText;
    public TextMeshProUGUI magdurAdiText;
    public Button magdurDevamEtButon;
    public Button magdurKapatButon;
    public Button magdurButon; // Ma�dur'un kendi diyalogunu ba�latan buton

    // --- SES ���N EKLENEN KISIM ---
    public AudioSource diyalogAudioSource; // Ma�dur ve Hakim'in seslerini �alacak AudioSource
    // --- SES ���N EKLENEN KISIM SONU ---

    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullan�yor)
    public List<KonusmaMetniData> magdurDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null; // Aktif konu�an�n animasyonunu y�netmek i�in

    // Di�er diyalog y�neticilerine referanslar (buton etkile�imi i�in)
    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public San�kAvukat�Konusma san�kAvukat�Konusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;

    void Start()
    {
        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        magdurButon.onClick.AddListener(KonusmayiBaslat);
        magdurKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
        hakimDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        // Diyalog metinleri kontrol�
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
            Debug.LogError("Ma�dur Diyalog Metinleri Inspector'da atanmam�� veya bo�!");
        }
    }

    void KonusmayiBaslat()
    {
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Ma�dur diyalog listesi bo� veya atanmam��!");
            return;
        }

        // �nceki konu�an�n animasyonunu durdur (e�er varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES ���N EKLENEN KISIM: Konu�ma ba�lad���nda �nceki sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ���N EKLENEN KISIM SONU ---

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // E�er metin hala yaz�l�yorsa, yazmay� tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            // Metni an�nda tamamla
            KonusmaMetniData currentData = magdurDiyalogMetinleri[mevcutMetinIndex];
            if (currentData.konusmaciAdi == "Ma�dur" && magdurMetinText != null) magdurMetinText.text = currentData.metin;
            else if (currentData.konusmaciAdi == "Hakim" && hakimMetinText != null) hakimMetinText.text = currentData.metin;

            // H�zl� t�klamada metin yaz�m� tamamland���nda a�z� kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // --- SES ���N EKLENEN KISIM: H�zl� t�klamada ses hala �al�yorsa durdur ---
            if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
            {
                diyalogAudioSource.Stop();
            }
            // --- SES ���N EKLENEN KISIM SONU ---

            // Butonlar� tekrar aktif et (metin yazma tamamland�)
            magdurDevamEtButon.gameObject.SetActive(true);
            magdurDevamEtButon.interactable = true;
            hakimDevamEtButon.gameObject.SetActive(true);
            hakimDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir();
        }
    }

    void MevcutMetniGoster()
    {
        // �nceki metin animasyonunu durdur
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        // �nceki konu�an�n animasyonunu durdur (e�er varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES ���N EKLENEN KISIM: Yeni konu�ma ba�lad���nda �nceki sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ���N EKLENEN KISIM SONU ---

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        KonusmaMetniData mevcutKonusma = magdurDiyalogMetinleri[mevcutMetinIndex];

        if (mevcutKonusma.konusmaciAdi == "Ma�dur")
        {
            magdurKonusmaPanel.SetActive(true);
            magdurAdiText.text = mevcutKonusma.konusmaciAdi;

            // Ma�dur animasyonunu ba�lat
            if (magdurAnimationControl != null)
            {
                magdurAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAnimationControl; // Aktif konu�an yap
            }

            // --- SES ���N EKLENEN KISIM: �lgili ses dosyas�n� �al ---
            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Ma�dur diyalo�u i�in ses dosyas� atanmam��: Index {mevcutMetinIndex}");
            }
            // --- SES ���N EKLENEN KISIM SONU ---

            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;

            // Hakim i�in animasyon kontrol� varsa burada ba�lat�n
            // if (hakimAnimationControl != null)
            // {
            //    hakimAnimationControl.KonusmayaBasla();
            //    currentSpeakerAnimationControl = hakimAnimationControl;
            // }

            // --- SES ���N EKLENEN KISIM: �lgili ses dosyas�n� �al ---
            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            else if (mevcutKonusma.sesDosyasi == null)
            {
                Debug.LogWarning($"Hakim diyalo�u i�in ses dosyas� atanmam��: Index {mevcutMetinIndex}");
            }
            // --- SES ���N EKLENEN KISIM SONU ---

            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin, hakimDevamEtButon));
        }
        else
        {
            Debug.LogWarning("Bilinmeyen konu�mac�: " + mevcutKonusma.konusmaciAdi + " (Ma�dur Diyalog Scriptinde) �ndeks: " + mevcutMetinIndex);
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu)
    {
        int harfSayisi = 0;
        metinAlani.text = "";

        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // Metin yaz�m� tamamland���nda konu�ma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        // --- SES ���N EKLENEN KISIM: Metin animasyonu bitti�inde sesi durdur ---
        // E�er ses hala �al�yorsa (yani metinden daha uzunsa), durdur.
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ���N EKLENEN KISIM SONU ---

        devamButonu.gameObject.SetActive(true);
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konu�an�n animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        // --- SES ���N EKLENEN KISIM: Diyalog bitti�inde sesi durdur ---
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }
        // --- SES ���N EKLENEN KISIM SONU ---

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariAc(); // magdurButon da burada tekrar aktif edilecek.
        mevcutMetinIndex = 0;
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void ButonlariKapat()
    {
        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = false;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = false;
        if (san�kAvukat�Konusma != null && san�kAvukat�Konusma.sanikAvukatiButon != null)
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = false;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        magdurButon.interactable = true;

        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = true;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = true;
        if (san�kAvukat�Konusma != null && san�kAvukat�Konusma.sanikAvukatiButon != null)
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}