using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Tesseract;
using Tesseract.Droid;
using TinyIoC;

namespace Burton.Android
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static Activity CurrentActivity { get; set; }

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            var container = TinyIoCContainer.Current;
            container.Register<ITesseractApi>((cont, parameters) => new TesseractApi(
                ApplicationContext, 
                AssetsDeployment.OncePerInitialization));

            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CurrentActivity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CurrentActivity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CurrentActivity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}