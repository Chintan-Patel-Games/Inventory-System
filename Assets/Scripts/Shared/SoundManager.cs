using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip uiClick;
    [SerializeField] private AudioClip popupOpenClick;
    [SerializeField] private AudioClip popupCloseClick;
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip sellSound;
    [SerializeField] private AudioClip errorSound;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip[] bgmPlaylist;
    private int currentTrackIndex = 0;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional if you want music to persist
    }

    private void Start() => PlayCurrentBGM();

    private void Update()
    {
        if (!bgmSource.isPlaying && bgmPlaylist.Length > 0) NextBGM();
    }

    // ----------------- BGM Logic -----------------

    private void PlayCurrentBGM()
    {
        if (bgmPlaylist.Length == 0) return;

        bgmSource.clip = bgmPlaylist[currentTrackIndex];
        bgmSource.loop = false; // we rotate manually
        bgmSource.Play();
    }

    private void NextBGM()
    {
        currentTrackIndex = (currentTrackIndex + 1) % bgmPlaylist.Length;
        PlayCurrentBGM();
    }

    public void StopBGM() => bgmSource.Stop();
    public void PauseBGM() => bgmSource.Pause();
    public void ResumeBGM() => bgmSource.UnPause();

    // ----------------- SFX Logic -----------------

    public void PlayUIClick() => PlaySFX(uiClick);
    public void PlayPopupOpenClick() => PlaySFX(popupOpenClick);
    public void PlayPopupCloseClick() => PlaySFX(popupCloseClick);
    public void PlayBuySound() => PlaySFX(buySound);
    public void PlaySellSound() => PlaySFX(sellSound);
    public void PlayErrorSound() => PlaySFX(errorSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
    }
}