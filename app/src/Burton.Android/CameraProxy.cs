using Android.Graphics;

namespace Burton.Android
{
    public class CameraProxy
    {
        private global::Android.Hardware.Camera _camera;


        public void Start(SurfaceTexture surface)
        {
            _camera = global::Android.Hardware.Camera.Open();

            try
            {
                _camera.SetPreviewTexture(surface);
                _camera.StartPreview();

            }
            catch (Java.IO.IOException ex)
            {
            }
        }

        public void Shutdown()
        {
            _camera?.StopPreview();
            _camera?.Release();
        }
    }
}