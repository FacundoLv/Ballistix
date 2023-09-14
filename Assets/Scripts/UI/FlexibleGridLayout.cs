using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
    }

    public Vector2 spacing;
    public FitType fitType;

    public bool fitX;
    public bool fitY;

    public int rows;
    public int columns;

    private Vector2 cellSize;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        rows = rows > 1 ? rows : 1;
        columns = columns > 1 ? columns : 1;

        float sqrt = Mathf.Sqrt(transform.childCount);
        switch (fitType)
        {
            case FitType.Uniform:
                SetFit();
                rows = Mathf.CeilToInt(sqrt);
                columns = Mathf.CeilToInt(sqrt);
                break;
            case FitType.Width:
                SetFit();
                columns = Mathf.CeilToInt(sqrt);
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            case FitType.Height:
                SetFit();
                rows = Mathf.CeilToInt(sqrt);
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.FixedRows:
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.FixedColumns:
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            default:
                break;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    private void SetFit()
    {
        fitX = true;
        fitY = true;
    }

    #region LAYOUTGROUP
    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
    #endregion
}
