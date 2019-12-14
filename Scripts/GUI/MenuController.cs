using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MenuController : MenuScriptParent
{
	[Header("SFX")]
	[SerializeField] private AudioClip titleCardSFX;

	[Header("Music")]
	[SerializeField] private AudioSource musicSource;
	
	[Header("Credits")]
	[SerializeField] private GameObject creditsScreen;
	
	bool inMenu = false;
	bool introReady = false;

	private Animation anim;
	

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();

		anim = GetComponent<Animation>();

		Init();
        InitiateIntro();
    }

    // Update is called once per frame
    void Update()
    {
        if (introReady && !inMenu && Input.GetAxisRaw("Action1") > .2f && Input.GetAxisRaw("Action2") > .2f)
		{
			InitiateMenu();
			lastInputPress = Time.timeSinceLevelLoad;

		}
		else if (inMenu)
		{
			ButtonNavigationUpdate();
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
		musicSource.Play();
	}

	void PlaySFXTitleCard()
	{
		sfxSource.clip = titleCardSFX;
		sfxSource.Play();
	}

	public void Play()
	{
		SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
	}

	public void ShowCredits()
	{
		creditsScreen.SetActive(true);
		creditsScreen.GetComponent<CreditsController>().Init();
		gameObject.SetActive(false);
	}

	
}
