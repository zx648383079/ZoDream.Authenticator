using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 密码本.ViewModel
{
    public abstract class ViewModelBase:INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 当属性改变的时候，调用该方法来发起一个消息，通知View中绑定了propertyName的元素做出调整
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler !=null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
