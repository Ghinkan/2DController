using UnityEngine.UI;

namespace UnityEngine
{
    public class FlexibleGridLayout : LayoutGroup
    {
        private enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        [SerializeField] private FitType _fitType = FitType.Uniform;

        [SerializeField, Min(1)] private int _rows;
        [SerializeField, Min(1)] private int _columns;

        [SerializeField] private Vector2 _cellSize = new Vector2(100, 100);
        [SerializeField] private Vector2 _spacing;

        [SerializeField] private bool _fitX;
        [SerializeField] private bool _fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            int childCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf == true)
                    childCount++;
            }

            if (_fitType is FitType.Width or FitType.Height or FitType.Uniform)
            {
                float squareRoot = Mathf.Sqrt(childCount);
                _columns = _rows = Mathf.CeilToInt(squareRoot) == 0 ? 1 : Mathf.CeilToInt(squareRoot);
                switch (_fitType)
                {
                    case FitType.Width:
                        _fitX = true;
                        _fitY = false;
                        break;
                    case FitType.Height:
                        _fitX = false;
                        _fitY = true;
                        break;
                    case FitType.Uniform:
                        _fitX = _fitY = true;
                        break;
                }
            }

            if (_fitType is FitType.Width or FitType.FixedColumns)
            {
                _rows = Mathf.CeilToInt(childCount / (float)_columns) == 0 ? 1 : Mathf.CeilToInt(childCount / (float)_columns);
            }
            if (_fitType is FitType.Height or FitType.FixedRows)
            {
                _columns = Mathf.CeilToInt(childCount / (float)_rows) == 0 ? 1 : Mathf.CeilToInt(childCount / (float)_rows);
            }


            float totalWidth = rectTransform.rect.width;
            float totalHeight = rectTransform.rect.height;

            float cellMaxWidth = totalWidth / _columns - ((_spacing.x / _columns) * (_columns - 1))
                - (padding.left / (float)_columns) - (padding.right / (float)_columns);
            float cellMaxHeight = totalHeight / _rows - ((_spacing.y / _rows) * (_rows - 1))
                - (padding.top / (float)_rows) - (padding.bottom / (float)_rows);
            ;

            _cellSize.x = _fitX ? cellMaxWidth : _cellSize.x;
            _cellSize.y = _fitY ? cellMaxHeight : _cellSize.y;


            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / _columns;
                int columnCount = i % _columns;

                float columnWidth = _cellSize.x * _columns + _spacing.x * (_columns - 1);
                float rowHeight = _cellSize.y * _rows + _spacing.y * (_rows - 1);

                RectTransform child = rectChildren[i];

                float xPos = (_cellSize.x * columnCount) + (_spacing.x * columnCount) + padding.left - padding.right;
                float yPos = (_cellSize.y * rowCount) + (_spacing.y * rowCount) + padding.top - padding.bottom;

                float xLeft = xPos;
                float xCenter = (totalWidth - columnWidth) / 2 + xPos;
                float xRight = totalWidth - columnWidth + xPos;

                float yTop = yPos;
                float yCenter = (totalHeight - rowHeight) / 2 + yPos;
                float yBottom = totalHeight - rowHeight + yPos;

                switch (m_ChildAlignment)
                {
                    default:
                    case TextAnchor.UpperLeft:
                        xPos = xLeft;
                        yPos = yTop;
                        break;
                    case TextAnchor.UpperCenter:
                        xPos = xCenter;
                        break;
                    case TextAnchor.UpperRight:
                        xPos = xRight;
                        break;
                    case TextAnchor.MiddleLeft:
                        yPos = yCenter;
                        break;
                    case TextAnchor.MiddleCenter:
                        xPos = xCenter;
                        yPos = yCenter;
                        break;
                    case TextAnchor.MiddleRight:
                        xPos = xRight;
                        yPos = yCenter;
                        break;
                    case TextAnchor.LowerLeft:
                        yPos = yBottom;
                        break;
                    case TextAnchor.LowerCenter:
                        xPos = xCenter;
                        yPos = yBottom;
                        break;
                    case TextAnchor.LowerRight:
                        xPos = xRight;
                        yPos = yBottom;
                        break;
                }

                SetChildAlongAxis(child, 0, xPos, _cellSize.x);
                SetChildAlongAxis(child, 1, yPos, _cellSize.y);
            }
            SetLayoutInputForAxis(
                padding.horizontal + (_cellSize.x + _spacing.x) * _columns - _spacing.x,
                padding.horizontal + (_cellSize.x + _spacing.x) * _columns - _spacing.x,
                -1, 0);
            float minSpace = padding.vertical + (_cellSize.y + _spacing.y) * _rows - _spacing.y;
            SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
        }

        public override void CalculateLayoutInputVertical() { }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }
    }
}
