using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace oojjrs.oui
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

        public static class Image
        {
            public static Sprite Null { get; set; }
        }

        public static bool IsQuitting { get; private set; }
        public static bool IsTexting => MyInputs.Any() && MyInputs.All(t => t.IsFocused);
        internal static HashSet<MyInput> MyInputs { get; } = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetQuitting()
        {
            IsQuitting = false;

            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }

        private static void OnQuitting()
        {
            IsQuitting = true;
        }
    }
}
