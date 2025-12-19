using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiceGameManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent StoppedGame = new();

    public enum DiceGameState { Off, InProgress, Won, Lost, Surrendered, CheatingExposed }

    private DiceGameState _gameState = DiceGameState.Off;
    public DiceGameState GameState => _gameState;

    public static DiceGameManager Instance;

    [Header("UI")]

    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private CanvasGroup _winCanvasGroup;
    [SerializeField] private CanvasGroup _loseCanvasGroup;
    [SerializeField] private TMP_Text _loseCanvasGroupMessage;
    [SerializeField] private float _uiFadeTime;

    [Space]

    [SerializeField] private Dice _dice;
    [SerializeField] private Image _playerSprite;
    [SerializeField] private Image _enemySprite;
    [SerializeField] private TMP_Text _currentScoreText;
    [SerializeField] private TMP_Text _playerScoreText;
    [SerializeField] private TMP_Text _enemyScoreText;
    [SerializeField] private Image _activeCharacterIndicator;
    [SerializeField] private RollOneIndicator _rollOneIndicator;
    [SerializeField] private Slider _enemySuspicionSlider;

    [Space]

    [SerializeField] private Button _surrenderButton;
    [SerializeField] private Button _passMoveButton;
    [SerializeField] private Button _rollDiceButton;
    [SerializeField] private Button _cheatButton;

    [Space]

    [SerializeField] private string _youLostMessage;
    [SerializeField] private string _youSurrenderedMessage;
    [SerializeField] private string _cheatingExposedMessage;
    [SerializeField] private float _activeCharacterIndicatorRotationTime = 1f;

    [Header("Game Settings")]

    [SerializeField][Min(0f)] private float _nextMoveDelay = 2f;
    [SerializeField][Min(0f)] private float _afterRollDelay = 1f;
    [SerializeField][Min(0f)] private float _rollDuration = 2f;

    [Header("Player Character")]
    [SerializeField] private PlayerCharacter _playerCharacter;

    private EnemyCharacter _enemyCharacter;
    private Character _activeCharacter;

    private int _rollResult;
    private int _currentScore;

    private bool _firstEnemyRoll;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        _enemySuspicionSlider.minValue = 0f;
        _enemySuspicionSlider.maxValue = EnemyCharacter.MaxSuspicion;
    }

    private void Update()
    {
        if (InputManager.Instance.Pause && _gameState != DiceGameState.Off)
        {
            _playerCharacter.AddScore(100);
            StartCoroutine(DoNextMoveAfterDelay(0f));
        }
    }

    #region Activation and Deactivation

    private bool TryInitGame(EnemyCharacter enemyCharacter)
    {
        _enemyCharacter = enemyCharacter;

        _playerCharacter.Init();
        _enemyCharacter.Init();

        _activeCharacter = _playerCharacter;
        _rollResult = 0;
        _currentScore = 0;

        _firstEnemyRoll = false;

        InitUI();

        return true;
    }

    public void StartGame(EnemyCharacter enemyCharacter)
    {
        if (_gameState != DiceGameState.Off)
            return;

        if (!TryInitGame(enemyCharacter))
        {
            Debug.LogError("Не удалось начать партию");
            return;
        }

        PlayerMovement.Instance.DisableMovement();
        PlayerInteraction.Instance.DisableInteraction();
        PauseManager.Instance.DisablePause();

        _gameState = DiceGameState.InProgress;

        StopCoroutine(FadeOutCanvasGroup(_mainCanvasGroup));
        StopCoroutine(UIFadeOut());
        StartCoroutine(UIFadeIn());
    }

    private void DisableWinAndLoseCanvasGroups()
    {
        if (_winCanvasGroup.interactable)
        {
            _winCanvasGroup.alpha = 0f;
            _winCanvasGroup.interactable = false;
            _winCanvasGroup.blocksRaycasts = false;
        }

        if (_loseCanvasGroup.interactable)
        {
            _loseCanvasGroup.alpha = 0f;
            _loseCanvasGroup.interactable = false;
            _loseCanvasGroup.blocksRaycasts = false;
        }
    }

    public void RestartGame()
    {
        if (_gameState == DiceGameState.Off)
            return;

        StopAllCoroutines();

        if (!TryInitGame(_enemyCharacter))
        {
            Debug.LogError("Не удалось перезапустить партию");
            return;
        }

        DisableWinAndLoseCanvasGroups();

        _gameState = DiceGameState.InProgress;

        StartCoroutine(DoNextMoveAfterDelay());
    }

    public void StopGame()
    {
        if (_gameState == DiceGameState.Off)
            return;

        StopAllCoroutines();

        _enemyCharacter = null;
        _activeCharacter = null;

        _rollResult = 0;
        _currentScore = 0;

        _firstEnemyRoll = false;

        _gameState = DiceGameState.Off;

        CanvasGroup activeGameEndCanvasGroup = _winCanvasGroup.interactable ? _winCanvasGroup : _loseCanvasGroup;
        StartCoroutine(FadeOutCanvasGroup(activeGameEndCanvasGroup));

        PlayerMovement.Instance.EnableMovement();
        PlayerInteraction.Instance.EnableInteraction();
        PauseManager.Instance.EnablePause();

        StoppedGame.Invoke();

        StartCoroutine(UIFadeOut());
    }

    #endregion Activation and Deactivation

    #region UI

    private IEnumerator UIFadeIn()
    {
        yield return FadeInCanvasGroup(_mainCanvasGroup);
        NextMove();
    }

    private IEnumerator UIFadeOut()
    {
        yield return FadeOutCanvasGroup(_mainCanvasGroup);
    }

    private IEnumerator FadeInCanvasGroupAfterDelay(CanvasGroup canvasGroup)
        => FadeInCanvasGroupAfterDelay(canvasGroup, _nextMoveDelay);

    private IEnumerator FadeInCanvasGroupAfterDelay(CanvasGroup canvasGroup, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        yield return FadeInCanvasGroup(canvasGroup);
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup)
    {
        float time = 0f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / _uiFadeTime);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup)
    {
        float time = 0f;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _uiFadeTime);

            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void InitUI()
    {
        _playerSprite.sprite = _playerCharacter.UISprite;
        _enemySprite.sprite = _enemyCharacter.UISprite;

        UpdateCurrentScoreUI();
        UpdateCharacterScoreUI(_playerCharacter);
        UpdateCharacterScoreUI(_enemyCharacter);

        StartCoroutine(IndicateActiveCharacter());

        _rollOneIndicator.Disable();
        _enemySuspicionSlider.value = 0f;

        _surrenderButton.interactable = false;
        _passMoveButton.interactable = false;
        _rollDiceButton.interactable = false;
        _cheatButton.gameObject.SetActive(false);
    }

    private IEnumerator IndicateActiveCharacter()
    {
        if (_activeCharacter == null)
        {
            _activeCharacterIndicator.gameObject.SetActive(false);
            yield break;
        }

        _activeCharacterIndicator.gameObject.SetActive(true);

        float startingRotation = _activeCharacterIndicator.rectTransform.localEulerAngles.z;
        float targetRotation = _activeCharacter == _playerCharacter ? 0f : 180f;

        float t = 0f;

        while (_activeCharacterIndicator.rectTransform.localEulerAngles.z != targetRotation)
        {
            _activeCharacterIndicator.rectTransform.localEulerAngles = new Vector3
            (
                x: 0f,
                y: 0f,
                z: Mathf.Lerp(startingRotation, targetRotation, t / _activeCharacterIndicatorRotationTime)
            );

            t += Time.deltaTime;
            yield return null;
        }
    }

    private void UpdateCurrentScoreUI()
    {
        _currentScoreText.text = _currentScore.ToString();
    }

    public void UpdateCharacterScoreUI(Character character)
    {
        if (character == _playerCharacter)
            _playerScoreText.text = _playerCharacter.Score.ToString();
        else if (character == _enemyCharacter)
            _enemyScoreText.text = _enemyCharacter.Score.ToString();
    }

    public void UpdateEnemySuspicionUI()
    {
        _enemySuspicionSlider.value = _enemyCharacter.Suspicion;
    }

    #endregion UI

    #region Game Logic

    private void SwitchActiveCharacter()
    {
        if (_activeCharacter == _playerCharacter)
        {
            _activeCharacter = _enemyCharacter;
            _firstEnemyRoll = true;
        }
        else
        {
            _activeCharacter = _playerCharacter;
        }

        StartCoroutine(IndicateActiveCharacter());
    }

    private IEnumerator DoNextMoveAfterDelay()
        => DoNextMoveAfterDelay(_nextMoveDelay);

    private IEnumerator DoNextMoveAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NextMove();
    }

    private void NextMove()
    {
        switch (_gameState)
        {
            case DiceGameState.Off:
            {
                break;
            }
            case DiceGameState.Won:
            {
                _playerCharacter.SaveScoreToCharacterData();
                StartCoroutine(FadeInCanvasGroupAfterDelay(_winCanvasGroup));
                break;
            }
            case DiceGameState.Lost:
            {
                _loseCanvasGroupMessage.text = _youLostMessage;
                StartCoroutine(FadeInCanvasGroupAfterDelay(_loseCanvasGroup));
                break;
            }
            case DiceGameState.Surrendered:
            {
                _loseCanvasGroupMessage.text = _youSurrenderedMessage;
                StartCoroutine(FadeInCanvasGroupAfterDelay(_loseCanvasGroup));
                break;
            }
            case DiceGameState.CheatingExposed:
            {
                _loseCanvasGroupMessage.text = _cheatingExposedMessage;
                StartCoroutine(FadeInCanvasGroupAfterDelay(_loseCanvasGroup));
                break;
            }
            default:
            {
                if (_activeCharacter is EnemyCharacter)
                {
                    EnemyRoll();
                }
                else
                {
                    _passMoveButton.interactable = true;
                    _surrenderButton.interactable = true;
                    _rollDiceButton.interactable = true;
                }

                break;
            }
        }
    }

    private void EnemyRoll()
    {
        if (_firstEnemyRoll || Random.value <= _enemyCharacter.Courage)
        {
            StartCoroutine(RollDice());
            _rollResult = _enemyCharacter.RollDice();
        }
        else
        {
            PassMove();
        }

        _firstEnemyRoll = false;
    }

    public void PlayerRoll()
    {
        _passMoveButton.interactable = false;
        _surrenderButton.interactable = false;
        _rollDiceButton.interactable = false;

        StartCoroutine(RollDice());

        _rollResult = _playerCharacter.RollDice();

        if (_rollResult == 1)
        {
            _rollOneIndicator.Enable();
            _cheatButton.gameObject.SetActive(true);
        }
    }

    public void Cheat()
    {
        _rollResult = _activeCharacter.RollDice(guaranteeSuccess: true);
        _enemyCharacter.AddSuspicion();

        _rollOneIndicator.Disable();
        _cheatButton.gameObject.SetActive(false);
    }

    private IEnumerator RollDice()
    {
        _dice.StartAnimation();

        yield return new WaitForSeconds(_rollDuration);

        _dice.StopAnimation();
        _dice.Value = _rollResult;

        _rollOneIndicator.Disable();
        _cheatButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(_afterRollDelay);

        if (_rollResult == 1)
        {
            _currentScore = 0;
            SwitchActiveCharacter();
        }
        else
        {
            _currentScore += _rollResult;
        }

        _rollResult = 0;

        UpdateCurrentScoreUI();
        StartCoroutine(DoNextMoveAfterDelay());

        yield return null;
    }

    public void Surrender()
    {
        _passMoveButton.interactable = false;
        _surrenderButton.interactable = false;
        _rollDiceButton.interactable = false;

        _gameState = DiceGameState.Surrendered;
        NextMove();
    }

    public void PassMove()
    {
        _passMoveButton.interactable = false;
        _surrenderButton.interactable = false;
        _rollDiceButton.interactable = false;

        _activeCharacter.AddScore(_currentScore);

        _currentScore = 0;
        UpdateCurrentScoreUI();

        SwitchActiveCharacter();
        StartCoroutine(IndicateActiveCharacter());
        StartCoroutine(DoNextMoveAfterDelay());
    }

    public void CharacterWon(Character character)
    {
        if (character == _playerCharacter)
            _gameState = DiceGameState.Won;
        else if (character == _enemyCharacter)
            _gameState = DiceGameState.Lost;
    }

    public void CheatingExposed()
    {
        _gameState = DiceGameState.CheatingExposed;
    }

    #endregion Game Logic
}
