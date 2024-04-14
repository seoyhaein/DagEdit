using Avalonia;
using Avalonia.Input;

namespace DagEdit
{
    public class PointerGesture
    {
        #region Fields

        private readonly KeyModifiers _keyModifiers;
        private readonly PointerUpdateKind _pointerUpdateKind;
        private readonly int _counter = 1; // 기본값으로 1을 설정

        #endregion

        #region Constructors

        public PointerGesture(PointerUpdateKind kind)
        {
            _pointerUpdateKind = kind;
            _keyModifiers = KeyModifiers.None;
        }

        public PointerGesture(PointerUpdateKind kind, int counter) : this(kind)
        {
            _counter = counter;
        }

        public PointerGesture(PointerUpdateKind kind, KeyModifiers modifiers) : this(kind)
        {
            _keyModifiers = modifiers;
        }

        public PointerGesture(PointerUpdateKind kind, KeyModifiers modifiers, int counter) : this(kind, modifiers)
        {
            _counter = counter;
        }

        #endregion

        #region Methods

        public bool Matches(object source, PointerEventArgs eventArgs)
        {
            // source가 Visual 타입이 아니면, 조건에 맞지 않으므로 바로 false 반환
            if (source is not Visual targetElement)
                return false;

            // 이벤트로부터 필요한 정보 추출
            var pointerProperties = eventArgs.GetCurrentPoint(targetElement).Properties;
            bool modifiersMatch = (eventArgs.KeyModifiers & _keyModifiers) == _keyModifiers; // 키 수정자가 일치하는지 검사
            bool pointerUpdateKindMatch =
                pointerProperties.PointerUpdateKind == _pointerUpdateKind; // 포인터 업데이트 종류가 일치하는지 검사

            // ClickCount를 검사하여 counterMatch 결정
            // eventArgs가 PointerPressedEventArgs 타입이고, ClickCount가 _counter와 다르면 false, 아니면 true
            bool counterMatch = eventArgs is not PointerPressedEventArgs pressedEventArgs ||
                                pressedEventArgs.ClickCount == _counter;

            // 모든 조건이 true일 때만 true 반환
            return modifiersMatch && pointerUpdateKindMatch && counterMatch;
        }

        #endregion
    }
}