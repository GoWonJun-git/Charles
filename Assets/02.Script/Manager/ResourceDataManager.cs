using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDataManager : MonoBehaviour
{
    static private bool initState = false;
    static public GameObject Player;
    static public GameObject HPDownText;

    static public GameObject BossObject;
    static public GameObject Enemy;
    static public GameObject EnemyDie;

    static public GameObject Item1;
    static public GameObject Item2;
    static public GameObject Item3;
    static public GameObject Item4;

    static public GameObject EnergyObject;
    static public GameObject SlowEffect;
    static public GameObject FireObject;
    static public GameObject MachineGunObject;
    static public GameObject MachineGunObjectBullet;
    static public GameObject ShieldObject;
    static public GameObject ShieldObjectPlayer;


    public static void LoadResourcesData()
    {
        if(!initState)
        {
            initState = true;

            Player = Resources.Load("Player/Player") as GameObject;
            HPDownText = Resources.Load("Text/HPDown") as GameObject;

            BossObject = Resources.Load("Enemy/BossObject") as GameObject;
            Enemy = Resources.Load("Enemy/Enemy") as GameObject;
            EnemyDie = Resources.Load("Enemy/EnemyDie") as GameObject;
            
            Item1 = Resources.Load("ItemBase/Item1") as GameObject;
            Item2 = Resources.Load("ItemBase/Item2") as GameObject;
            Item3 = Resources.Load("ItemBase/Item3") as GameObject;
            Item4 = Resources.Load("ItemBase/Item4") as GameObject;
            
            EnergyObject = Resources.Load("ItemObject/EnergyObject") as GameObject;
            SlowEffect = Resources.Load("ItemObject/SlowEffect") as GameObject;
            FireObject = Resources.Load("ItemObject/FireObject") as GameObject;
            MachineGunObject = Resources.Load("ItemObject/MachineGunObject") as GameObject;
            MachineGunObjectBullet = Resources.Load("ItemObject/MachineGunObjectBullet") as GameObject;
            ShieldObject = Resources.Load("ItemObject/ShieldObject") as GameObject;
            ShieldObjectPlayer = Resources.Load("ItemObject/ShieldObjectPlayer") as GameObject;
        }
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position, Quaternion rotate)
    {
        GameObject obj = ObjectPoolManager.Instance.Instantiate(resource, position, rotate);
        T script = obj.GetComponent<T>();

        return script;
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position)
    {
        return CreateObjectAndComponent<T>(resource, position, Quaternion.identity);
    }

    public static T CreateObjectAndComponent<T>(GameObject resource)
    {
        return CreateObjectAndComponent<T>(resource, Vector3.zero, Quaternion.identity);
    }
}
