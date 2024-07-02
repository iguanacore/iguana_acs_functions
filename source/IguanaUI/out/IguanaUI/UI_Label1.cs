/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace IguanaUI
{
    public partial class UI_Label1 : GButton
    {
        public Controller m_button;
        public GImage m_n82;
        public GImage m_n83;
        public GTextField m_title;
        public const string URL = "ui://2fevh8bimtkj3i";

        public static UI_Label1 CreateInstance()
        {
            return (UI_Label1)UIPackage.CreateObject("IguanaUI", "Label1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_button = GetControllerAt(0);
            m_n82 = (GImage)GetChildAt(0);
            m_n83 = (GImage)GetChildAt(1);
            m_title = (GTextField)GetChildAt(2);
        }
    }
}