using System.Windows.Forms;
using DirectShowLib;

namespace webcamControl
{
    public partial class FormCameras : Form
    {
        public FormCameras()
        {
            InitializeComponent();

            var capDev = DsDevice.GetDevicesOfCat( FilterCategory.VideoInputDevice );

            foreach ( var device in capDev )
            {
                var tabPage = new TabPage { Text = device.Name };

                tabPage.Controls.Add( new CameraProperties( device ) { Dock = DockStyle.Fill } );

                tabControl.Controls.Add( tabPage );
            }
        }
    }
}
