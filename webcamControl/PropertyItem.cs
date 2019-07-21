using System.Windows.Forms;
using DirectShowLib;

namespace webcamControl
{
    public partial class PropertyItem : UserControl
    {
        private readonly PropertyData _data;

        private double Scale( double x, double xmin, double xmax, double ymin, double ymax )
        {
            return ( x - xmin ) * ( ymax - ymin ) / ( xmax - xmin ) + ymin;
        }

        public PropertyItem( PropertyData data )
        {
            InitializeComponent();

            _data = data;

            if ( _data?.Device == null ) return;

            Initialize();
        }

        private void Initialize()
        {
            label1.Text = _data.Text;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;
            trackBar1.TickFrequency = 1;

            if ( _data.CamProperty != null )
            {
                _data.CameraControl.Get( ( CameraControlProperty ) _data.CamProperty, out _data.Value, out _data.CamFlags );

                trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );

                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                checkBox1.Checked = _data.CamFlags == CameraControlFlags.Auto || _data.CamFlags == ( CameraControlFlags.Auto | CameraControlFlags.Manual );
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            }
            else if ( _data.AmpProperty != null )
            {
                _data.VideoProcAmp.Get( ( VideoProcAmpProperty ) _data.AmpProperty, out _data.Value, out _data.AmpFlags );

                trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );

                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                checkBox1.Checked = _data.AmpFlags == VideoProcAmpFlags.Auto || _data.AmpFlags == ( VideoProcAmpFlags.Auto | VideoProcAmpFlags.Manual );
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            }

            trackBar1.Enabled = !checkBox1.Checked;
            checkBox1.Visible = checkBox1.Checked;
        }

        public void SetDefaults()
        {
            if ( _data?.CamProperty != null )
            {
                _data.CameraControl.GetRange( ( CameraControlProperty ) _data.CamProperty, out _data.Min, out _data.Max, out _data.Delta, out _data.Default, out _data.CamFlags );
                _data.CameraControl.Set( ( CameraControlProperty ) _data.CamProperty, _data.Default, _data.CamFlags );
            }
            else if ( _data?.AmpProperty != null )
            {
                _data.VideoProcAmp.GetRange( ( VideoProcAmpProperty ) _data.AmpProperty, out _data.Min, out _data.Max, out _data.Delta, out _data.Default, out _data.AmpFlags );
                _data.VideoProcAmp.Set( ( VideoProcAmpProperty ) _data.AmpProperty, _data.Default, _data.AmpFlags );
            }

            Initialize();
        }

        public void UpdateValue()
        {
            if ( _data.CamProperty != null )
            {
                _data.CameraControl.Get( ( CameraControlProperty ) _data.CamProperty, out _data.Value, out _data.CamFlags );

                trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );
            }
            else if ( _data.AmpProperty != null )
            {
                _data.VideoProcAmp.Get( ( VideoProcAmpProperty ) _data.AmpProperty, out _data.Value, out _data.AmpFlags );

                trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );
            }
        }

        private void checkBox1_CheckedChanged( object sender, System.EventArgs e )
        {
            if ( _data?.CamProperty != null )
            {
                trackBar1.Enabled = !checkBox1.Checked;

                // Auto.
                if ( checkBox1.Checked )
                {
                    //_data.CameraControl.Set( ( CameraControlProperty ) _data.CamProperty, _data.Min, CameraControlFlags.Manual );
                    _data.CameraControl.Set( ( CameraControlProperty ) _data.CamProperty, 0, CameraControlFlags.Auto );
                    _data.CameraControl.Get( ( CameraControlProperty ) _data.CamProperty, out _data.Value, out _data.CamFlags );

                    trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );
                }
                // Manual.
                else
                {
                    _data.CameraControl.Get( ( CameraControlProperty ) _data.CamProperty, out _data.Value, out _data.CamFlags );

                    var value = ( int ) Scale( trackBar1.Value, trackBar1.Minimum, trackBar1.Maximum, _data.Min, _data.Max );

                    _data.CameraControl.Set( ( CameraControlProperty ) _data.CamProperty, value, CameraControlFlags.Manual );
                }
            }
            else if ( _data?.AmpProperty != null )
            {
                trackBar1.Enabled = !checkBox1.Checked;

                // Auto.
                if ( checkBox1.Checked )
                {
                    //_data.VideoProcAmp.Set( ( VideoProcAmpProperty ) _data.AmpProperty, _data.Min, VideoProcAmpFlags.Manual );
                    _data.VideoProcAmp.Set( ( VideoProcAmpProperty ) _data.AmpProperty, 0, VideoProcAmpFlags.Auto );
                    _data.VideoProcAmp.Get( ( VideoProcAmpProperty ) _data.AmpProperty, out _data.Value, out _data.AmpFlags );

                    trackBar1.Value = ( int ) Scale( _data.Value, _data.Min, _data.Max, trackBar1.Minimum, trackBar1.Maximum );
                }
                // Manual.
                else
                {
                    _data.VideoProcAmp.Get( ( VideoProcAmpProperty ) _data.AmpProperty, out _data.Value, out _data.AmpFlags );

                    var value = ( int ) Scale( trackBar1.Value, trackBar1.Minimum, trackBar1.Maximum, _data.Min, _data.Max );

                    _data.VideoProcAmp.Set( ( VideoProcAmpProperty ) _data.AmpProperty, value, VideoProcAmpFlags.Manual );
                }
            }
        }

        private void trackBar1_Scroll( object sender, System.EventArgs e )
        {
            var value = ( int ) Scale( trackBar1.Value, trackBar1.Minimum, trackBar1.Maximum, _data.Min, _data.Max );

            if ( _data?.CamProperty != null )
            {
                _data.CameraControl.Set( ( CameraControlProperty ) _data.CamProperty, value, CameraControlFlags.Manual );
            }
            else if ( _data?.AmpProperty != null )
            {
                _data.VideoProcAmp.Set( ( VideoProcAmpProperty ) _data.AmpProperty, value, VideoProcAmpFlags.Manual );
            }
        }

        public bool IsAuto => checkBox1.Checked;

        public PropertyData Data => _data;
    }
}
