using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;

namespace OtterSauceGui
{
    /// <summary>
    /// This is the base class for creating an OtterSauceGui in Otter.
    /// Use AddWidjet to add widgets to this entity, then add the GuiManager object to your Scene after you have added the widjets.
    /// </summary>
    public class GuiManager : Entity
    {
        # region Private Fields

        private Game game;
        private List<List<GuiButton>> buttonGroups = null;
        private List<string> groupNames = null;

        #endregion

        #region Public Fields

        public bool isTyping = false;

        public bool isDisabled = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new GuiManager entity to add gui widjets to
        /// </summary>
        /// <param name="g">The instance of the game. Used for passing certain values to widjets</param>
        /// <param name="s">The Surface in your Scene you wish to render the gui to</param>
        public GuiManager(Game g, Surface s)
        {
            Surface = s;
            game = g;
            buttonGroups = new List<List<GuiButton>>();
            groupNames = new List<string>();
        }

        #endregion

        #region Entity Overrides

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();
            Surface.Render();
        }

        public override void Added()
        {
            base.Added();
            CreateGroups();
        }

        public override void Removed()
        {
            base.Removed();

            foreach (Component c in Components)
            {
                c.Removed();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this method to add buttons with the type ButtonType.RADIO to a group, so they can react to each other.
        /// The buttons may not function correctly if they are not ButtonType.RADIO
        /// </summary>
        /// <param name="name">The name of the group.  Each name represents a different group</param>
        /// <param name="button">The Button you wish to add</param>
        public void AddButtonToGroup(string name, GuiButton button)
        {
            bool nameExists = false;
            foreach (string n in groupNames)
            {
                if (n == name)
                {
                    nameExists = true;
                    break;
                }
            }

            if(!nameExists) groupNames.Add(name);
          
            button.groupName = name;
        }

        /// <summary>
        /// When you have created a widjet and definded its properties, Use this to add the Widjet component to 
        /// your GuiManager object
        /// </summary>
        /// <param name="w">The widjet you wish to add</param>
        public void AddWidjet(Widjet w)
        {
            w.SetGame(game);
            AddComponent(w);
        }

        public void DisableWidjets(bool disabled = true)
        {
            foreach (Widjet w in GetComponents<Widjet>())
            {
                w.isDisabled = disabled;
            }

            isDisabled = disabled;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a list for each group of radio buttons, and passes the list to each button in
        /// the group to make them aware of eachother
        /// </summary>
        private void CreateGroups()
        {
            foreach (string name in groupNames)
            {
                buttonGroups.Add(new List<GuiButton>());

                foreach (GuiButton b in GetComponents<GuiButton>())
                {
                    
                    if (b.groupName == name)
                    {
                        buttonGroups.Last().Add(b);
                    }
                }

                foreach (GuiButton b in GetComponents<GuiButton>())
                {
                    if (b.groupName == name)
                    {
                        b.buttonGroup = buttonGroups.Last();
                    }
                }
            }
        }
        /*
        /// <summary>
        /// Resets specific fields in each Widjet
        /// </summary>
        private void ResetValues()
        {
            foreach (Component c in Components)
            {
                if (c.GetType() == typeof(Widjet))
                {
                    Widjet w = (Widjet)c;
                    w.ResetEventFlags();
                    w.isActive = false;
                    w.hasClicked = false;
                }
            }
        }*/

        #endregion
    }
}
