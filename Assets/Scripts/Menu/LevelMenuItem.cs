using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class LevelMenuItem : MonoBehaviour
{
    [SerializeField] private LevelLockedPanel _lvlLockedPanel;
    [SerializeField] private Text _rewardText;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private bool _unlocked = false;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _lvlImage;
    [SerializeField] private GameObject _firstWinBonus;

    private bool _firstWin;

    private void OnEnable()
    {
        //CurrencyManager.Instance.CupsChanged += CheckCupsButton;
        _animator.SetTrigger("Open");
        if (_unlocked)
        {
            _lvlLockedPanel.gameObject.SetActive(false);
            if (!_firstWin)
            {
                _firstWinBonus.gameObject.SetActive(true);
            }
        }
        else
        {
            CheckCupsButton(CurrencyManager.Instance.Cups);
        }
        
    }

    private void Start()
    {
        _lvlImage.sprite = _levelData.IMG;
        SetReward();
        if (!_unlocked)
        {
            _lvlLockedPanel.SetPrice(_levelData);
        }
    }


    public void Init(LevelData data, bool unlocked, bool win)
    {
        _levelData = data;
        _unlocked = unlocked;
        _firstWin = win;
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
            GameManager.Instance.StartLevel(_levelData);
        }
    }


    public void UnlockLevel()
    {
        _unlocked = true;
        _lvlLockedPanel.gameObject.SetActive(false);
    }


    public void TryUnlockWithCups()
    {
        LevelsManager.Instance.TryUnlockLevel(_levelData, CurrencyType.Cups);
    }

    public void TryUnlockWithGems()
    {
        LevelsManager.Instance.TryUnlockLevel(_levelData, CurrencyType.Gems);
    }

    private void SetReward()
    {
        _rewardText.text = "+" + _levelData.Reward[0].Amount.ToString();
    }

}
