using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MagdurveHakimKonusma : MonoBehaviour
{
    public KarakterKonusmaAnimasyonu magdurAnimationControl;
   

    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    public GameObject magdurKonusmaPanel;
    public TextMeshProUGUI magdurMetinText;
    public TextMeshProUGUI magdurAdiText;
    public Button magdurDevamEtButon;
    public Button magdurKapatButon;
    public Button magdurButon;

    public AudioSource diyalogAudioSource; 

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> magdurDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null; 

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
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

        
    }

    void KonusmayiBaslat()
    {
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
           
            return;
        }

        
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

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

            KonusmaMetniData currentData = magdurDiyalogMetinleri[mevcutMetinIndex];
            if (currentData.konusmaciAdi == "Maðdur" && magdurMetinText != null) magdurMetinText.text = currentData.metin;
            else if (currentData.konusmaciAdi == "Hakim" && hakimMetinText != null) hakimMetinText.text = currentData.metin;

            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
            {
                diyalogAudioSource.Stop();
            }

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

        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        KonusmaMetniData mevcutKonusma = magdurDiyalogMetinleri[mevcutMetinIndex];

        if (mevcutKonusma.konusmaciAdi == "Maðdur")
        {
            magdurKonusmaPanel.SetActive(true);
            magdurAdiText.text = mevcutKonusma.konusmaciAdi;

            if (magdurAnimationControl != null)
            {
                magdurAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAnimationControl;
            }

            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;

            if (diyalogAudioSource != null && mevcutKonusma.sesDosyasi != null)
            {
                diyalogAudioSource.clip = mevcutKonusma.sesDosyasi;
                diyalogAudioSource.Play();
            }
            
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin, hakimDevamEtButon));
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

        
        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
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

        if (diyalogAudioSource != null && diyalogAudioSource.isPlaying)
        {
            diyalogAudioSource.Stop();
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariAc(); 
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
        if (sanýkAvukatýKonusma != null && sanýkAvukatýKonusma.sanikAvukatiButon != null)
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
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
        if (sanýkAvukatýKonusma != null && sanýkAvukatýKonusma.sanikAvukatiButon != null)
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}