using System;
using System.Collections.Generic;
using System.Text;

namespace Addon.Core.Models
{
    public interface IProgressable
    {
        int Progress{get;set; }
        string Message {get;set; }
        bool ShowMessage {get; }
    }
}
