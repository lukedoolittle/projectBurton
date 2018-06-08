using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Burton.Android
{
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity, TextureView.ISurfaceTextureListener
    {
        private global::Android.Hardware.Camera _camera;
        private SurfaceTexture _surface;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void RequestCamera()
        {
            string[] PermissionsLocation =
            {
                Manifest.Permission.Camera
            };
            const string permission = Manifest.Permission.Camera;

            if ((int)Build.VERSION.SdkInt >= 23 && CheckSelfPermission(permission) != (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, 0);
            }
            else
            {
                StartCamera();
            }
        }

        private void StartCamera()
        {
            _camera = global::Android.Hardware.Camera.Open();

            try
            {
                _camera.SetPreviewTexture(_surface);
                _camera.StartPreview();

            }
            catch (Java.IO.IOException ex)
            {
                throw new Exception();
            }
        }

        private void Shutdown()
        {
            _camera?.StopPreview();
            _camera?.Release();
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            if (requestCode == 0)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    StartCamera();
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                base.OnRequestPermissionsResult(
                    requestCode, 
                    permissions, 
                    grantResults);
            }
        }

        public virtual void OnSurfaceTextureAvailable(
            SurfaceTexture surface, 
            int width, 
            int height)
        {
            _surface = surface;
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            Shutdown();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(
            SurfaceTexture surface, 
            int width, 
            int height)
        {
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }
    }
}