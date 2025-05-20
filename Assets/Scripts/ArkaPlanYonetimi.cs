// Script Ad�: ArkaPlanYonetimi.cs
using UnityEngine;

public class ArkaPlanYonetimi : MonoBehaviour
{
    [Tooltip("Bulan�kla�t�r�lm�� arka plan g�rselini i�eren GameObject")]
    public GameObject blurluArkaPlanGameObject;

    public static ArkaPlanYonetimi Ornek { get; private set; }

    void Awake()
    {
        Debug.Log("ArkaPlanYonetimi: Awake() �ALI�TI.");
        if (Ornek == null)
        {
            Ornek = this;
            Debug.Log("ArkaPlanYonetimi: Ornek (Singleton) ATANDI.");
        }
        else if (Ornek != this)
        {
            Debug.LogWarning("ArkaPlanYonetimi: Zaten bir �rnek var, bu kopya yok ediliyor: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        if (blurluArkaPlanGameObject == null)
        {
            Debug.LogError("HATA (ArkaPlanYonetimi): 'blurluArkaPlanGameObject' Inspector'da ATANMAMI�! Script d�zg�n �al��mayacak.");
            this.enabled = false;
            return;
        }
        blurluArkaPlanGameObject.SetActive(false); // Ba�lang��ta kesin kapal�
        Debug.Log("ArkaPlanYonetimi: Blurlu arka plan ba�lang��ta de-aktif edildi.");
    }

    public void BlurluArkaPlaniAyarla(bool aktifEt)
    {
        Debug.Log("ArkaPlanYonetimi: BlurluArkaPlaniAyarla(" + aktifEt + ") �A�RILDI. Obje: " + (blurluArkaPlanGameObject != null ? blurluArkaPlanGameObject.name : "NULL"));
        if (!this.enabled || blurluArkaPlanGameObject == null)
        {
            Debug.LogError("HATA (ArkaPlanYonetimi): BlurluArkaPlaniAyarla �a�r�ld� ama script devred��� veya blurluArkaPlanGameObject NULL.");
            return;
        }

        blurluArkaPlanGameObject.SetActive(aktifEt);
        Debug.Log("ArkaPlanYonetimi: " + blurluArkaPlanGameObject.name + " SetActive(" + aktifEt + ") YAPILDI.");
    }
}