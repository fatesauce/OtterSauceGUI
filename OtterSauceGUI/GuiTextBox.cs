﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;
using System.Threading;
using System.Windows;

namespace OtterSauceGui
{
    #region Enums

    /// <summary>
    /// Used to define what characters you wish to allow the user to input
    /// </summary>
    public enum TextBoxType
    {
        /// <summary>
        /// The default type for text boxes is NORMAL.  Will allow all alphanumerical and special characters.
        /// </summary>
        NORMAL,

        /// <summary>
        /// Will accept the same characters as NORMAL, but will not display them. (replaces characters with "*").  
        /// Call GetPassword() to retrieve the raw password from the  text box for processing
        /// </summary>
        PASSWORD,

        /// <summary>
        /// Allows alpha characters, ".", "-", "_", and "@" characters
        /// </summary>
        EMAIL,

        /// <summary>
        /// Allows numbers to be entered only
        /// </summary>
        NUMERICAL,

        /// <summary>
        /// Allows numbers and the "." character
        /// </summary>
        NUMERICALDECI
    }

    #endregion

    /// <summary>
    /// This class allows you to create text boxes to add to your GuiManager.  Text boxes are designed to accept a single line of input.
    /// </summary>
    public class GuiTextBox : Widget
    {

        #region Private Fields

        private Text TextDisplay;
        private Image TextBox;

        private Image TextBoxBlinker;
        private bool showBlinker;
        private int BlinkerAdjustment;

        /*
        private int CursorTextIndex;
        private int CursorX;
        private int CursorY;
         * */

        private TextBoxType textBoxType;
        private string CurrentTextInput = "";
        private string OldTextInput = "";
        private string PasswordString = "";
        private int CurrentTextIndex = 0;
        private int TypeCounter = 0;

        #endregion

        #region Public Fields

        /// <summary>
        /// The maximum amount of characters the user can input and display at once
        /// </summary>
        public int MaxCharacters = 32;

        #endregion

        #region Events

        /// <summary>
        /// This event is fired everytime the text is changed by the user.  ie: Each time the user inputs
        /// a character, or deletes a character
        /// </summary>
        public event EventHandler OnTextChangeEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initialises your textbox widget.  After you create it and adjust its properties,
        /// add it to the GuiManager with AddWidjet
        /// </summary>
        /// <param name="px">X position of the widjet relative to the GuiManagers Surface</param>
        /// <param name="py">Y position of the widjet relative to the GuiManagers Surface</param>
        /// <param name="w">Width of widjet</param>
        /// <param name="h">Height of widjet</param>
        /// <param name="size">Font size of input</param>
        /// <param name="type">Type of input</param>
        public GuiTextBox(int px, int py, int w, int h, int size = 12, TextBoxType type = TextBoxType.NORMAL)
            : base(px, py, w, h)
        {
            PosX = px;
            PosY = py;
            Width = w;
            Height = h;
            textBoxType = type;

            TextDisplay = new Text();
            TextDisplay.Color = Color.Black;
            TextDisplay.FontSize = size;

            TextBox = Image.CreateRectangle(Width, Height);
            TextBox.Color = Color.White;

            TextBoxBlinker = Image.CreateRectangle(1, size, Color.Black);
            BlinkerAdjustment = (int) (size * 0.1f);

            clickTimer = 0;
            RenderAfterEntity = true;

            OnTextChangeEvent += new EventHandler(OnTextChange);
        }

        #endregion

        #region Component Overrides

        /// <summary>
        /// Controls the behavior of the textbox relative to users actions and input
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (isActive && !isDisabled)
            {

                OldTextInput = CurrentTextInput;

                if ((Input.Instance.KeyDown(Key.LControl) || Input.Instance.KeyDown(Key.RControl)) && Input.Instance.KeyPressed(Key.V))
                {
                    Input.Instance.KeyString = CurrentTextInput + PasteText();
                    TextDisplay.String = GetSubstring();
                }

                switch (textBoxType)
                {
                    case TextBoxType.NORMAL:
                        CurrentTextInput = GetNormalString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.NUMERICAL:
                        CurrentTextInput = GetNumericalString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.PASSWORD:
                        CurrentTextInput = GetPasswordString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.EMAIL:
                        CurrentTextInput = GetEmailString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.NUMERICALDECI:
                        CurrentTextInput = GetNumericalDeciString(Input.Instance.KeyString);
                        break;
                }

                SetSubstringIndex();

                if (CurrentTextInput.Length > MaxCharacters)
                {
                    CurrentTextInput = CurrentTextInput.Substring(0, MaxCharacters);
                    Input.Instance.KeyString = CurrentTextInput;
                }

                if (Timer % 30 >= 15)
                {
                    TextDisplay.String = GetSubstring();
                    showBlinker = true;
                }
                else
                {
                    TextDisplay.String = GetSubstring();
                    showBlinker = false;
                }
            }
            else
            {
                Input.Instance.KeyString = CurrentTextInput;
            }

            if (OldTextInput != CurrentTextInput)
            {
                OnTextChangeEvent.Invoke(this, null);
            }
        }

        /// <summary>
        /// Components have no native Graphic fields, so graphics for the textbox component have to be Drawn 
        /// in the Render loop
        /// </summary>
        public override void Render()
        {
            base.Render();
            Draw.Graphic(TextBox, PosX, PosY);
            Draw.Graphic(TextDisplay, PosX, PosY);
            if (isActive && showBlinker)
                Draw.Graphic(TextBoxBlinker, PosX + TextDisplay.Width + 1 , PosY + BlinkerAdjustment);
        }

        #endregion

        #region Widjet Overrides

        /// <summary>
        /// Notifies the parent GuiManager that the user is inputing text, so will disable the use of shortcut keys.
        /// Its recommended to disable player controls while typing.  Add a condition in your control logic to check the
        /// GuiManagers "isTyping" flag.
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public override void OnClick(object sender, EventArgs e)
        {
            base.OnClick(sender, e);
            if (Entity.GetType() == typeof(GuiManager) || Entity.GetType().IsSubclassOf(typeof(GuiManager)))
            {
                GuiManager gui = (GuiManager)Entity;
                gui.isTyping = true;
            }
            Input.Instance.KeyString = CurrentTextInput;
        }

        /// <summary>
        /// Changes the graphical appearance and sets the games Input.KeyString when the text box becomes active
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public override void OnActive(object sender, EventArgs e)
        {
            base.OnActive(sender, e);
            
            TextBox.Color = Color.Grey;
        }

        /// <summary>
        /// Changes the graphical appearance when the text box becomes inactive
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public override void OnInactive(object sender, EventArgs e)
        {
            base.OnInactive(sender, e);
            TextBox.Color = Color.White;

            if (Entity.GetType() == typeof(GuiManager))
            {
                GuiManager gui = (GuiManager)Entity;
                gui.isTyping = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The default method for the OnTextChangeEvent
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public void OnTextChange(object sender, EventArgs e)
        {
            TypeCounter++;
            Console.WriteLine("Text changed! " + TypeCounter);
            Console.WriteLine("TextWidth: {0}", TextDisplay.Width);
            Console.WriteLine("Substring Index: {0}",CurrentTextIndex);
        }

        /// <summary>
        /// Set the text to be displayed in the textbox.  Can be deleted by the user
        /// </summary>
        /// <param name="t">The text to display</param>
        public void SetText(string t)
        {
            Input.Instance.KeyString = t;
            OldTextInput = t;
            CurrentTextInput = t;
            TextDisplay.String = t;
        }

        /// <summary>
        /// Clears the text displayed in the text box and resets the Input.KeyString
        /// </summary>
        public void ClearText()
        {
            OldTextInput = "";
            CurrentTextInput = "";
            TextDisplay.String = "";
            Input.Instance.ClearKeystring();
        }

        /// <summary>
        /// Returns the text displayed in the text box
        /// </summary>
        /// <returns>The text in the text box</returns>
        public string GetText()
        {
            return CurrentTextInput;
        }

        /// <summary>
        /// Returns the raw, uncensored password, which was entered in the textbox
        /// </summary>
        /// <returns>The actual string entered in the text box, not the stars</returns>
        public string GetPassword()
        {
            if (textBoxType != TextBoxType.PASSWORD)
            {
                return null;
            }
            else
            {
                return PasswordString;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method for determining which part of the string should be displayed when the length of the string exceeds the size of the textbox
        /// </summary>
        private void SetSubstringIndex()
        {
            if (Input.Instance.KeyPressed(Key.Back) || Input.Instance.KeyDown(Key.Back))
            {
                if (CurrentTextIndex != 0)
                    CurrentTextIndex--;
            }
            if (TextDisplay.Width >= TextBox.Width)
            {
                CurrentTextIndex++;
            }
        }

        /// <summary>
        /// The actual text that will be rendered to the string when the text width exceeds the textbox width
        /// </summary>
        /// <returns>The text that will be rendered</returns>
        private string GetSubstring()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            foreach (Char c in CurrentTextInput)
            {
                sb.Append(c);
                TextDisplay.String = sb.ToString();
                if (TextDisplay.Width > Width)
                    count++;
            }

            CurrentTextIndex = count;

            return CurrentTextInput.Substring(CurrentTextIndex);
        }

        /// <summary>
        /// Checks the keys entered into NORMAL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNormalString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c != '\n')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into NUMERICAL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNumericalString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c != '\n')
                {
                    if(c >= '0' && c <= '9')
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into NUMERICALDECI text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNumericalDeciString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c != '\n')
                {
                    if ((c >= '0' && c <= '9') || c == '.')
                        sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into PASSWORD text box.  Stores the input string in another variable for retrieval using GetPassword().
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string of "*" based on the amount of characters entered</returns>
        private string GetPasswordString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c != '\n')
                {
                    sb.Append(c);
                }
            }
            PasswordString = sb.ToString();

            StringBuilder hs = new StringBuilder();
            foreach (char c in PasswordString)
            {
                hs.Append('*');
            }
            return hs.ToString();
        }

        /// <summary>
        /// Checks the keys entered into EMAIL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetEmailString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c != '\n')
                {
                    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||c == '@' || c == '.' || c == '-' || c == '_')
                        sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Used for pasting text to the textbox from windows clipboard
        /// </summary>
        /// <returns>Sting from clipboard</returns>
        private string PasteText()
        {
            string pasteText = null;
            Exception threadEx = null;
            Thread staThread = new Thread(
                delegate()
                {
                    try
                    {
                        pasteText = Clipboard.GetText();
                    }

                    catch (Exception ex)
                    {
                        threadEx = ex;
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            return pasteText;
        }

        #endregion
    }
}
