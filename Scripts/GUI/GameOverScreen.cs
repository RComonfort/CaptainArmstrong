using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{
	[SerializeField] private Sprite normalButton;
	[SerializeField] private Sprite pressedButton;
	[SerializeField] private Sprite hoveredButton;

	private Button[] buttons;
	private Image[] buttonImages;
	private int selectedButtonIndex = 0;
	private float lastInputPress = 0;


    // Start is called before the first frame update
    void Start()
    {
    	buttons = GetComponentsInChildren<Button>();
		buttonImages = new Image[buttons.Length];
		for (int i = 0; i < buttons.Length; i++)
			buttonImages[i] = buttons[i].GetComponent<Image>();
			

		HoverButton(0);
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
        var pointer = new PointerEventData(EventSystem.current);

		//Click current button
		if (Input.GetAxisRaw("Action1") > .2f && Time.timeSinceLevelLoad > lastInputPress + .2f)
		{
			ClickButton(selectedButtonIndex);
			lastInputPress = Time.timeSinceLevelLoad;
		}
		else if (Input.GetAxisRaw("Action2") > .2f && Time.timeSinceLevelLoad > lastInputPress + .2f) //Select next button (downwards)
		{
			//unhover current button
			UnhoverButton(selectedButtonIndex);

			//Update selected button index
			selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Length;

			//Hover next button
			HoverButton(selectedButtonIndex);

			lastInputPress = Time.timeSinceLevelLoad;
		}
    }

	void ClickButton(int buttonIndex)
	{
		var pointer = new PointerEventData(EventSystem.current);
		ExecuteEvents.Execute(buttons[buttonIndex].gameObject, pointer, ExecuteEvents.submitHandler);

		buttonImages[buttonIndex].sprite = pressedButton;
	}

	void HoverButton(int buttonIndex)
	{
		buttonImages[buttonIndex].sprite = hoveredButton;
	}

	void UnhoverButton(int buttonIndex)
	{
		buttonImages[buttonIndex].sprite = normalButton;
	}
}
