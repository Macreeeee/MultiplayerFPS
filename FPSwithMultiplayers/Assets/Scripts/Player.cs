
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar] //Syncronize all players's hp in all clients
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public void Setup() {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefault();
    }

    private void Update() {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage( 9999 );
            Debug.Log(transform.name + " killed himself");
        }
        
    }

    

    [ClientRpc]
    public void RpcTakeDamage(int _damage)
    {
        if ( isDead ) return;
        currentHealth -= _damage;

        Debug.Log(transform.name + " took damage and has health: " + currentHealth);

        if (currentHealth <= 0){
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if ( _col != null )
        {
            _col.enabled = false;
        }

        Debug.Log(transform.name + " is died");

        StartCoroutine(Respawn());

    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        SetDefault();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned");
    }

    public void SetDefault()
    {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if ( _col != null )
        {
            _col.enabled = true;
        }

        Debug.Log(transform.name + " appears!");
    }

}
