using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TanikCagirici : MonoBehaviour
{
    [Header("Ge�i� Ekran�")]
    public GameObject AliKemalCagirmaGecisEkrani;
    public GameObject AliKemalBitirmeGecisEkrani;
    public GameObject EbruGurCagirmaGecisEkrani;
    public GameObject EbruGurBitirmeGecisEkrani;

    public TextMeshProUGUI gecisYazisi;
    public AudioSource sesOynatici;
    public AudioClip tanikSesi;

    [Header("Sahne Adlar�")]
    public string tanik1SahneAdi;
    public string tanik2SahneAdi;
    public string anaSahneAdi; // ifadesi biten tan���n d�n�� sahnesi

    // Tan�k �a��rma fonksiyonu
    public void Tanik1Cagir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(TanikGecisi("Tan�k Ali Kemal Gezmi�", tanik1SahneAdi));
        AliKemalCagirmaGecisEkrani.SetActive(true);

    }

    public void Tanik2Cagir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(TanikGecisi("Tan�k Ebru G�r", tanik2SahneAdi));
        EbruGurCagirmaGecisEkrani.SetActive(true);
    }

    // Tan�k ifadesini sonland�rma fonksiyonlar�
    public void Tanik1IfadesiniSonlandir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(SonlandirmaGecisi("Ali Kemal Gezmi��in ifadesi sonland�r�l�yor...", anaSahneAdi));
        AliKemalBitirmeGecisEkrani.SetActive(true);
    }

    public void Tanik2IfadesiniSonlandir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(SonlandirmaGecisi("Ebru G�r'�n ifadesi sonland�r�l�yor...", anaSahneAdi));
        EbruGurBitirmeGecisEkrani.SetActive(true);
    }

    // Ortak ge�i� coroutine�leri
    private IEnumerator TanikGecisi(string tanikAdi, string sahneAdi)
    {
        // gecisEkrani.SetActive(true);
        sesOynatici.PlayOneShot(tanikSesi);
        gecisYazisi.text = $"{tanikAdi} �a�r�l�yor...";
        

        yield return new WaitForSeconds(tanikSesi.length + 1f);

        SceneManager.LoadScene(sahneAdi);
    }

    private IEnumerator SonlandirmaGecisi(string mesaj, string sahneAdi)
    {
       // gecisEkrani.SetActive(true);
        gecisYazisi.text = mesaj;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sahneAdi);
    }
}
