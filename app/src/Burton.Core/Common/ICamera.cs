using System;

namespace Burton.Core.Common
{
    public interface ICamera
    {
        event EventHandler<PreviewImageEventArgs> GeneratedPreviewImage;
    }
}
