using System;
using UnityEngine;

namespace Assets.oojjrs.oui
{
    public static class MyControl
    {
        public static class Audio
        {
            public static Action PlayCancelSfx { get; set; }
            public static Action PlayClickSfx { get; set; }
            public static Action PlayConfirmSfx { get; set; }
            public static Action PlayErrorSfx { get; set; }
            public static Action PlayHoverSfx { get; set; }
            public static Action PlayStartSfx { get; set; }
            public static Action PlaySwitchSfx { get; set; }
        }

        internal static class Dropdown
        {
            internal static event Action OnUpdate;

            internal static void Update()
            {
                OnUpdate?.Invoke();
            }
        }

        public static class Image
        {
            public static Sprite Null { get; set; }
        }

        internal static class Selector
        {
            internal static event Action OnUpdate;

            internal static void Update()
            {
                OnUpdate?.Invoke();
            }
        }

        public static bool Texting { get; internal set; }

        public static void Update()
        {
            Dropdown.Update();
            Selector.Update();
        }
    }
}
