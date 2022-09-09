using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private UIFadeIO fadeIo;
	private bool isTransiting = false;
	private WaitForSeconds fadeTime;

	static public GameManager instance;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
    private void Start()
    {
		fadeIo = GetComponentInChildren<UIFadeIO>();
		fadeTime = new WaitForSeconds(fadeIo.Duration);
	}



    public void RequestSceneTransition(BuildScene scene)
    {
        if (!isTransiting)
        {
			StartCoroutine(OnSceneTransitionBegin(scene));
        }
	}


	private IEnumerator OnSceneTransitionBegin(BuildScene scene)
    {
		isTransiting = true;
		fadeIo.OnFadeBegin(1);
		yield return fadeTime;

		var load = SceneManager.LoadSceneAsync(((int)scene));
		yield return new WaitWhile(() => load.isDone);

		fadeIo.OnFadeBegin(0);
		yield return fadeTime;
		isTransiting = false;
	}
}