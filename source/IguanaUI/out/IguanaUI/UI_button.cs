/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace IguanaUI
{
    public partial class UI_button : GButton
    {
        public Controller m_button;
        public GImage m_n1;
        public GImage m_n2;
        public GImage m_n3;
        public GTextField m_title;
        public GImage m_n7;
        public const string URL = "ui://2fevh8bimtkjb2";

        public static UI_button CreateInstance()
        {
            return (UI_button)UIPackage.CreateObject("IguanaUI", "button");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_button = GetControllerAt(0);
            m_n1 = (GImage)GetChildAt(0);
            m_n2 = (GImage)GetChildAt(1);
            m_n3 = (GImage)GetChildAt(2);
            m_title = (GTextField)GetChildAt(3);
            m_n7 = (GImage)GetChildAt(4);
        }
    }
}