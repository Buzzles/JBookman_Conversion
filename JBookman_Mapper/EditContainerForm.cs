using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using JBookman_Conversion;

namespace JBookman_Mapper
{

    class EditContainerForm : Form
    {
        //Form vars
        //List<CContainer> containerList;

        //Form Components
        Label lblContainerCount;
        CMap map;

        Button okButton;
        Button cancelButton;

        public EditContainerForm(CMap currentmapIn)
        {
            //default constructor

            map = currentmapIn;
            
            //MessageBox.Show("Dialog loaded");
            this.Size = new Size(200, 250);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Text = "Container Editor";
            this.Location = new Point(540, 312);

            //components:
            okButton = new Button();
            cancelButton = new Button();
            lblContainerCount = new Label();


            okButton.Size = new Size(50, 25);
            //okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Text = "OK";
            okButton.Location = new Point(ClientRectangle.Left + 25, 175);
            okButton.MouseClick += new MouseEventHandler(this.okClickEvent);

            cancelButton.Size = new Size(50, 25);
            cancelButton.Location = new Point(((ClientRectangle.Right - cancelButton.Width) - 25), 175);
            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            lblContainerCount.Text = "Container Count:" + map.m_ContainersInMap.GetContainerCount();
            lblContainerCount.Location = new Point(5, 5);
            lblContainerCount.Size = new Size(90, 30);


            this.Controls.AddRange(new Control[] { okButton, cancelButton, lblContainerCount });
        }

     /*   protected override void OnLoad(EventArgs e)
        {
            


        }*/

        private void okClickEvent(object sender, EventArgs e)
        {
            this.Close();
        }



//endofclass
    }
}