using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomChance : Singleton<RandomChance>
{
    private void Start()
    {

    }
    public void GetRandomItem(int _item)
    {
        List<int> categoryItem = new List<int>();

        //create item
        for (int i = 1; i <= _item; i++)
        {
            categoryItem.Add(i);
        }
        foreach (int _i in categoryItem)//Debug
        {
            Debug.Log(_i);
        }
    }
    public bool GetRandomChance(int _chanceItemA)
    {
        List<int> item = new List<int>();
        int[] itemSwapOrder = new int[100];
        int _chanceItemB = 100 - _chanceItemA;
        //Create Item A
        for (int itemA = 1; itemA <= _chanceItemA; itemA++)
        {
            item.Add(1);
        }
        //Create Item B
        for (int itemB = 1; itemB <= _chanceItemB; itemB++)
        {
            item.Add(2);
        }
        //Shuffle Item Order
        item = item.OrderBy(x => Random.value).ToList();
        // for (int o = 0; o < item.Count; o++)
        // {
        //     Debug.Log(item[o] + " : " + o);
        // }
        int rndItem = Random.Range(0, 100);
        // Debug.Log(itemSwapOrder[rndItem]);
        if (item[rndItem] == 1)
        {
            return true;
        }
        return false;
    }
}