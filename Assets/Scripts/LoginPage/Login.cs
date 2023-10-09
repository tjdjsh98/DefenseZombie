using System.Collections;
using System.Collections.Generic;
using System.Net;
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

        string clientName = "Client";
        if (GameObject.Find(clientName) == null)
        {
            temp = new GameObject(clientName);
            DontDestroyOnLoad(temp);
            Client client = temp.AddComponent<Client>();

            string strHostName = string.Empty;
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;

            client.Init(addr[1].ToString());
        }
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
        string clientName = "Client";
        if (GameObject.Find(clientName) == null){
            GameObject temp = new GameObject(clientName);
            DontDestroyOnLoad(temp);
            Client client = temp.AddComponent<Client>();
            string ip = _inputField.text.TrimEnd();
            ip = ip.Remove(ip.Length - 1, 1);
            client.Init(ip);
        }
    }
}
