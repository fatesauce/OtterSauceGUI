using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;
using OtterSauceGui;

namespace OtterSauceGui
{
    public class GuiContainer : Component
    {

        List<Image> _tiles;

        public Image ContainerImage;

        int _posX;
        int _posY;
        
        public GuiContainer(int px, int py) 
        {
            _posX = px;
            _posY = py;
        }

        public override void Added()
        {
            base.Added();
        }

        public override void Render()
        {
            base.Render();
            if (ContainerImage != null)
            {
                ContainerImage.SetPosition(_posX, _posY);
                Draw.Graphic(ContainerImage);
            }
            if (_tiles != null)
            {
                foreach (Image t in _tiles)
                {
                    Draw.Graphic(t);
                }
            }
        }

        public void AddTile(string tilepath, int w, int h, int cols, int rows)
        {
            _tiles = new List<Image>();

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    int x = _posX + (c * w);
                    int y = _posY + (r * h);
                    _tiles.Add(new Image(tilepath));
                    _tiles.Last().SetPosition(x, y);
                }
            }
            
        }
    }
}
