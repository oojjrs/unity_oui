using System;
using UnityEngine.Events;

namespace oojjrs.oui
{
    public partial class MyDropdown
    {
        [Serializable]
        public class DropdownEvent : UnityEvent<int>
        {
        }
    }
}
