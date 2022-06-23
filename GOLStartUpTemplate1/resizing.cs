using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate1
{
    public partial class resizing : Form
    {
        public resizing()
        {
            InitializeComponent();
        }
        public int Get_height()
        {
            return (int)numericUpDown1.Value;
        }

        public void Set_height(int height)
        {
            numericUpDown1.Value = height;
        }
        public int Get_width()
        {
            return (int)numericUpDown2.Value;
        }

        public void Set_width(int width)
        {
            numericUpDown2.Value = width;
        }
    }
}
