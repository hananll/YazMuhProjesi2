using UnityEngine;

public class ArkaPlanYonetimi : MonoBehaviour
{
    
    public GameObject blurluArkaPlanGameObject; // Burada blurlu background a referans verdim.

    public static ArkaPlanYonetimi Ornek { get; private set; }

    void Awake()
    {
       
        if (Ornek == null)
        {
            Ornek = this;
            
        }
        else if (Ornek != this)
        {
           
            Destroy(gameObject);
            return;
        }

        if (blurluArkaPlanGameObject == null)
        {
            
            this.enabled = false;
            return;
        }
        blurluArkaPlanGameObject.SetActive(false); // Bu satýrda baþlangýçta kapalý olmasýný saðladým.
        
    }

    public void BlurluArkaPlaniAyarla(bool aktifEt)
    {
        if (!this.enabled || blurluArkaPlanGameObject == null)
        {
            return;
        }

        blurluArkaPlanGameObject.SetActive(aktifEt);
    }
}