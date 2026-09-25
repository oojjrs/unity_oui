using UnityEngine;

namespace oojjrs.oui
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MyRadio))]
    public sealed class MyRadioNavigation : UnityEngine.UI.Selectable
    {
        private MyRadio _radio;

        protected override void Awake()
        {
            base.Awake();

            _radio = GetComponent<MyRadio>();
        }

        public override bool IsInteractable()
        {
            return base.IsInteractable() && _radio.IsInteractable;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
        }
    }
}
