using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMN;

public class ItemPool : MonoBehaviour
{
    [SerializeField] AudioClip jellBulletSound = default;
    [SerializeField] AudioClip jellRecoverSound = default;
    [SerializeField] Jell jellBulletPrefab = default;
    [SerializeField] Jell jellRecoverPrefab = default;
    [SerializeField] int jellBulletNum = 10;
    [SerializeField] int jellRecoverNum = 10;

    private Transform playerform;
    private AudioSource audioSource;
    private List<Jell> jellBulletList;
    private List<Jell> jellRecoverList;


    private void Awake()
    {
        Locator<ItemPool>.Bind(this);
        audioSource = GetComponent<AudioSource>();

        playerform = GameObject.FindWithTag("Player").transform;
        SetPool(ref jellBulletList, jellBulletPrefab.gameObject, jellBulletNum);
        SetPool(ref jellRecoverList, jellRecoverPrefab.gameObject, jellRecoverNum);

        foreach (var el in jellBulletList)
        {
            el.OnCollideToTargetHandler = OnJBToTargetReciever;
        }
        foreach (var el in jellRecoverList)
        {
            el.OnCollideToTargetHandler = OnJRToTargetReciever;
        }
    }
    private void OnDestroy()
    {
        Locator<ItemPool>.Unbind(this);
    }


    public Jell GetJell()
    {
        Jell jell = Utility.Probability(85f) ? GetJellBullet() : GetJellRecover();
        if (jell != null)
        {
            return jell;
        }
        return null;
    }


    private JellBullet GetJellBullet()
    {
        foreach (var jell in jellBulletList)
        {
            if (!jell.gameObject.activeSelf)
            {
                return (JellBullet)jell;
            }
        }
        return null;
    }
    private JellRecover GetJellRecover()
    {
        foreach (var jell in jellRecoverList)
        {
            if (!jell.gameObject.activeSelf)
            {
                return (JellRecover)jell;
            }
        }
        return null;
    }
    private void SetPool(ref List<Jell> list, GameObject prefab, int num)
    {
        list = new List<Jell>(num);
        for (int i = 0; i < num; i++)
        {
            var obj = Instantiate(prefab, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            var jell = obj.GetComponent<Jell>();
            jell.OnGenerate(playerform);
            list.Add(jell);
        }
    }


    private void OnJBToTargetReciever()
    {
        audioSource.PlayOneShot(jellBulletSound);
    }
    private void OnJRToTargetReciever()
    {
        audioSource.PlayOneShot(jellRecoverSound);
    }
}
