using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public Number number;

    public bool IsHaveNumber()
    {
        return number != null;
    }

    public Number GetNumber()
    {
        return number;
    }

    public void SetNumber(Number num)
    {
        this.number = num;
    }
}
