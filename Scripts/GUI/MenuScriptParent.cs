using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScriptParent : MonoBehaviour
{
	[Header("Button Sprites")]
	[SerializeField] protected Sprite normalButton;
	[SerializeField] protected Sprite pressedButton;
	[SerializeField] protected Sprite hoveredButton;

	[Header("Button SFX")]
	[SerializeField] protected AudioClip buttonClickClip;
	[SerializeField] protected AudioClip buttonHovered;

	[Header("SFX")]
	[SerializeField] protected AudioSource sfxSource;
	

	protected Button[] buttons;
	protected Image[] buttonImages;

	protected int selectedButtonIndex = 0;
	protected float lastInputPress = 0;

	private bool calledStart = false;

	public virtual void Init()
	{
		//Start will init the obj anyways. So it hasnt being called (the object started the game disabled), then ignore this init call
		if (!calledStart)
			return;

		lastInputPress = Time.timeSinceLevelLoad;
		
		for (int i = 0; i < buttons.Length; i++)
			UnhoverButton(i);

		HoverButton(0);
	}

    // Start is called before the first frame update
    protected virtual void Start()
    {
        buttons = GetComponentsInChildren<Button>();
		buttonImages = new Image[buttons.Length];
		for (int i = 0; i < buttons.Length; i++)
			buttonImages[i] = buttons[i].GetComponent<Image>();

		calledStart = true;

		Init();
    }

	protected void ButtonNavigationUpdate()
	{
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

	protected void ClickButton(int buttonIndex)
	{
		var pointer = new PointerEventData(EventSystem.current);
		ExecuteEvents.Execute(buttons[buttonIndex].gameObject, pointer, ExecuteEvents.submitHandler);

		buttonImages[buttonIndex].sprite = pressedButton;
	}

	protected void HoverButton(int buttonIndex)
	{
		buttonImages[buttonIndex].sprite = hoveredButton;
	}

	protected void UnhoverButton(int buttonIndex)
	{
		buttonImages[buttonIndex].sprite = normalButton;
	}
}
