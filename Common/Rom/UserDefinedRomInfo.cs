using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Cereal64.Common.Rom
{
    public class UserDefinedRomInfo : IXMLSerializable, ITreeNodeElement
    {
        public const string USERDEFINEDINFO = "UserDefinedRomInfo";
        private const string GAMENAME = "GameName";
        private const string OTHERNOTES = "OtherNotes";

        [CategoryAttribute("User Info"),
        DescriptionAttribute("Name of the game")]
        public string GameName { get; set; }

        [CategoryAttribute("User Info"),
        DescriptionAttribute("Misc. info"),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string OtherNotes { get; set; }

        public UserDefinedRomInfo()
        {
            GameName = string.Empty;
            OtherNotes = string.Empty;
        }

        public UserDefinedRomInfo(XElement xml)
            : this()
        {
            if (xml.HasAttributes)
            {
                XAttribute attribute = xml.Attribute(GAMENAME);
                if (attribute != null)
                    GameName = attribute.Value;

                attribute = xml.Attribute(OTHERNOTES);
                if (attribute != null)
                    OtherNotes = attribute.Value;
            }
        }

        public XElement GetAsXML()
        {
            XElement info = new XElement(USERDEFINEDINFO);

            info.Add(new XAttribute(GAMENAME, GameName));
            info.Add(new XAttribute(OTHERNOTES, OtherNotes));

            return info;
        }

        public TreeNode GetAsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Text = "User Info";
            node.Tag = this;

            return node;
        }

    }
}
