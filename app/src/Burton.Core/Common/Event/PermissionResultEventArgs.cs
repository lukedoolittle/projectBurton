using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Common
{
    public class PermissionResultEventArgs : EventArgs
    {
        public int RequestCode { get; set; }
        public string[] Permissions { get; set; }
        public Permission[] GrantResults { get; set; }
    }
}
