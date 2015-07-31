using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByDesign.Excelerator.Classes
{
    public class CodeFile
    {

        public string Path { get; set; }
        public string Content { get; set; }
        public string ProjectFile { get; set; }

        public bool CopyToProject { get; set; }

        public CodeFile() : this("", "") { }
        public CodeFile(string path) : this(path, "") { }
        
        public CodeFile(string path, string content)
        {
            this.Path = path;
            this.Content = content;
            this.CopyToProject = true;
            this.ProjectFile = "";
        }
    }
}
