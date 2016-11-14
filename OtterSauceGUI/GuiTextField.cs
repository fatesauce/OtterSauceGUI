using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;

namespace OtterSauceGui
{
    /// <summary>
    /// WIP 
    /// Multi line text area.
    /// 
    /// TODO:
    /// Word wrapping
    /// Scrolling
    /// Organising multiple lines of text
    /// </summary>
    public class GuiTextField : Widget
    {

        private List<Text> TextDisplay;
        private int TextSize;
        private string TextFont;
        private Image TextFieldImage;

        public GuiTextField(int px, int py, int w, int h, int size = 12, string font = "")
            : base(px, py, w, h)
        {
            TextSize = size;
            TextFont = font;
        }
    }
}
