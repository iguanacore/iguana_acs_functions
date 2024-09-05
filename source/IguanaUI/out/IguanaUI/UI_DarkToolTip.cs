/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace IguanaUI
{
    public partial class UI_DarkToolTip : GLabel
    {
        public Controller m_lang;
        public GImage m_n0;
        public GRichTextField m_title;
        public Transition m_anim;
        public const string URL = "ui://2fevh8biu5n81";

        public static UI_DarkToolTip CreateInstance()
        {
            return (UI_DarkToolTip)UIPackage.CreateObject("IguanaUI", "DarkToolTip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_lang = GetControllerAt(0);
            m_n0 = (GImage)GetChildAt(0);
            m_title = (GRichTextField)GetChildAt(1);
            m_anim = GetTransitionAt(0);
        }
    }
}