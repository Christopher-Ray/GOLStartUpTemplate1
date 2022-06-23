using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOLStartUpTemplate1
{
    public partial class Form1 : Form
    {
        static int height = 20;
        static int width = 20;
        bool gridswitcher = true;
        bool switchcount = true;
        bool hudswitcher = true;
        bool swapper = true;
        // The universe array
        bool[,] universe = new bool[width, height];
        bool[,] scratchpad = new bool[width, height];
        // Drawing colors
        Color gridColor = Properties.Settings.Default.Gridcolor;
        Color cellColor = Properties.Settings.Default.cellcolor;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int seed = 0;
        public Form1()
        {
            InitializeComponent();
            graphicsPanel1.BackColor = Properties.Settings.Default.Background;
            // Setup the timer
            timer.Interval = Properties.Settings.Default.timerintervil; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xLen = -1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yLen = -1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int alive = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    if (universe[x,y] == true)
                    {
                        alive++;
                    }
                    SwapBoundry(x, y, out int count);
                    if (universe[x, y] == true)
                    {
                        if (count > 3)
                        {
                            scratchpad[x, y] = false;
                        }
                        else if (count < 2 )
                        {
                            scratchpad[x, y] = false;
                        }
                        else if (count == 2 || count == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                    }
                    else if (count == 3)
                    {
                        scratchpad[x, y] = true;
                    }
                }
            }


            // Increment generation count
            generations++;
            bool[,] swop = universe;
            universe = scratchpad;
            bool[,] empty = new bool[20, 20];
            scratchpad = empty;
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Cells Alive = " + alive.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Font hudwriter = new Font("Arial", 10);
            StringFormat fontform = new StringFormat();
            fontform.Alignment = StringAlignment.Center;
            fontform.LineAlignment = StringAlignment.Center;
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    if (gridswitcher == true)
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    if (switchcount == true)
                    {
                        SwapBoundry(x, y, out int countneighbor);
                    if (countneighbor != 0)
                    {
                        e.Graphics.DrawString(countneighbor.ToString(), hudwriter, Brushes.Gray, cellRect, fontform);
                    }
                    }
                }
            }
            int alive = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    if (universe[x, y] == true)
                    {
                        alive++;
                    }
                }
            }
           
            if (hudswitcher == true)
            {
                RectangleF headsup = RectangleF.Empty;
                string hudtext = "Generation = " + generations + " Living Cells = " + alive + " Universe Size = " + Width + ", " + Height;
                headsup.Width = graphicsPanel1.Width;
                headsup.Height = graphicsPanel1.Height;
                headsup.X = graphicsPanel1.Width / 4;
                headsup.Y = graphicsPanel1.Height - 50;

                e.Graphics.DrawString(hudtext.ToString(), hudwriter, Brushes.Black, headsup);
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            int alive = 0;
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                toolStripStatusLabel1.Text = "Cells Alive = " + alive.ToString();
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = false;
            int alive = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    universe[x, y] = false;
                }

            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Cells Alive = " + alive.ToString();
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {

                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x,y] == true)
                        {
                            currentRow += "O";
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x,y] == false)
                        {
                            currentRow += ".";
                        }
                    }
                    writer.WriteLine(currentRow);

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    if (row.Contains("O")|| row.Contains("."))
                    {
                        maxHeight++;
                        
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if (row.Length > maxWidth)
                    {
                        maxWidth = row.Length;
                    }
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchpad = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                int y = 0;
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.

                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O' )
                        {
                            universe[xPos, y] = true;
                        }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                        if (row[xPos] == '.')
                        {
                            universe[xPos, y] = false;
                        }
                    }
                    y += 1;
                }

                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void theSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    seed = random.Next(0, 2);
                    if (seed == 0)
                    {
                        universe[x, y] = true;
                    }
                   
                }

            }
            graphicsPanel1.Invalidate();
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fromseed = new Form2();
            fromseed.Set_Seed(seed);
            if (DialogResult.OK == fromseed.ShowDialog())
            {
                int seed;
                Random randomrandom = new Random();
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        seed = randomrandom.Next(0, 2);
                        if (seed == 0)
                        {
                            universe[x, y] = true;
                        }
                        else
                        {
                            universe[x, y] = false;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            int alive = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    universe[x, y] = false;
                }

            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Cells Alive = " + alive.ToString();
            graphicsPanel1.Invalidate();
        }

        private void millsecondsTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timingintervil time = new timingintervil();
            time.Set_timer(timer.Interval);
            if (DialogResult.OK == time.ShowDialog())
            {
                timer.Interval = time.Get_timer();
            }
            graphicsPanel1.Invalidate();
        }

        private void resizingGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resizing size = new resizing();
            int width = 0;
            int height = 0;
            size.Set_width(width);
            size.Set_height(height);
            if (DialogResult.OK == size.ShowDialog())
            {
                width = size.Get_width();
                height = size.Get_height();
                universe = new bool[width, height];
                scratchpad = new bool[width, height];
            }
            graphicsPanel1.Invalidate();

        }
        private void SwapBoundry(int x , int y , out int neighbor)
        {
            if (swapper == true)
            {
                neighbor = CountNeighborsFinite(x, y);
            }
            else 
            {
                neighbor = CountNeighborsToroidal(x, y);
            }
            
        }

        private void countOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switchcount = true;
            graphicsPanel1.Invalidate();
        }

        private void countOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switchcount = false;
            graphicsPanel1.Invalidate();
        }

        private void hudOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudswitcher = true;
            graphicsPanel1.Invalidate();
        }

        private void hudOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hudswitcher = false;
            graphicsPanel1.Invalidate();
        }

        private void gridOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridswitcher = true;
            graphicsPanel1.Invalidate();
        }

        private void gridOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridswitcher = false;
            graphicsPanel1.Invalidate();
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog grids = new ColorDialog();
            grids.Color = gridColor;
            if (DialogResult.OK == grids.ShowDialog())
            {
                gridColor = grids.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cells = new ColorDialog();
            cells.Color = cellColor;
            if (DialogResult.OK == cells.ShowDialog())
            {
                cellColor = cells.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog backgrounds = new ColorDialog();
            backgrounds.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == backgrounds.ShowDialog())
            {
                graphicsPanel1.BackColor = backgrounds.Color;
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.Background;
            cellColor = Properties.Settings.Default.cellcolor;
            gridColor = Properties.Settings.Default.Gridcolor;
            width = Properties.Settings.Default.universewidth;
            height = Properties.Settings.Default.universeheight;
            timer.Interval = Properties.Settings.Default.timerintervil;
            universe = new bool[width, height];
            scratchpad = new bool[width, height];
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            graphicsPanel1.BackColor = Properties.Settings.Default.Background;
            cellColor = Properties.Settings.Default.cellcolor;
            gridColor = Properties.Settings.Default.Gridcolor;
            width = Properties.Settings.Default.universewidth;
            height = Properties.Settings.Default.universeheight;
            timer.Interval = Properties.Settings.Default.timerintervil;
            universe = new bool[width, height];
            scratchpad = new bool[width, height];
            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Background = graphicsPanel1.BackColor;
            Properties.Settings.Default.cellcolor = cellColor;
            Properties.Settings.Default.Gridcolor = gridColor;
            Properties.Settings.Default.universewidth = width;
            Properties.Settings.Default.universeheight = height;
            Properties.Settings.Default.timerintervil = timer.Interval;
            Properties.Settings.Default.Save();
        }
    }
}
