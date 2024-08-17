using System;
using UnityEngine;

namespace oojjrs.oui
{
    public partial class MyDropdown
    {
        [Serializable]
        public class OptionData
        {
            [SerializeField]
            private string m_Text;

            [SerializeField]
            private Sprite m_Image;

            public string text
            {
                get
                {
                    return m_Text;
                }
                set
                {
                    m_Text = value;
                }
            }

            public Sprite image
            {
                get
                {
                    return m_Image;
                }
                set
                {
                    m_Image = value;
                }
            }

            public OptionData()
            {
            }

            public OptionData(string text)
            {
                this.text = text;
            }

            public OptionData(Sprite image)
            {
                this.image = image;
            }

            public OptionData(string text, Sprite image)
            {
                this.text = text;
                this.image = image;
            }
        }
    }
}
