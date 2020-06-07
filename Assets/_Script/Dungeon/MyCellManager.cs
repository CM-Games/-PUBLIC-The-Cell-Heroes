using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class MyCellManager : MonoBehaviour
{
    public int starCost;
    float moveForce;
    IEnumerator moveCellAddress;
    bool flag = false;

    private void Awake()
    {
       // StartCoroutine(moveCellAddress);
    }
    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!flag) {
            moveForce = transform.localPosition.x;

            moveCellAddress = moveCell();
            StartCoroutine(moveCellAddress);
            flag = true;
        }

    }

    IEnumerator moveCell()
    {
        while (true)
        {
            moveForce += 0.3f;
            transform.DOLocalMoveX(moveForce, 1f);
            yield return null;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTower"))
        {
            CellMakeManager.CMM.currentStarCost -= starCost;
            StopCoroutine(moveCellAddress);
            flag = false;
            transform.DOKill();
            gameObject.SetActive(false);
        }
    }

    public void setStarCost(int starCost) => this.starCost = starCost;
}
