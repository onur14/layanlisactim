using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class KameraTakip : MonoBehaviour
{
    [Header("Takip Ayarları")]
    [Tooltip("Takip edilecek karakterin ana Transform bileşeni.")]
    [SerializeField] private Transform karakter;
    [Tooltip("Karakterin merkezine göre göz konumu. Baş noktasını seçersen sıfır yapabilirsin.")]
    [SerializeField] private Vector3 gozOfseti = new Vector3(0f, 1.6f, 0f);

    [Header("Fare Ayarları")]
    [SerializeField, Min(0f)] private float fareHassasiyeti = 0.1f;
    [SerializeField, Range(0f, 89f)] private float dikeyBakisSiniri = 85f;
    [Tooltip("Yatay bakış yönüne karakteri de döndürür.")]
    [SerializeField] private bool karakteriDondur = true;

    private float yatayAci;
    private float dikeyAci;
    private bool fareKilitli;

    private void OnEnable()
    {
        if (karakter == null || karakter == transform || karakter.IsChildOf(transform))
        {
            Debug.LogWarning("KameraTakip: Kameradan bağımsız bir karakter Transform'u atayın.", this);
            enabled = false;
            return;
        }

        yatayAci = karakter.eulerAngles.y;
        dikeyAci = Mathf.Clamp(Mathf.DeltaAngle(0f, transform.eulerAngles.x),
            -dikeyBakisSiniri, dikeyBakisSiniri);
        FareKilidiniAyarla(true);
    }

    private void LateUpdate()
    {
        if (karakter == null)
        {
            enabled = false;
            return;
        }

        Mouse fare = Mouse.current;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            FareKilidiniAyarla(false);
        else if (fare != null && fare.leftButton.wasPressedThisFrame && !fareKilitli)
            FareKilidiniAyarla(true);

        if (fareKilitli && Cursor.lockState == CursorLockMode.Locked && fare != null)
        {
            // Mouse delta zaten kare başına harekettir; deltaTime ile çarpılmaz.
            Vector2 hareket = fare.delta.ReadValue() * Mathf.Max(0f, fareHassasiyeti);
            yatayAci = Mathf.Repeat(yatayAci + hareket.x, 360f);
            dikeyAci = Mathf.Clamp(dikeyAci - hareket.y, -dikeyBakisSiniri, dikeyBakisSiniri);
        }

        if (karakteriDondur)
            karakter.rotation = Quaternion.Euler(0f, yatayAci, 0f);

        transform.SetPositionAndRotation(
            karakter.position + karakter.rotation * gozOfseti,
            Quaternion.Euler(dikeyAci, yatayAci, 0f));
    }

    private void FareKilidiniAyarla(bool kilitle)
    {
        fareKilitli = kilitle;
        Cursor.lockState = kilitle ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !kilitle;
    }

    private void OnApplicationFocus(bool odakta)
    {
        if (!odakta && fareKilitli)
            FareKilidiniAyarla(false);
    }

    private void OnDisable()
    {
        if (fareKilitli)
            FareKilidiniAyarla(false);
    }
}
