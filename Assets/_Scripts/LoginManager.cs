using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField nameField;
    [SerializeField]
    TextMeshProUGUI warningText;

    private void Start()
    {
        this.nameField.ActivateInputField();
    }

    private bool ValidateName(string name)
    {
        if (name.All(char.IsLetterOrDigit))
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            this.SaveUsername();
        }
    }

    public void SaveUsername()
    {
        if (this.ValidateName(nameField.text) == true)
        {
            PlayerPrefs.SetString("name", nameField.text);
            SceneLoader.instance.LoadScene("MainMenu");
        }
        else
        {
            this.warningText.gameObject.SetActive(true);
            this.nameField.ActivateInputField();
        }
    }
}
