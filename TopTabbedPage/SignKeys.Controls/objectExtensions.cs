using System;
using Xamarin.Forms;

namespace SignKeys.Controls
{
    public interface IObservableDisposal
    {
        bool IsDisposed { get; }
    }
    public static class objectExtensions
    {
        public static void PerformOnMainThread<T>(this T obj, Action<T> action) where T: class, IObservableDisposal
        {
            var meRef = new WeakReference<T>(obj);
            Device.InvokeOnMainThreadAsync(() =>
            {
                if (meRef.TryGetTarget(out T me) && !me.IsDisposed)
                {
                    action.Invoke(me);
                }
            });
        }
    }
}
