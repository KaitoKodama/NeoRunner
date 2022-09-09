using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIPlayer : MonoBehaviour
{
    [SerializeField] GameObject closePanel = default;
    [SerializeField] Button restartBtn = default;
    [SerializeField] Button[] backBtns = default;
    [SerializeField] Text resultText = default;
    [SerializeField] Text distanceText = default;
    [SerializeField] Text scoreText = default;
    [SerializeField] Text moveText = default;
    [SerializeField] Image fillHP = default;
    [SerializeField] Image fillBullet = default;
    [SerializeField] EnemyFaced enemyFaced = default;

    private Player player;
    private Transform playerform;
    private float distance, score;
    private float startPos = 18f;
    private float speed = 5f;


    void Start()
    {
        closePanel.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnDeathNotifyerHandler = OnPlayerDeathReciever;

        playerform = player.gameObject.transform;

        foreach (var btn in backBtns)
        {
            btn.onClick.AddListener(OnBackToHomeButton);
        }
        restartBtn.onClick.AddListener(OnRestartButton);
    }
    private void Update()
    {
        fillHP.fillAmount = Mathf.Lerp(fillHP.fillAmount, player.ClampHP, Time.deltaTime * speed);
        fillBullet.fillAmount = Mathf.Lerp(fillBullet.fillAmount, player.ClampBulletNum, Time.deltaTime * speed);

        distance = Mathf.Clamp(playerform.position.x + startPos, 0, float.MaxValue);
        distanceText.text = $"distance {(int)distance}m";

        score = enemyFaced.GetFullScore();
        scoreText.text = $"score {score}";

        string movement = player.IsDash ? "dash" : "normal";
        moveText.text = $"{movement}";
    }


    private void OnPlayerDeathReciever()
    {
        closePanel.SetActive(true);
        string text = $"distance / {(int)distance}m\nscore / {score}\n\ntotal score / {score + distance}";
        resultText.DOText(text, 3f, true, ScrambleMode.All);
    }
    private void OnBackToHomeButton()
    {
        GameManager.instance.RequestSceneTransition(BuildScene.Title);
    }
    private void OnRestartButton()
    {
        GameManager.instance.RequestSceneTransition(BuildScene.Battle);
    }
}
