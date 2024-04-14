using System;
using Avalonia;
using Avalonia.Controls;

namespace DagEdit
{
    /*
     * TemplateLayoutCanvas 이 녀석은 ControlTemplate 안에서만 사용해야 한다.
     * 다른 곳에서 사용할 경우 오작동을? 일으킬 수 있다. 
     */
    public class TemplateLayoutCanvas : Canvas
    {
        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            double maxWidth = 0.0;
            double maxHeight = 0.0;

            // TODO
            // 만약 w / h 설정되어 있으면 잘못된 방식으로 나타날텐데, 이문제를 어떻게 해결할지 생각해야함.
            // 고정적으로 코드에 넣어두면 안될 것 같은데....
            foreach (var child in Children)
            {
                child.Measure(constraint);

                // 자식 컨트롤이 ILocatable 인터페이스를 구현하는 경우, Location 속성을 사용
                Point location = child is ILocatable locatableChild ? locatableChild.Location : Constants.ZeroPoint;

                double childRight = location.X + child.DesiredSize.Width;
                double childBottom = location.Y + child.DesiredSize.Height;

                maxWidth = Math.Max(maxWidth, childRight);
                maxHeight = Math.Max(maxHeight, childBottom);
            }

            return new Size(maxWidth, maxHeight);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
            {
                // ILocatable 인터페이스를 구현하는지 확인
                if (child is ILocatable locatableChild)
                {
                    Point location = locatableChild.Location;

                    //child.Arrange(new Rect(location.X, location.Y, child.DesiredSize.Width+20, child.DesiredSize.Height+20));
                    child.Arrange(new Rect(location, child.DesiredSize));
                }
                else
                {
                    // ILocatable을 구현하지 않는 경우, 기본 위치나 다른 로직을 사용하여 Arrange를 수행
                    // 기본 위치를 (0, 0)으로 설정
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }

            return finalSize;
        }
    }
}