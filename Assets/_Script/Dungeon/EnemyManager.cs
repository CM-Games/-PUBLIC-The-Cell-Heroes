using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public Slider enemyHP;
    bool flag;

    // Start is called before the first frame update
    void Start()
    {
        flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHP.value < 0.05 && !flag)
        {
            successDungeon();
            flag = true;
        }

    }

    public void successDungeon()
    {
        float temp;

        for (int i = 0; i < SystemDB.DungeonClear.Length; i++)
        {
            if(SystemDB.DungeonClear[i] == 0)
            {
                SystemDB.DungeonClear[i] = 1;
                break;
            }

        }
        User.exp += 0.6f;

        if (User.exp > 1)
        {
            temp = User.exp - 1;
            User.exp = temp;
            User.level++;
        }
        AllManager.AM.DM.giveMoney(100);
        AllManager.AM.DM.saveUserInfo();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyHP.value -= 0.2f;
    }
}
