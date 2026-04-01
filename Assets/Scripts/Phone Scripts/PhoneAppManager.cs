using UnityEngine;

public class PhoneAppManager : MonoBehaviour
{
    public enum App { Home, Uber, Game, Shop }

    [Header("Panels")]
    public GameObject homePanel;
    public GameObject uberPanel;
    public GameObject gamePanel;
    public GameObject shopPanel;

    [Header("References")]
    public PhoneMinigameSelector minigameSelector;

    public App CurrentApp { get; private set; } = App.Home;

    void Start()
    {
        OpenHome();
    }

    void LateUpdate()
    {
        // HARD LOCK: prevent panel stacking no matter what other scripts do
        EnforcePanels();
    }

    void EnforcePanels()
    {
        bool home = (CurrentApp == App.Home);
        bool uber = (CurrentApp == App.Uber);
        bool game = (CurrentApp == App.Game);
        bool shop = (CurrentApp == App.Shop);

        if (homePanel != null && homePanel.activeSelf != home) homePanel.SetActive(home);
        if (uberPanel != null && uberPanel.activeSelf != uber) uberPanel.SetActive(uber);
        if (gamePanel != null && gamePanel.activeSelf != game) gamePanel.SetActive(game);
        if (shopPanel != null && shopPanel.activeSelf != shop) shopPanel.SetActive(shop);
    }

    public bool IsUberOpen()
    {
        return CurrentApp == App.Uber;
    }

    public void OpenHome()
    {
        if (CurrentApp == App.Game && minigameSelector != null)
        {
            minigameSelector.ResetCurrentGame();
            minigameSelector.HideAllGames();
        }

        if (ShopMusicPlayer.Instance != null)
            ShopMusicPlayer.Instance.StopShopMusic();

        CurrentApp = App.Home;
        EnforcePanels();
    }

    public void OpenUber()
    {
        if (CurrentApp == App.Game && minigameSelector != null)
        {
            minigameSelector.ResetCurrentGame();
            minigameSelector.HideAllGames();
        }

        if (ShopMusicPlayer.Instance != null)
            ShopMusicPlayer.Instance.StopShopMusic();

        CurrentApp = App.Uber;
        EnforcePanels();
    }

    public void OpenGame()
    {
        if (ShopMusicPlayer.Instance != null)
            ShopMusicPlayer.Instance.StopShopMusic();

        CurrentApp = App.Game;
        EnforcePanels();

        if (minigameSelector != null)
            minigameSelector.OpenRandomUnlockedGame();
    }

    public void OpenShop()
    {
        CurrentApp = App.Shop;
        EnforcePanels();

        if (ShopMusicPlayer.Instance != null)
            ShopMusicPlayer.Instance.PlayShopMusic();
    }

    // Old tap-button method, no longer needed
    public void OnGameTap()
    {
        // intentionally left empty
    }
}