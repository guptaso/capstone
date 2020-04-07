using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyStrokes
{
    public partial class AddApplication : Form
    {
        private readonly GamingUseCase GamingWindow;
        private Screen currentScreen;


        public AddApplication(GamingUseCase game)
        {

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();

            // These simply set the order of Tabs and allow these controls to be changed via Tab
            textBox1.TabStop = true;
            textBox1.TabIndex = 0;
            textBox2.TabStop = true;
            textBox2.TabIndex = 1;
            textBox3.TabStop = true;
            textBox3.TabIndex = 2;
            button1.TabStop = true;
            button1.TabIndex = 4;
            button2.TabStop = true;
            button2.TabIndex = 3;

            // Add keydown events on the constructor here
            // Gets overwritten when this is put in the designer.cs adn the form changes
            textBox1.KeyDown += textBox1_KeyDown;
            textBox2.KeyDown += textBox2_KeyDown;
            textBox3.KeyDown += textBox3_KeyDown;
            button1.KeyDown += button1_KeyDown;
            button2.KeyDown += button2_KeyDown;

            GamingWindow = game;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //If at least one of the fields are empty
            if (textBox1.Text == ""/* || textBox2.Text == "" */|| textBox3.Text == "")
            {
                MessageBox.Show("At least one of the required entries are empty, please try again", "Empty Inputs");

                // Whichever text item was empty, make sure to give it focus
                if (textBox1.Text == "")
                    textBox1.Select();
                else
                    textBox3.Select();
            }

            //Textbox 3 input should only be 1 character
            else if (textBox3.Text.Length > 1)
            {
                MessageBox.Show("The assigned hotkey should only be a single character", "Invalid Hotkey");
                textBox3.Select();      // reassign the hotkey so give it control
            }

            //Textbox 3, for now, should ONLY have alphanumeric characters
            else if (!Char.IsLetterOrDigit(textBox3.Text[0]))
            {
                MessageBox.Show("The assigned hotkey should only be alphanumeric", "Invalid Hotkey");
                textBox3.Select();      // reassign the hotkey so give it control
            }

            // DISCORD SPECIAL CONDITION: if any part of the automatically filled out fields were altered, then do not accept it
            else if (((textBox1.Text == "https://discordapp.com" || textBox1.Text == "https://discord.gg") && !textBox2.Text.Contains(@"KeyStrokes\Images\discord.png"))
                        || ((textBox1.Text != "https://discordapp.com" && textBox1.Text != "https://discord.gg") && textBox2.Text.Contains(@"KeyStrokes\Images\discord.png"))) 
            {
                MessageBox.Show("I see you want to load Discord.  However, please DO NOT change the automatically loaded fields...", "Discord Forms Altered");

                // Reload the fields
                textBox1.Text = "https://discordapp.com";   // or https://discord.gg
                textBox2.Text = @"C:\Users\Intel-VCSE-7\Desktop\gaming-improvements\Software Innovation for Dual Screen Notebook\KeyStrokes\Images\discord.png";
                textBox3.Select();

            }

            //Otherwise, we chilling
            else
            {
                //Send the form information to the MainWindow
                //Now checks if the hotkey used was unique.
                //If the hotkey was not unique, restore focus to that textbox
                if (GamingWindow.processFormInputs(textBox1.Text, textBox2.Text, textBox3.Text))
                {
                    GamingUseCase.finished = false;
                    GamingWindow.Activate();        // for some reason, the gaming use case window goes out of focus when app is closed.  this line fixes that issue
                    this.Close();
                }
                else
                    textBox3.Select();
            }
        }

        //Switch to textbox 2 if tab is pressed
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                textBox2.Select();
        }

        //Switch to textbox 3 if tab is pressed
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                textBox3.Select();
        }

        //Switch to cancel button if tab is pressed
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                button2.Select();
        }

        //Simply close the form
        private void button2_Click(object sender, EventArgs e)
        {
            GamingUseCase.finished = false;
            this.Close();
        }

        //Switch to submit button if tab is pressed
        //Or cancel the form if the enter key was pressed
        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                button1.Select();
            else if (e.KeyData == Keys.Enter)
            {
                GamingUseCase.finished = false;
                this.Close();
            }
        }

        //Switch to textbox 1 if tab is pressed
        //Also submit the form if the ENTER key was pressed
        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                textBox1.Select();
            else if (e.KeyData == Keys.Enter)
                button1_Click(sender, e);
        }

        //Open the file dialog upon clicking "Load Application"
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Filter = "All files (*.*)|*.*";
            openFile.DefaultExt = "(*.*)";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFile.FileName;

                // only discord has problems with Process.Start(), so with ONLY discord, we automatically fill in 2 textbox fields
                if (textBox1.Text.Contains(@"Discord\Update.exe"))
                {
                    MessageBox.Show("Discord must be loaded onto a website, so the first two fields will automatically be provided", "Discord Warning");
                    textBox1.Text = "https://discordapp.com";               // or https://discord.gg
                    textBox2.Text = @"C:\Users\Intel-VCSE-7\Desktop\gaming-improvements\Software Innovation for Dual Screen Notebook\KeyStrokes\Images\discord.png";
                    textBox3.Select();
                }
                else
                    textBox2.Select();
            }
        }

        //Open the file dialog upon clicking "Load Image"
        private void button4_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();

            openFile.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG";
            openFile.DefaultExt = "(*.*)";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFile.ShowDialog() == true)
            {
                textBox2.Text = openFile.FileName;
                textBox3.Select();
            }
        }

        // When the form is closed using the x button on the top right
        private void Application_Closing(object sender, CancelEventArgs e)
        {
            GamingUseCase.finished = false;
            // this.Close();                    // because this event closes the app already, it would actually cause issues if this.Close() was called
        }

        // When the form is opened, determine if there's a second screen
        private void Application_Opening(object sender, EventArgs e)
        {
            // If there is only one screen, place it on the main screen
            // Otherwise, load it on the companion screen
            if (Screen.AllScreens.Length == 1)
                currentScreen = Screen.AllScreens[0];
            else
            {
                currentScreen = Screen.AllScreens[1];
                if (currentScreen != null)
                {

                    // Winforms and wpf interpret dimensions differently.  
                    // In the case of putting the add form on the second screen, we have to multiply the working area's height by 2
                    this.Top = currentScreen.WorkingArea.Height *2;
                    this.Left = currentScreen.WorkingArea.Width - this.Width;    //basically, offset to the left by the width of the form

                }
            }
        }
    }
}
