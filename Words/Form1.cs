using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words
{
    public partial class Form1 : Form
    {
        string[] words;
        string displayedWord = "";
        bool isWordDisplayed = false;

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                using (StreamReader reader = new StreamReader(selectedFilePath))
                {
                    string fileContent = reader.ReadToEnd();

                    textBox1.Text = fileContent;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            words = textBox1.Text.Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HandleSelectedText(textBox1.SelectedText, textBox1.SelectionStart);
        }

        private void HandleSelectedText(string selectedText, int selectionStart)
        {
            string pattern = "^[а-щА-ЩЬьЮюЯяЇїІіЄєҐґ’-]+$";
            bool containsNonUkrainianLetters = !Regex.IsMatch(selectedText, pattern, RegexOptions.IgnoreCase);
            if (containsNonUkrainianLetters || !words.Contains(selectedText))
            {
                ClearLetterImages();
                return;
            }
            char[] separators = new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?' };
            try
            {
                if (separators.Contains(textBox1.Text[selectionStart - 1]) && separators.Contains(textBox1.Text[selectionStart + selectedText.Length]))
                    DisplayLetterImages(selectedText, 10, ClientSize.Width - 10, ClientSize.Height - 150);
            }
            catch
            {
                if (selectionStart - 1 < 0 && separators.Contains(textBox1.Text[selectionStart + selectedText.Length]))
                    DisplayLetterImages(selectedText, 10, ClientSize.Width - 10, ClientSize.Height - 150);
                else if (separators.Contains(textBox1.Text[selectionStart - 1]) && selectionStart + selectedText.Length >= textBox1.Text.Length)
                    DisplayLetterImages(selectedText, 10, ClientSize.Width - 10, ClientSize.Height - 150);
            }
        }

        private void DisplayLetterImages(string word, int x1, int x2, int y)
        {
            word = word.ToUpper();
            if (displayedWord == word && isWordDisplayed)
                return;
            else
                ClearLetterImages();
            displayedWord = word;
            int areaWidth = x2 - x1;

            int totalImagesWidth = 0;
            foreach (char letter in word)
            {
                Image image = Image.FromFile(@"..\\..\\Resources\\Letters\\" + letter + ".png");
                totalImagesWidth += image.Width;
            }

            if (totalImagesWidth > this.ClientSize.Width)
                totalImagesWidth = this.ClientSize.Width - 10;

            int startX = x1 + (areaWidth - totalImagesWidth) / 2;

            foreach (char letter in word)
            {
                Image image = Image.FromFile(@"..\\..\\Resources\\Letters\\" + letter + ".png");
                PictureBox pb = new PictureBox();
                int size = totalImagesWidth / word.Length;
                pb.Size = new Size(size, size);
                pb.Location = new Point(startX, y);
                pb.BackgroundImage = image;
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                Controls.Add(pb);
                startX += size;
            }
            isWordDisplayed = true;
        }

        private void ClearLetterImages()
        {
            if (isWordDisplayed == false)
                return;
            while (this.Controls.OfType<PictureBox>().Any())
            {
                PictureBox pictureBox = this.Controls.OfType<PictureBox>().First();
                this.Controls.Remove(pictureBox);
                pictureBox.Dispose();
            }
            isWordDisplayed = false;
        }
    }
}
