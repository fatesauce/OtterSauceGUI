using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;

namespace OtterSauceGui
{
    /// <summary>
    /// A simple stand alone entity.  This is not a widjet and must be added to your Scene like any other entity.
    /// I recommend adding it to the same surface as your GuiManager object, and adding it to the scene after the GuiManager.
    /// The ONLY purpose of this class is to give you the ability to create a custom graphical mouse cursor, by assigning a Graphic 
    /// to a GuiPointer object.
    /// For best results, set MouseVisible to false, to hide the OS mouse cursor.
    /// </summary>
    public class GuiPointer : Entity
    {
        public GuiPointer()
        {
        }

        public override void Update()
        {
            base.Update();
            Layer = -11;
            X = Scene.MouseX;
            Y = Scene.MouseY;
        }
    }
}
