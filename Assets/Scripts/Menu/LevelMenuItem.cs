using UnityEngine;
using YG;

public class LevelMenuItem : MonoBehaviour
{
    [SerializeField] private LevelLockedPanel _lvlLockedPanel;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private bool _unlocked = false;
    [SerializeField] private Animator _animator;

    private int _levelsCount = 5;

    private void OnEnable()
    {
        CurrencyManager.Instance.CupsChanged += CheckCupsButton;
        _animator.SetTrigger("Open");
        if (_unlocked || (YG2.saves.levels.Length > 0 && YG2.saves.levels[_levelData.ID-1]))
        {
            UnlockLevel();
        } else
        {
            CheckCupsButton(CurrencyManager.Instance.Cups);
        }
    }

    private void Start()
    {
        if (!_unlocked)
        {
            _lvlLockedPanel.SetPrice(_levelData);
        }
    }


    private void CheckCupsButton(int newAmount)
    {
        if (newAmount >= _levelData.CupsPrice)
        {
            _lvlLockedPanel.UnlockCupsButton();
        }
    }

    public void LoadLevel()
    {
        if (_unlocked)
        {
            GameLoader.Instance.LoadNextScene(_levelData.Scene, true);
            GameManager.Instance.LevelInProgress?.Invoke(true);
        }
    }


    public void UnlockLevel()
    {
        _unlocked = true;
        YG2.saves.levels[_levelData.ID - 1] = true;
        _lvlLockedPanel.gameObject.SetActive(false);
    }


    public void TryUnlockWithCups()
    {
        if (CurrencyManager.Instance.RemoveCups(_levelData.CupsPrice))
        {
            UnlockLevel();
        }
    }

    public void TryUnlockWithGems()
    {
        if (CurrencyManager.Instance.RemoveGems(_levelData.GesmsPrice))
        {
            UnlockLevel();
        } 
    }

}
