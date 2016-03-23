using UnityEngine;
using System.Collections;


[System.Serializable]
public class TileStyle
{
    public int Number;          // 显示的数字
    public Color32 TileColor;   // 显示的数字的颜色
    public Color32 TextColor;   // 显示的数字的背景颜色
}

public class TileStyleHolder : MonoBehaviour
{
    // 单例
    public static TileStyleHolder Instance;

    public TileStyle[] TileStyles;     // 会在编辑器的 Inspector 上赋值 

    void Awake()
    {
        Instance = this;
    }
}
