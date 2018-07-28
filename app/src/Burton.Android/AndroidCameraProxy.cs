using System;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Burton.Core.Common;
using Permission = Android.Content.PM.Permission;
#pragma warning disable 618
using Camera = Android.Hardware.Camera;
#pragma warning restore 618

namespace Burton.Android
{
    public class AndroidCameraProxy : 
        Java.Lang.Object, 
#pragma warning disable 618
        Camera.IPreviewCallback,
#pragma warning restore 618
        ICamera
    {
        //private const int PICTURE_REQUEST_CODE = 0;
        private const string CAMERA_PERMISSION = Manifest.Permission.Camera;
        private const int PERMISSION_REQUEST_CODE = 0;

#pragma warning disable CS0618 // Type or member is obsolete
        private Camera __camera;
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        private Camera _camera
        {
            get
            {
                if (__camera == null)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    __camera = Camera.Open();
#pragma warning restore CS0618 // Type or member is obsolete
                    var param = __camera.GetParameters();
#pragma warning disable CS0618 // Type or member is obsolete
                    param.FocusMode = Camera.Parameters.FocusModeContinuousPicture;
#pragma warning restore CS0618 // Type or member is obsolete
                    __camera.SetParameters(param);
                }

                return __camera;
            }
        }

        private readonly ReadingActivity _activity;
        private readonly int _previewFrequencyInFrames;
        private readonly ThreadSafeCounter _counter;
        private TaskCompletionSource<bool> _permissionSource;
        public event EventHandler<PreviewImageEventArgs> GeneratedPreviewImage;

        public AndroidCameraProxy(
            ReadingActivity activity,
            int previewFrequencyInFrames)
        {
            _activity = activity ?? throw new ArgumentNullException(nameof(activity));
            _counter = new ThreadSafeCounter();
            _previewFrequencyInFrames = previewFrequencyInFrames;
        }

        #region Permission
        public Task<bool> CanAccessCamera()
        {
            if ((int)Build.VERSION.SdkInt >= 23 &&
                _activity.CheckSelfPermission(CAMERA_PERMISSION) != (int)Permission.Granted)
            {
                _permissionSource = new TaskCompletionSource<bool>();

                _activity.RequestPermissions(
                    new[] { CAMERA_PERMISSION },
                    PERMISSION_REQUEST_CODE);

                return _permissionSource.Task;
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        public void OnCameraPermissionFinished(
            object sender,
            PermissionResultEventArgs args)
        {
            if (args.RequestCode == PERMISSION_REQUEST_CODE)
            {
                if (args.GrantResults.Length == 1 && 
                    args.GrantResults[0] == Core.Common.Permission.Granted)
                {
                    _permissionSource.SetResult(true);
                }
                else
                {
                    _permissionSource.SetResult(false);
                }
            }
        }

        #endregion Permission

        #region Preview

        public void StartCameraPreview(SurfaceTexture surface)
        {
            if (surface == null) throw new ArgumentNullException(nameof(surface));

            _camera.SetPreviewTexture(surface);
            _camera.SetPreviewCallback(this);
            _camera.StartPreview();
        }

        public void ShutdownCameraPreview()
        {
            _camera.StopPreview();
            _camera.Release();
        }

        public void OnPreviewFrame(
            byte[] data,
#pragma warning disable CS0618 // Type or member is obsolete
            Camera camera)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            if (_counter.Next() % _previewFrequencyInFrames == 0)
            {
                var parameters = camera.GetParameters();

                GeneratedPreviewImage?.Invoke(
                    this,
                    new PreviewImageEventArgs
                    {
                        Image = data.ImageToJpeg(
                            parameters.PreviewSize.Width,
                            parameters.PreviewSize.Height,
                            parameters.PreviewFormat)
                    });
            }
        }

        #endregion Preview

    }
}