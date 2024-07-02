/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace IguanaUI
{
    public partial class UI_LabelsList : GLabel
    {
        public Controller m_button;
        public Controller m_button_2;
        public Controller m_button_3;
        public GImage m_n7;
        public GGraph m_contentArea;
        public GList m_n8;
        public GList m_filter_mode;
        public GTextField m_n26;
        public UI_button m_n29;
        public GImage m_n30;
        public GTextField m_n31;
        public UI_button m_n32;
        public const string URL = "ui://2fevh8biv6073f";

        public static UI_LabelsList CreateInstance()
        {
            return (UI_LabelsList)UIPackage.CreateObject("IguanaUI", "LabelsList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_button = GetControllerAt(0);
            m_button_2 = GetControllerAt(1);
            m_button_3 = GetControllerAt(2);
            m_n7 = (GImage)GetChildAt(0);
            m_contentArea = (GGraph)GetChildAt(1);
            m_n8 = (GList)GetChildAt(2);
            m_filter_mode = (GList)GetChildAt(3);
            m_n26 = (GTextField)GetChildAt(4);
            m_n29 = (UI_button)GetChildAt(5);
            m_n30 = (GImage)GetChildAt(6);
            m_n31 = (GTextField)GetChildAt(7);
            m_n32 = (UI_button)GetChildAt(8);
        }
    }
}