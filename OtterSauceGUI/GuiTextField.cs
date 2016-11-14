using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;

namespace OtterSauceGui
{
    public class GuiTextField : Widjet
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
