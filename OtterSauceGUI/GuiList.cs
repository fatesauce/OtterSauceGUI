using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterSauceGui
{
    /// <summary>
    /// WIP
    /// The class to contain and manage GuiListElement widgets
    /// 
    /// TODO:
    /// 
    /// 
    /// </summary>
    public class GuiList : Widget
    {

        List<GuiListElement> _listElements;

        public GuiList(int px, int py, int w, int h)
            : base(px, py, w, h)
        {

        }
    }
}
