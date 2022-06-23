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
    public partial class timingintervil : Form
    {
        public timingintervil()
        {
            InitializeComponent();
        }

        private void timingintervil_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {


        }
        public int Get_timer()
        {
            return (int)numericUpDown1.Value;
        }

        public void Set_timer(int timer)
        {
            numericUpDown1.Value = timer;
        }
    }
}
