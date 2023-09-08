using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]GameObject _loginPage;
    [SerializeField]TextMeshProUGUI _inputField;

    private void Start()
    {
        _loginPage.SetActive(false);
    }
    public void CreateServer()
    {
        GameObject temp = new GameObject("Server");
        DontDestroyOnLoad(temp);
        Server server = temp.AddComponent<Server>();
        server.Init();

        SceneManager.LoadScene("InGame");

    }

    public void OpenLoginPage()
    {
        _loginPage.SetActive(true);
    }
    public void CloseLoginPage()
    {
        _loginPage.SetActive(false);
    }

    public void LoginClient()
    {
        GameObject temp = new GameObject("Client");
        DontDestroyOnLoad(temp);
        Client client = temp.AddComponent<Client>();
        client.Init(_inputField.text);
        SceneManager.LoadScene("InGame");
    }
}
