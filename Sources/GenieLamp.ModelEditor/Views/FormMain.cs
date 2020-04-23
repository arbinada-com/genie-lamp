using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GenieLamp.ModelEditor.Services;

namespace GenieLamp.ModelEditor.Views
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            GNU.Gettext.WinForms.Localizer.Localize(this, L.Catalog);
        }

        private void actionFileExit_Execute(object sender, EventArgs e)
        {
            this.Close();
        }

        private void actionFileOpen_Execute(object sender, EventArgs e)
        {
            if (dlgOpenProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(dlgOpenProject.FileName);
            }
        }

    }
}
