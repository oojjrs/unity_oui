using System;
using System.Collections.Generic;
using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyDropdown
    {
        [Serializable]
        public class OptionDataList
        {
            [SerializeField]
            private List<OptionData> m_Options;

            public List<OptionData> options
            {
                get
                {
                    return m_Options;
                }
                set
                {
                    m_Options = value;
                }
            }

            public OptionDataList()
            {
                options = new List<OptionData>();
            }
        }
    }
}
