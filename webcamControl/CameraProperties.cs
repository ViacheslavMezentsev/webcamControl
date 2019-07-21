using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Timer = System.Threading.Timer;

using DirectShowLib;

namespace webcamControl
{
    public partial class CameraProperties : UserControl
    {
        private const int EVALUATION_INTERVAL_MSEC = 1000;
        private const int EVALUATION_START_DELAY_MSEC = 1000;

        private Timer _updateAuto;

        private readonly List<PropertyData> _properties = new List<PropertyData>
        {
            new PropertyData { CamProperty = CameraControlProperty.Focus, Text = "Focus" },
            new PropertyData { CamProperty = CameraControlProperty.Exposure, Text = "Exposure" },
            new PropertyData { CamProperty = CameraControlProperty.Iris, Text = "Iris" },
            new PropertyData { CamProperty = CameraControlProperty.Zoom, Text = "Zoom" },
            new PropertyData { CamProperty = CameraControlProperty.Pan, Text = "Pan" },
            new PropertyData { CamProperty = CameraControlProperty.Tilt, Text = "Tilt" },
            new PropertyData { CamProperty = CameraControlProperty.Roll, Text = "Roll" },

            new PropertyData { AmpProperty = VideoProcAmpProperty.Brightness, Text = "Brightness" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Contrast, Text = "Contrast" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Hue, Text = "Hue" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Saturation, Text = "Saturation" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Sharpness, Text = "Sharpness" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Gamma, Text = "Gamma" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.ColorEnable, Text = "Color Enable" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.WhiteBalance, Text = "White Balance" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.BacklightCompensation, Text = "Backlight Compensation" },
            new PropertyData { AmpProperty = VideoProcAmpProperty.Gain, Text = "Gain" },
        };

        public CameraProperties( DsDevice device )
        {
            InitializeComponent();

            var iid = typeof( IBaseFilter ).GUID;

            device.Mon.BindToObject( null, null, ref iid, out var camDevice );

            var camFilter = camDevice as IBaseFilter;

            var cameraControl = camFilter as IAMCameraControl;
            var videoProcAmp = camFilter as IAMVideoProcAmp;

            Debug.WriteLine( device.Name );

            foreach ( var property in _properties )
            {
                property.Device = device;
                property.CameraControl = cameraControl;
                property.VideoProcAmp = videoProcAmp;

                if ( cameraControl != null && property.CamProperty != null )
                {
                    cameraControl.GetRange( ( CameraControlProperty ) property.CamProperty, out property.Min, out property.Max, out property.Delta, out property.Default, out property.CamFlags );

                    if ( property.CamFlags == CameraControlFlags.None ) continue;

                    var propItem = new PropertyItem( property );

                    Controls.Add( propItem );

                    propItem.Dock = DockStyle.Top;
                }

                else if ( videoProcAmp != null && property.AmpProperty != null )
                {
                    videoProcAmp.GetRange( ( VideoProcAmpProperty ) property.AmpProperty, out property.Min, out property.Max, out property.Delta, out property.Default, out property.AmpFlags );

                    if ( property.AmpFlags == VideoProcAmpFlags.None ) continue;

                    var propItem = new PropertyItem( property );

                    Controls.Add( propItem );

                    propItem.Dock = DockStyle.Top;
                }
            }

            _updateAuto = new Timer( OnTick, null, EVALUATION_START_DELAY_MSEC, EVALUATION_INTERVAL_MSEC );
        }

        private void OnTick( object sender )
        {
            if ( InvokeRequired )
            {
                Invoke( ( Action ) UpdateItems );
            }
            else
            {
                UpdateItems();
            }
        }

        private void UpdateItems()
        {
            foreach ( var item in Controls.OfType<PropertyItem>().Where( x => x.IsAuto ) )
            {
                item.UpdateValue();
            }
        }

        private void btnSetDefaults_Click( object sender, EventArgs e )
        {
            foreach ( var item in Controls.OfType<PropertyItem>().Where( x => !x.IsAuto ) )
            {
                item.SetDefaults();
            }
        }

        private GraphicsPath GetRoundPath( RectangleF rect, int radius )
        {
            var r2 = radius / 2f;
            var path = new GraphicsPath();

            path.AddArc( rect.X, rect.Y, radius, radius, 180, 90 );
            path.AddLine( rect.X + r2, rect.Y, rect.Width - r2, rect.Y );
            
            path.AddArc( rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90 );
            path.AddLine( rect.Width, rect.Y + r2, rect.Width, rect.Height - r2 );
            
            path.AddArc( rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90 );
            path.AddLine( rect.Width - r2, rect.Height, rect.X + r2, rect.Height );
            
            path.AddArc( rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90 );
            path.AddLine( rect.X, rect.Height - r2, rect.X, rect.Y + r2 );

            path.CloseFigure();

            return path;
        }

        private void CameraProperties_Load( object sender, EventArgs e )
        {
            var rect = btnSetDefaults.ClientRectangle;

            btnSetDefaults.Region = new Region( GetRoundPath( rect, 32 ) );

            rect = btnSave.ClientRectangle;

            btnSave.Region = new Region( GetRoundPath( rect, 32 ) );
        }

        public List<PropertyData> Properties => Controls.OfType<PropertyItem>().Select( x => x.Data ).ToList();
    }
}
