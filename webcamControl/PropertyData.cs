using DirectShowLib;

namespace webcamControl
{
    public class PropertyData
    {
        public int Min = 0;
        public int Max = 0;
        public int Default = 0;
        public int Delta = 0;
        public int Value;

        public string Text = string.Empty;

        public DsDevice Device;
        public IAMCameraControl CameraControl;
        public IAMVideoProcAmp VideoProcAmp;

        public CameraControlProperty? CamProperty = null;
        public CameraControlFlags CamFlags = CameraControlFlags.None;

        public VideoProcAmpProperty? AmpProperty = null;
        public VideoProcAmpFlags AmpFlags = VideoProcAmpFlags.None;
    }
}