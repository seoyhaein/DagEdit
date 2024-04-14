using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace DagEdit
{
    public class MultiGesture
    {
        public enum Match
        {
            Any,
            All
        }

        private readonly object[] _gestures;
        private readonly Match _match;

        public MultiGesture(Match match, params object[] gestures)
        {
            _gestures = gestures ?? throw new ArgumentNullException(nameof(gestures));
            _match = match;
        }

        public bool Matches(object targetElement, RoutedEventArgs eventArgs)
        {
            var pointerEventArgs = eventArgs as PointerEventArgs;
            var keyEventArgs = eventArgs as KeyEventArgs;

            if (_match == Match.Any)
            {
                foreach (var gesture in _gestures)
                {
                    if ((gesture is PointerGesture pointerGesture && pointerEventArgs != null &&
                         pointerGesture.Matches(targetElement, pointerEventArgs)) ||
                        (gesture is KeyGesture keyGesture && keyEventArgs != null && keyGesture.Matches(keyEventArgs)))
                    {
                        return true;
                    }
                }

                return false;
            }
            else // Match.All
            {
                foreach (var gesture in _gestures)
                {
                    if ((gesture is PointerGesture pointerGesture && (pointerEventArgs == null ||
                                                                      !pointerGesture.Matches(targetElement,
                                                                          pointerEventArgs))) ||
                        (gesture is KeyGesture keyGesture &&
                         (keyEventArgs == null || !keyGesture.Matches(keyEventArgs))))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}