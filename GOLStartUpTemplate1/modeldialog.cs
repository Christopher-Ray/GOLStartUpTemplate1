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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public int Git_Seed()
        {
            return (int)numericUpDown1.Value;
        }

        public void Set_Seed(int seed)
        {
            numericUpDown1.Value = seed;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int seed = random.Next(int.MinValue, int.MaxValue);
            Set_Seed(seed);
        }
    }
}
