using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 using UnityEngine.EventSystems;


public class MenuController : MonoBehaviour
{
	[SerializeField] private AudioClip titleCardSFX;
	

	bool inMenu = false;
	bool introReady = false;

	private AudioSource[] audioSources;
	private Animation anim;
	private Button[] buttons;

	private int selectedButtonIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
		audioSources = GetComponents<AudioSource>();
		anim = GetComponent<Animation>();
		buttons = GetComponentsInChildren<Button>();

        InitiateIntro();
    }

    // Update is called once per frame
    void Update()
    {
        if (introReady && !inMenu && Input.GetAxisRaw("Action1") > .2f && Input.GetAxisRaw("Action2") > .2f)
		{
			InitiateMenu();
		}
		else if (inMenu)
		{
			var pointer = new PointerEventData(EventSystem.current);

			//Click current button
			if (Input.GetAxisRaw("Action1") > .2f)
			{
				ExecuteEvents.Execute(buttons[selectedButtonIndex].gameObject, pointer, ExecuteEvents.submitHandler);
			}
			else if (Input.GetAxisRaw("Action1") > .2f) //Select next button (downwards)
			{
				//unhover current button
				ExecuteEvents.Execute(buttons[selectedButtonIndex].gameObject, pointer, ExecuteEvents.pointerExitHandler);

				//Update selected button index
				selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Length;

				//Hover next button
				ExecuteEvents.Execute(buttons[selectedButtonIndex].gameObject, pointer, ExecuteEvents.pointerEnterHandler);
			}
		}
    }

	void InitiateMenu()
	{
		if (inMenu)
			return;

		inMenu = true;

		anim.Play("menu_anim");
	}

	void InitiateIntro()
	{
		anim.Play("intro_anim");
	}

	void OnIntroAnimEnded()
	{
		anim.Play("flashingText_anim");
		introReady = true;
	}

	void BeginPlayingMusic()
	{
		audioSources[0].Play();
	}

	void PlaySFXTitleCard()
	{
		audioSources[1].clip = titleCardSFX;
		audioSources[1].Play();
	}

	public void Play()
	{
		SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
	}
}
