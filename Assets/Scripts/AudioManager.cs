using UnityEngine;

// Maneja todos los sonidos del juego.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips de audio")]
    [SerializeField] private AudioClip placePieceClip;  // sonido al colocar pieza
    [SerializeField] private AudioClip lineClearClip;   // sonido al eliminar línea

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlacePiece()
    {
        audioSource.PlayOneShot(placePieceClip);
    }

    public void PlayLineClear()
    {
        audioSource.PlayOneShot(lineClearClip);
    }
}