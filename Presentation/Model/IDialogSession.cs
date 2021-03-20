using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model
{
    public interface IDialogSession
    {
        bool IsEnded { get; }
        object Content { get; }
        void Close();
    }
}
