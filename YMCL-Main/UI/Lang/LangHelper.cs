using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YMCL.Public;
using YMCL.Public.Class;

namespace YMCL.UI.Lang
{
    public class LangHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly LangHelper _current = new LangHelper();
        public static LangHelper Current => _current;

        readonly Main resource = new Main();
        public Main Resources => resource;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangedCulture(string name)
        {
            Main.Culture = CultureInfo.GetCultureInfo(name);
            this.RaisePropertyChanged("Resources");
        }

        public string GetText(string name,CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = Const.culture;
            }
            var res = Main.ResourceManager.GetObject(name, culture).ToString();
            if (res == null)
            {
                return "Null";
            }
            else
            {
                return res;
            }
        }
    }
}
