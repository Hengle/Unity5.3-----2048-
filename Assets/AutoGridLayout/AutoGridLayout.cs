using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


namespace UnityEditor.UI
{

    [ExecuteInEditMode]
    [AddComponentMenu("Layout/Auto Grid Layout Group", 152)]

    /// <summary>
    /// 扩展了 GridLayoutGroup 组件的 功能，使 Cell Size 大小是根据父容器和Padding、Spaceing等改变大小
    /// </summary>
    public class AutoGridLayout : GridLayoutGroup
    {
        [SerializeField] private bool m_IsColumn;
        [SerializeField] private int m_Column = 1, m_Row = 1;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            constraintCount = constraintCount;
        }

#endif

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float iColumn = -1;
            float iRow = -1;

            if (m_IsColumn)
            {
                iColumn = m_Column;
                if (iColumn <= 0)
                {
                    iColumn = 1;
                }
                iRow = Mathf.CeilToInt(this.transform.childCount/iColumn);
            }
            else
            {
                iRow = m_Row;
                if (iRow <= 0)
                {
                    iRow = 1;
                }
                iColumn = Mathf.CeilToInt(this.transform.childCount/iRow);
            }

            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            cellSize = new Vector2((width - m_Padding.horizontal + m_Spacing.x)/iColumn - m_Spacing.x,
                (height - m_Padding.vertical + m_Spacing.y)/iRow - m_Spacing.y);
        }
    }
}