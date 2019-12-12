using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipComponentsProgress : MonoBehaviour
{
    private Player player;

	private Dictionary<EShipComponent, TextMeshPro> componentTexts;

    // Start is called before the first frame update
    void Start()
    {
		componentTexts = new Dictionary<EShipComponent, TextMeshPro>();

        player = Object.FindObjectOfType<Player>();

		//Get template to use in vertical stack
		GameObject componentGUI = transform.GetChild(0).gameObject;

		for (int i = 0; i < player.RequiredComponents.Length; i++)
		{
			EShipComponent type = player.RequiredComponents[i].component;

			//Add GUI template for the ship component progress: ([0]icon, [1]text)
			GameObject newComponentGUI = Instantiate(componentGUI);
			newComponentGUI.transform.SetParent(transform);

			//Copy sprite that represents the ship component
			Sprite compSprite = player.RequiredComponents[i].prefab.GetComponent<SpriteRenderer>().sprite;
			newComponentGUI.transform.GetChild(0).GetComponent<Image>().sprite = compSprite;

			//Save the ship component type to its text in the GUI
			componentTexts.Add(type, newComponentGUI.transform.GetChild(1).GetComponent<TextMeshPro>());
		}

		Destroy(componentGUI);
    }

    void Update()
    {
		//Redraw the ship components player has: collected/needs
        foreach (RequiredComponent reqComp in player.RequiredComponents)
		{
			EShipComponent type = reqComp.component;

			int has = player.ObtainedComps[type];
			int needs = player.NeededComps[type];

			TextMeshPro tmPRO = componentTexts[type];
			tmPRO.text = "<sup>" + has + "</sup><size=90%>/<size=100%><sub>" + needs + "</sub>";
		}
    }
}
