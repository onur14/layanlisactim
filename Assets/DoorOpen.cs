using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
public class DoorOpen : MonoBehaviour
{
    [Header("Etkileşim")]
    [Tooltip("Boş bırakılırsa MainCamera etiketli kamera kullanılır.")]
    [SerializeField] private Camera oyuncuKamerasi;
    [SerializeField, Min(0.1f)] private float etkilesimMesafesi = 3f;

    [Header("Kapı")]
    [Tooltip("Kapının döneceği menteşe Transform'u. Boş bırakılırsa bu nesne döner.")]
    [SerializeField] private Transform mentese;
    [SerializeField] private float acilmaAcisi = 90f;
    [SerializeField, Min(1f)] private float acilmaHizi = 120f;

    [Tooltip("Collider scriptin üst nesnesindeyse veya ayrı bir nesnedeyse buraya atayın.")]
    [SerializeField] private Collider kapiCollider;
    [SerializeField] private bool hataAyiklama = true;

    private Transform hareketEdenKapi;
    private float mevcutAci;
    private bool hareketEdiyor;
    private bool hedefAcik;

    private void Awake()
    {
        if (mentese == null)
            mentese = transform;

        // Menteşe kapının üst nesnesiyse bütün kapıyı onunla döndür.
        // Ayrı bir noktaysa kapının kendisini o noktanın çevresinde döndür.
        hareketEdenKapi = transform.IsChildOf(mentese) ? mentese : transform;
    }

    private void LateUpdate()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye != null && klavye.eKey.wasPressedThisFrame)
            KapiyiKontrolEt();

        if (!hareketEdiyor || mentese == null || hareketEdenKapi == null)
            return;

        float hedefAci = hedefAcik ? acilmaAcisi : 0f;
        float yeniAci = Mathf.MoveTowards(mevcutAci, hedefAci,
            Mathf.Max(1f, acilmaHizi) * Time.deltaTime);
        hareketEdenKapi.RotateAround(mentese.position, mentese.up, yeniAci - mevcutAci);
        mevcutAci = yeniAci;

        if (Mathf.Approximately(mevcutAci, hedefAci))
        {
            hareketEdiyor = false;
        }
    }
    private void KapiyiKontrolEt()
    {
        if (oyuncuKamerasi == null)
        {
            oyuncuKamerasi = Camera.main;
            if (oyuncuKamerasi == null)
            {
                KameraTakip takip = FindAnyObjectByType<KameraTakip>();
                if (takip != null)
                    oyuncuKamerasi = takip.GetComponent<Camera>();
            }
        }

        if (oyuncuKamerasi == null)
        {
            Debug.LogWarning("DoorOpen: Oyuncu kamerası bulunamadı. Inspector'dan kamerayı atayın.", this);
            return;
        }

        // Kamera LateUpdate içinde hareket ettikten sonra ekran merkezinden ışın at.
        Ray isin = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.SyncTransforms();
        Debug.DrawRay(isin.origin, isin.direction * etkilesimMesafesi, Color.yellow, 2f);

        if (!Physics.Raycast(isin, out RaycastHit temas, etkilesimMesafesi,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hataAyiklama)
                Debug.Log("DoorOpen: E algılandı, " + etkilesimMesafesi
                    + " metre içinde Collider bulunamadı. Mesafeyi ve kapının Collider'ını kontrol edin.", this);
            return;
        }

        bool buKapi = temas.collider == kapiCollider
            || temas.collider.GetComponentInParent<DoorOpen>() == this;

        if (!buKapi)
        {
            if (hataAyiklama)
                Debug.Log("DoorOpen: Işın kapı yerine '" + temas.collider.name
                    + "' nesnesine çarptı (" + temas.distance.ToString("F2")
                    + " m). Bu kapının Collider'ıysa Kapi Collider alanına atayın.", temas.collider);
            return;
        }

        if (Mathf.Approximately(acilmaAcisi, 0f))
        {
            Debug.LogWarning("DoorOpen: Açılma açısı 0; kapı dönemez.", this);
            return;
        }

        if (Time.timeScale == 0f)
        {
            Debug.LogWarning("DoorOpen: Oyun duraklatılmış (Time.timeScale = 0); kapı hareket edemez.", this);
            return;
        }

        hedefAcik = !hedefAcik;
        hareketEdiyor = true;
        if (hataAyiklama)
            Debug.Log("DoorOpen: " + name + (hedefAcik ? " açılıyor." : " kapanıyor."), this);
    }
}
