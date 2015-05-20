using System;
using System.Collections.Generic;
using System.Windows;

using System.Windows.Controls;
using Microsoft.Devices;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using WPCordovaClassLib.Cordova.UI;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Media;

/**
 * Navbar Plugin for windows phone
 */
namespace WPCordovaClassLib.Cordova.Commands
{
    public class NavBar : BaseCommand
    {
        private double previousHeight;
        private double softKeyHeight;
        private PhoneApplicationPage currentPage;
        private CordovaView CordovaView;
        private bool monitor = false;

        public NavBar()
        {
            previousHeight = System.Windows.Application.Current.Host.Content.ActualHeight;
            if (VisibleBoundsExtensions.IsSupported)
            {
                this.currentPage = ((PhoneApplicationFrame)Application.Current.RootVisual).Content as PhoneApplicationPage;

                try
                {
                    //Expected to be the grid
                    DependencyObject grid = VisualTreeHelper.GetChild(this.currentPage, 0);

                    int count = VisualTreeHelper.GetChildrenCount(grid);
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            UIElement child = (UIElement)VisualTreeHelper.GetChild(grid, i);

                            if (child is CordovaView)
                            {
                                this.CordovaView = (CordovaView)child;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to locate CordovaView in MainPage" + e.Message);
                    return;
                }
                this.currentPage.VisibleBoundsChangedAdd(UpdateBounds);
                this.currentPage.OrientationChanged += UpdateView;
                UpdateBounds(null, null);
            }
        }

        public void start(string options)
        {
            monitor = true;
            this.DispatchResult();
        }

        public void stop(string options)
        {
            monitor = false;
        }

        private void UpdateBounds(object sender, EventArgs args)
        {
            Thickness bounds = this.currentPage.GetVisibleBounds();
            double calculated = 0;
            if (this.currentPage.Orientation == PageOrientation.Portrait || this.currentPage.Orientation == PageOrientation.PortraitUp || this.currentPage.Orientation == PageOrientation.PortraitDown)
            {
                calculated = System.Windows.Application.Current.Host.Content.ActualHeight - (this.currentPage.Orientation == PageOrientation.PortraitUp ? bounds.Bottom : bounds.Top);
            }
            else if (this.currentPage.Orientation == PageOrientation.Landscape || this.currentPage.Orientation == PageOrientation.LandscapeLeft || this.currentPage.Orientation == PageOrientation.LandscapeRight)
            {
                calculated = System.Windows.Application.Current.Host.Content.ActualHeight - (this.currentPage.Orientation == PageOrientation.LandscapeLeft ? bounds.Right : bounds.Left);
            }

            double difference = Math.Round(previousHeight - calculated);

            if (previousHeight != calculated && Math.Abs(difference) < System.Windows.Application.Current.Host.Content.ActualHeight / 8)
            {
                this.softKeyHeight = difference > 0 ? difference : 0;
            }

            previousHeight = calculated;
            this.UpdateView(null, null);

            if (monitor)
            {
                this.DispatchResult();
            }

            //this.CordovaView.CordovaBrowser.InvokeScript("eval", new string[] { "cordova.fireDocumentEvent('navbarchange', {height:" + softKeyHeight + "});" });
        }

        private void UpdateView(object sender, EventArgs args)
        {
            if (this.currentPage.Orientation == PageOrientation.Portrait || this.currentPage.Orientation == PageOrientation.PortraitUp || this.currentPage.Orientation == PageOrientation.PortraitDown)
            {
                this.CordovaView.CordovaBrowser.Height = System.Windows.Application.Current.Host.Content.ActualHeight - softKeyHeight;
                this.CordovaView.CordovaBrowser.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
                this.CordovaView.CordovaBrowser.VerticalAlignment = (this.currentPage.Orientation == PageOrientation.PortraitUp ? VerticalAlignment.Top : VerticalAlignment.Bottom);
            }
            else if (this.currentPage.Orientation == PageOrientation.Landscape || this.currentPage.Orientation == PageOrientation.LandscapeLeft || this.currentPage.Orientation == PageOrientation.LandscapeRight)
            {
                this.CordovaView.CordovaBrowser.Height = System.Windows.Application.Current.Host.Content.ActualWidth;
                this.CordovaView.CordovaBrowser.Width = System.Windows.Application.Current.Host.Content.ActualHeight - softKeyHeight;
                this.CordovaView.CordovaBrowser.HorizontalAlignment = (this.currentPage.Orientation == PageOrientation.LandscapeLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right);
            }
        }

        private void DispatchResult()
        {
            PluginResult result = new PluginResult(PluginResult.Status.OK, softKeyHeight);
            result.KeepCallback = true;
            DispatchCommandResult(result);
        }
    }
}
