using UnityEngine;
using UnityEngine.UI;

public class ObjectInfo : MonoBehaviour
{
  public GameObject IconCamera;
  public CanvasGroup InfoPanel;

  public bool isSelected = false;
  public string objectName;
  public Text nameDisplay;

  public int health;
  public int maxHealth;
  public Slider healthBar;
  public Text healthDisplay;

  public int energy;
  public int maxenergy;
  public Slider energyBar;
  public Text energyDisplay;

  public int physicalAttack;
  public Text physicalAttackDisplay;
  public int physicalDefense;
  public Text physicalDefenseDisplay;
  public int energyAttack;
  public Text energyAttackDisplay;
  public int energyDefense;
  public Text energyDefenseDisplay;

  public enum Ranks { Apprentice, Journeyman, Master }
  public Text rankDisplay;
  public Ranks rank;

  public int kills;
  public Text killsDisplay;

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    if (maxenergy <= 0)
    {
      energyBar.gameObject.SetActive(false);
    }

    if (health <= 0)
    {
      Destroy(gameObject);
    }

    healthDisplay.text = "" + health + "/" + maxHealth;
    healthBar.maxValue = maxHealth;
    healthBar.value = health;

    energyDisplay.text = "" + energy + "/" + maxenergy;
    energyBar.maxValue = maxenergy;
    energyBar.value = energy;

    physicalAttackDisplay.text = "P Atk: " + physicalAttack;
    physicalDefenseDisplay.text = "P Def: " + physicalDefense;
    energyAttackDisplay.text = "E Atk: " + energyAttack;
    energyDefenseDisplay.text = "E Def: " + energyDefense;
    rankDisplay.text = rank.ToString();
    killsDisplay.text = "Kills: " + kills;


    nameDisplay.text = objectName;
    IconCamera.SetActive(isSelected);

    if (isSelected)
    {
      InfoPanel.alpha = 1;
      InfoPanel.blocksRaycasts = true;
      InfoPanel.interactable = true;
    }
    else
    {
      InfoPanel.alpha = 0;
      InfoPanel.blocksRaycasts = false;
      InfoPanel.interactable = false;
    }
  }
}