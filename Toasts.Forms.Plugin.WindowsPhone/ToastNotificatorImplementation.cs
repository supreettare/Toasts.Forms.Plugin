﻿using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Toasts.Forms.Plugin.Abstractions;
using Toasts.Forms.Plugin.WindowsPhone;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastNotificatorImplementation))]
namespace Toasts.Forms.Plugin.WindowsPhone
{
    public class ToastNotificatorImplementation : IToastNotificator
    {
        private static IToastLayoutCustomRenderer _customRenderer;

        /// <summary>
        /// Should be called after Xamarin.Forms.Init();
        /// </summary>
        /// <param name="stackSize">max toast messages count - not implemented on ios and android yet - they show only 1 toast max</param>
        /// <param name="customRenderer">you can override default layout by passing custom renderer, null means DefaultToastLayoutRenderer</param>
        public static void Init(int stackSize = 3, IToastLayoutCustomRenderer customRenderer = null)
        {
            ToastPromtsHostControl.MaxToastCount = stackSize;
            _customRenderer = customRenderer ?? new DefaultToastLayoutRenderer();
            ToastInjector.Inject();
        }

        public Task<bool> Notify(ToastNotificationType type, string title, string description, TimeSpan duration, object context)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            Brush brush;
            var element = _customRenderer.Render(type, title, description, context, out brush);

            ToastPromtsHostControl.EnqueueItem(element, b => taskCompletionSource.TrySetResult(b), brush, 
                tappable: _customRenderer.IsTappable, 
                timeout: duration, 
                showCloseButton: _customRenderer.HasCloseButton);
            return taskCompletionSource.Task;
        }

        public void HideAll()
        {
            ToastPromtsHostControl.Clear();
        }
    }
}
