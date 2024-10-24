using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EditDialogViewModel: BindableBase, IFormValidator
    {


        public bool IsValid => true;
    }
}
