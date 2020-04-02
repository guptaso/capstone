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

        private GamingUseCase GamingWindow;

        public AddApplication(GamingUseCase game)
        {
            InitializeComponent();

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
                MessageBox.Show("At least one of the required entries are empty, please try again", "Empty Inputs");

            //Textbox 3 input should only be 1 character
            else if (textBox3.Text.Length > 1)
                MessageBox.Show("The assigned hotkey should only be a single character", "Invalid Hotkey");

            //Textbox 3, for now, should ONLY have alphanumeric characters
            else if(!Char.IsLetterOrDigit(textBox3.Text[0]))
            {
                MessageBox.Show("The assigned hotkey should only be alphanumeric", "Invalid Hotkey");
            }

            //Otherwise, we chilling
            else
            {
                //Send the form information to the MainWindow.
                //To avoid making a new MainWindow class, make the method static

                //Now checks if the hotkey used was unique.
                if (GamingWindow.processFormInputs(textBox1.Text, textBox2.Text, textBox3.Text))
                {
                    GamingUseCase.finished = false;
                    this.Close();
                }
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
            this.Close();
        }

        //Switch to submit button if tab is pressed
        //Or cancel the form if the enter key was pressed
        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                button1.Select();
            else if (e.KeyData == Keys.Enter)
                this.Close();
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
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();

            openFile.Filter = "All files (*.*)|*.*";
            openFile.DefaultExt = "(*.*)";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFile.ShowDialog() == true)
            {
                textBox1.Text = openFile.FileName;
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
    }
}
