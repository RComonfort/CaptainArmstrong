using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MenuScriptParent
{
	[Header("Main Menu")]
	[SerializeField] private GameObject mainMenu;

    void Update()
    {
        ButtonNavigationUpdate();
    }

	public void GoBack()
	{
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MenuController>().Init();
		gameObject.SetActive(false);
	}
}
