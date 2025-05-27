using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;



public class MagdurAvukatiKonusma : MonoBehaviour
{
    public KarakterKonusmaAnimasyonu magdurAvukatiAnimationControl;

    public GameObject magdurAvukatiKonusmaPanel;
    public TextMeshProUGUI magdurAvukatiMetinText;
    public TextMeshProUGUI magdurAvukatiAdiText;
    public Button magdurAvukatiKapatButon;
    public Button magdurAvukatiButon;
    public Button magdurAvukatiDevamEtButon;

    public AudioSource magdurAvukatiAudioSource; 

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> magdurAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public TokmakSistemiYoneticisi tokmakSistemiYoneticisi;

    void Start()
    {
        magdurAvukatiKonusmaPanel.SetActive(false); 

        magdurAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        magdurAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurAvukatiDevamEtButon.gameObject.SetActive(false);

      
    }

    void KonusmayiBaslat()
    {
        if (magdurAvukatiDiyalogMetinleri == null || magdurAvukatiDiyalogMetinleri.Count == 0)
        {
           
            return;
        }

        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }

        magdurAvukatiKonusmaPanel.SetActive(true); 
        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            KonusmaMetniData currentData = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];
            magdurAvukatiMetinText.text = currentData.metin; 

            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
            {
                magdurAvukatiAudioSource.Stop();
            }

            magdurAvukatiDevamEtButon.gameObject.SetActive(true);
            magdurAvukatiDevamEtButon.interactable = true;
            return; 
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
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
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); 

        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];

            magdurAvukatiMetinText.text = ""; 
            magdurAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            if (magdurAvukatiAnimationControl != null)
            {
                magdurAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAvukatiAnimationControl;
            }

            if (magdurAvukatiAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                magdurAvukatiAudioSource.clip = mevcutKonusma.sesDosyasi;
                magdurAvukatiAudioSource.Play();
            }
            

            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurAvukatiMetinText, mevcutKonusma.metin, magdurAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir();
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

        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

       
        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }

        devamButonu.gameObject.SetActive(true);
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (magdurAvukatiAudioSource != null && magdurAvukatiAudioSource.isPlaying)
        {
            magdurAvukatiAudioSource.Stop();
        }

        magdurAvukatiKonusmaPanel.SetActive(false);
        magdurAvukatiDevamEtButon.gameObject.SetActive(false); 

        ButonlariAc(); 
        mevcutMetinIndex = 0; 
    }

    void ButonlariKapat()
    {
        sanikveHakimKonusma.mikrofonButon.interactable = false;
        savciKonusma.savciButon.interactable = false;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
        magdurKonusma.magdurButon.interactable = false;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = false;
    }

    void ButonlariAc()
    {
        magdurAvukatiButon.interactable = true;

        sanikveHakimKonusma.mikrofonButon.interactable = true;
        savciKonusma.savciButon.interactable = true;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        magdurKonusma.magdurButon.interactable = true;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = true;
    }
}