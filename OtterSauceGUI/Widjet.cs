using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;

namespace OtterSauceGui
{
    /// <summary>
    /// The base class for all Widjets.  Do not use this class directly.  
    /// Contains information and rules for how subclasses should behave when certain things happen in your game,
    /// as well as primary information that all subclassed Widjets should have.
    /// </summary>
    public class Widjet : Component
    {

        #region Private Fields

        /// <summary>
        /// If a shortcut is defined, this will be set to true and the widjet will listen for the key to be pressed.
        /// Do not use shortcuts if there are GuiTextBoxes in your gui
        /// </summary>
        private bool isShortcutSet = false;

        /// <summary>
        /// The Key to be used as the shortcut key for this widjet.
        /// </summary>
        private Key _shortcut;

        /// <summary>
        /// A text label to display next to your Widjet
        /// </summary>
        private Text _label;

        #endregion

        #region Protected Fields

        /// <summary>
        /// Current game instance
        /// </summary>
        protected Game game;

        /// <summary>
        /// Width of the widjet
        /// </summary>
        protected int Width;

        /// <summary>
        /// Height of the widjet
        /// </summary>
        protected int Height;

        /// <summary>
        /// X coordinate relative to its surface
        /// </summary>
        protected int PosX;

        /// <summary>
        /// Y coordinate relative to its surface
        /// </summary>
        protected int PosY;

        /// <summary>
        /// How long a "click" graphic should disply, in frames, before firing the next event
        /// </summary>
        protected int clickTimer = 15;

        /// <summary>
        /// Counts the frames when clicked
        /// </summary>
        protected int clickCounter = 0;

        /// <summary>
        /// Used for ButtonType.DOWNABLE GuiButtons.  If true, will repeatedly fire OnClickEvent when the left mouse button
        /// is held down on a widjet.  If false, will only fire OnClickEvent once when the button is clicked.
        /// </summary>
        protected bool isDownable = false;

        /// <summary>
        /// Flag used to tell other events if the OnClickEvent has been invoked
        /// </summary>
        protected bool hasOnClickFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnActiveEvent has been invoked
        /// </summary>
        protected bool hasOnActiveFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnInactiveEvent has been invoked
        /// </summary>
        protected bool hasOnInactiveFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnHoverEvent has been invoked
        /// </summary>
        protected bool hasOnHoverFired = false;

        #endregion

        #region Public Fields

        /// <summary>
        /// If a widjet has just been clicked, it becomes "active".  
        /// There can only be one active widjet at any time regardless of grouping, or which GuiManager the widjet belongs to
        /// </summary>
        public bool isActive = false;

        /// <summary>
        /// Set to true to disable input on this widjet
        /// </summary>
        public bool isDisabled = false;

        /// <summary>
        /// If a widjet has just been clicked
        /// </summary>
        public bool hasClicked = false;

        public Sound HoverSound;

        public Sound ClickSound;

        #endregion

        #region Events

        /// <summary>
        /// Fired when a user clicks a widjet.  In most cases it will only fire ONCE per click.
        /// If the widjet field "isDownable" is true, it will fire EVERY FRAME while the mouse button is held over it.
        /// </summary>
        public event EventHandler OnClickEvent;

        /// <summary>
        /// Fired ONCE when a widjet becomes active.  A widjet becomes active after the user clicks on it.  Only one widjet can be active in the game scene at any one time.
        /// Used mainly for changing widjet graphics based on the isActive flag, and for choosing which GuiTextBox should allow input.
        /// </summary>
        public event EventHandler OnActiveEvent;

        /// <summary>
        /// Fires ONCE when a widjet becomes inactive.  Used mainly for preventing GuiTextBox input and changing widjet graphics.
        /// </summary>
        public event EventHandler OnInactiveEvent;

        /// <summary>
        /// Fires ONCE when a users mouse hovors over the widjet.  Used mainly for changing the widjet graphic.  
        /// Reverts back to an "active" or "inactive" graphic when the mouse moves out of bounds of the widjet.
        /// </summary>
        public event EventHandler OnHoverEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialises the widjet with basic properties required by all widgets, and initialises events for the widjet
        /// </summary>
        /// <param name="px">X position of the widjet relative to the GuiManagers surface</param>
        /// <param name="py">Y position of the widjet relative to the GuiManagers surface</param>
        /// <param name="w">Width of the widjet</param>
        /// <param name="h">Height of the widjet</param>
        public Widjet(int px, int py, int w, int h)
        {
            PosX = px;
            PosY = py;
            Width = w;
            Height = h;

            _shortcut = Key.Unknown;

            OnClickEvent += new EventHandler(OnClick);
            OnActiveEvent += new EventHandler(OnActive);
            OnInactiveEvent += new EventHandler(OnInactive);
            OnHoverEvent += new EventHandler(OnHover);
            
        }

        #endregion

        #region Component Overrides

        public override void Update()
        {
            base.Update();
            if (!isDisabled)
            {
                CheckBounds();
                CheckKey();
            }
                ClickTimer();
        }

        public override void Render()
        {
            base.Render();

            if (_label != null)
                Draw.Graphic(_label);
        }

        public override void Removed()
        {
            base.Removed();

            ResetEventFlags();
            isActive = false;
            hasClicked = false;
        }
        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Checks the click timer and sets hasClicked as false if the timer has finished
        /// </summary>
        protected virtual void ClickTimer()
        {
            if (hasClicked && clickCounter == clickTimer)
            {
                hasClicked = false;
            }
            else
            {
                clickCounter++;
            }
        }

        #endregion

        #region Default Event Methods

        /// <summary>
        /// The default method added to OnActiveEvent.  Is triggered by all widjets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnActive(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnActiveFired = true;
        }

        /// <summary>
        /// The default method added to OnInactiveEvent.  Is triggered by all widjets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnInactive(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnInactiveFired = true;
        }

        /// <summary>
        /// The default method added to OnClickEvent.  Is triggered by all widjets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnClick(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnClickFired = true;
            if (ClickSound != null)
                ClickSound.Play();
        }

        /// <summary>
        /// The default method added to OnHoverEvent.  Is triggered by all widjets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnHover(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnHoverFired = true;
            if (HoverSound != null)
                HoverSound.Play();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the game instance for the widjet to use.
        /// Dont call this directly.
        /// </summary>
        /// <param name="g">The current game instance</param>
        public void SetGame(Game g)
        {
            game = g;
        }

        /// <summary>
        /// Set the shortcut key you want to trigger the widjet.  Using the shortcut key
        /// invokes an OnClickEvent.
        /// Dont use shortcuts if your gui contains a GuiTextBox
        /// </summary>
        /// <param name="k">The Key you want to reserve for the widjet</param>
        public void SetShortcut(Key k)
        {
            _shortcut = k;
            isShortcutSet = true;
        }

        public void SetLabel(int xOffset, int yOffset, string text, int size = 12, string font = "")
        {
            _label = new Text(text, font, size);
            _label.SetPosition(PosX + xOffset, PosY + yOffset);
        }

        public void SetLabel(Text textObject)
        {
            _label = textObject;
        }

        public void SetLabel(int xOffset, int yOffset, Text textObject)
        {
            _label = textObject;
            _label.SetPosition(PosX + xOffset, PosY + yOffset);
        }

        /// <summary>
        /// Call this to manually simulate a click on a widjet from within your code.
        /// </summary>
        public void Click()
        {
            hasClicked = true;
            clickCounter = 0;
            isActive = true;
            OnClickEvent.Invoke(this, null);
        }

        /// <summary>
        /// Resets flags each time an event is fired, to allow other events to fire.
        /// The flags help control when events are fired and how frequently.
        /// </summary>
        public void ResetEventFlags()
        {
            hasOnClickFired = false;
            hasOnActiveFired = false;
            hasOnInactiveFired = false;
            hasOnHoverFired = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is in charge of checking collisions, your mouse position in the gui, and fires events based
        /// on user actions.
        /// </summary>
        private void CheckBounds()
        {
            bool inBounds = Util.InRect(Scene.MouseX, Scene.MouseY, Scene.CameraX + PosX, Scene.CameraY + PosY, Width, Height);

            if (inBounds && CheckMouseClick())
            {
                Click();
            }
            else if (!inBounds && Input.Instance.MouseButtonPressed(MouseButton.Left))
            {
                if (!hasOnInactiveFired) OnInactiveEvent.Invoke(this, null);
                isActive = false;
            }
            else if (inBounds && !hasClicked)
            {
                if (!hasOnHoverFired) OnHoverEvent.Invoke(this, null);
            }
            else if (!inBounds && !hasClicked)
            {
                if (isActive)
                {
                    if (!hasOnActiveFired) OnActiveEvent.Invoke(this, null);
                }
                else
                {
                    if (!hasOnInactiveFired) OnInactiveEvent.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// If you set a shortcut key for the widjet, this method checks to see
        /// when the user presses the key, and responds to it with a Click()
        /// </summary>
        private void CheckKey()
        {
            if (Input.Instance.KeyPressed(_shortcut) && isShortcutSet)
            {
                Click();
            }
        }

        /// <summary>
        /// Checks if the mouse has been "Pressed" or is "down" based on the widjet type.
        /// </summary>
        /// <returns>Returns true if the mouse is clicked in the bounds of a widjet</returns>
        private bool CheckMouseClick()
        {
            if (isDownable)
            {
                return Input.Instance.MouseButtonDown(MouseButton.Left);
            }
            else
            {
                return Input.Instance.MouseButtonPressed(MouseButton.Left);
            }
        }
        #endregion
    }
}
