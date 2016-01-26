using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cereal64.Common
{
    public class RomProject
    {
        private static RomProject _instance = null;
        private static object syncObject = new object();

        public string ProjectName { get; set; }

        public static RomProject Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new RomProject();
                    }
                }
                return _instance;
            }
        }

        public ReadOnlyCollection<RomFile> Files { get { return _files.AsReadOnly(); } }
        private List<RomFile> _files;

        private RomProject()
        {
            _files = new List<RomFile>();
            ProjectName = "New Rom Project";
        }
    }
}
