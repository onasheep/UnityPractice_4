using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ???
using UnityEditor;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance_;

    public static ResourceManager instance
    {
        get
        {
            if(instance_ == null)
            {
                instance_ = FindObjectOfType<ResourceManager>();
            }
            return instance_;
        }
    }

    private static string zombieDataPath = default;
    public ZombieData zombieData_default = default;
    private void Awake()
    {
        zombieDataPath = "Scriptables";

        zombieDataPath = string.Format("{0}/{1}", zombieDataPath, "Zombie Default");

        zombieData_default = Resources.Load<ZombieData>(zombieDataPath);
        //ZombieData zombieData_ = Resources.Load<ZombieData>(zombieDataPath);

        //Debug.LogFormat("Zombie data path : {0}", zombieDataPath);
        //Debug.LogFormat("Zombid data: {0}, {1}, {2}", zombieData_.health, zombieData_.damage, zombieData_.speed);

        Debug.LogFormat("게임 save data를 여기에다가 저장하는 것이 상식이다. -> {0}", Application.persistentDataPath);
    }
}
