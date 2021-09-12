using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour 
    
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;

    private void Start() {
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
            
        }else{
            sceneCamera = Camera.main;
            if ( sceneCamera != null){
                sceneCamera.gameObject.SetActive(false);
            }   

            //Disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));   

            playerUIInstance = Instantiate(playerUIPrefab); 
            playerUIInstance.name = playerUIPrefab.name;    
        }  

        GetComponent<Player>().Setup(); 
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
    }

    

    void AssignRemoteLayer()
    {
        //tag other players as remote player
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++){
                componentsToDisable[i].enabled = false;
            }
    }

    private void OnDisable() {
        Destroy(playerUIInstance);

        if ( sceneCamera != null){
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}
