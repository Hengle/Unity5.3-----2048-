using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public bool mergedThisTurn = false;   // 本回合是否发生合并
    public int indRow;      // todo 要在编辑器中手动赋值？ 肯定不能这样开发呀！//
    public int indCol;

    public int Number
    {
        get { return number; }
        set
        {
            number = value;
            if (number == 0)
            {
                SetEmpty();
            }
            else
            {
                ApplyStyle(number);
                SetVisible();
            }
        }
    }

    private int number;

    private Text titeText;
    private Image tileImage;
    
    private Animator anim;

    void Awake()
    {
        // todo 这样 查找并不好，所以使用 Inspector上 进行赋值的方式
        // 1 为什么不好？   查找就是  遍历， 遍历就需要时间， 即使不是在Update中进行的。
        // 带来的问题 为16个对象 进行相同的 赋值会不会特别麻烦？
        // 解决：（1）只对一个进行复制 然后 拷贝出 15个 就行了。
        // （2） 使用 Reset()  然后进行赋值
        titeText = GetComponentInChildren<Text>();
        tileImage = transform.Find("NumberedCell").GetComponent<Image>();
        anim = gameObject.GetComponent<Animator>();
    }
    
    public void PlayMergedAnimation()
    {
        anim.SetTrigger("Merge");
    }
    public void PlayAppearAnimation()
    {
        anim.SetTrigger("Appear");
    }

    void ApplyStyleFromHolder(int index)
    {
        titeText.text = TileStyleHolder.Instance.TileStyles[index].Number.ToString();
        titeText.color = TileStyleHolder.Instance.TileStyles[index].TextColor;
        tileImage.color = TileStyleHolder.Instance.TileStyles[index].TileColor;
    }


    // todo 这个 转换 是不是有更好的 方式 解决呢？
    void ApplyStyle(int num)
    {
        switch (num)
        {
            case 2:
                ApplyStyleFromHolder(0);
                break;
            case 4:
                ApplyStyleFromHolder(1);
                break;
            case 8:
                ApplyStyleFromHolder(2);
                break;
            case 16:
                ApplyStyleFromHolder(3);
                break;
            case 32:
                ApplyStyleFromHolder(4);
                break;
            case 64:
                ApplyStyleFromHolder(5);
                break;
            case 128:
                ApplyStyleFromHolder(6);
                break;
            case 256:
                ApplyStyleFromHolder(7);
                break;
            case 512:
                ApplyStyleFromHolder(8);
                break;
            case 1024:
                ApplyStyleFromHolder(9);
                break;
            case 2048:
                ApplyStyleFromHolder(10);
                break;
            case 4096:
                ApplyStyleFromHolder(11);
                break;
            default:
                Debug.LogError("未经处理的类型： " + num);    // 好的编程习惯这个default要有
                break;
        }
    }

    // 显示
    private void SetVisible()
    {
        tileImage.enabled = true;
        titeText.enabled = true;
    }
    
    // 隐藏
    private void SetEmpty()
    {
        tileImage.enabled = false;
        titeText.enabled = false;
    }
}
