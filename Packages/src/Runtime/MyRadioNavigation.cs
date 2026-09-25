using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MyRadio))]
    public sealed class MyRadioNavigation : UnityEngine.UI.Selectable
    {
        private MyRadio _radio;

        public override bool IsInteractable()
        {
            if (_radio == null)
                _radio = GetComponent<MyRadio>();

            return base.IsInteractable() && _radio.IsInteractable;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
        }
    }
}
