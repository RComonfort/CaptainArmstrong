using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOverScreen : MenuScriptParent
{

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();
    }

	public void Init(bool playerWon, string timeTaken)
	{
		string resultText = playerWon ? "Escaped in " + timeTaken + "!" : "Survived for " + timeTaken;
		transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(resultText);
	}

	public void PlayAgain()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void MainMenu()
	{
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}

    // Update is called once per frame
    void Update()
    {
		ButtonNavigationUpdate();
    }
}
