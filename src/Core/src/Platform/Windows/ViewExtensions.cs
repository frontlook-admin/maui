#nullable enable
using Microsoft.Graphics.Canvas;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;

namespace Microsoft.Maui
{
	public static class ViewExtensions
	{
		public static void TryMoveFocus(this FrameworkElement nativeView, FocusNavigationDirection direction)
		{
			if (nativeView?.XamlRoot?.Content is UIElement elem)
				FocusManager.TryMoveFocus(direction, new FindNextElementOptions { SearchRoot = elem });
		}

		public static void UpdateIsEnabled(this FrameworkElement nativeView, IView view) =>
			(nativeView as Control)?.UpdateIsEnabled(view.IsEnabled);

		public static void UpdateVisibility(this FrameworkElement nativeView, IView view)
		{
			double opacity = view.Opacity;

			switch (view.Visibility)
			{
				case Visibility.Visible:
					nativeView.Opacity = opacity;
					nativeView.Visibility = UI.Xaml.Visibility.Visible;
					break;
				case Visibility.Hidden:
					nativeView.Opacity = 0;
					nativeView.Visibility = UI.Xaml.Visibility.Visible;
					break;
				case Visibility.Collapsed:
					nativeView.Opacity = opacity;
					nativeView.Visibility = UI.Xaml.Visibility.Collapsed;
					break;
			}
		}

		public static void UpdateClip(this FrameworkElement nativeView, IView view)
		{
			var clipGeometry = view.Clip;
			if (clipGeometry == null)
				return;

			if (view.Handler?.MauiContext?.Window is not Window window)
				return;

			var compositor = window.Compositor;
			var visual = ElementCompositionPreview.GetElementVisual(nativeView);

			var pathSize = new Rectangle(0, 0, view.Width, view.Height);
			var clipPath = clipGeometry.PathForBounds(pathSize);
			var device = CanvasDevice.GetSharedDevice();
			var geometry = clipPath.AsPath(device);

			var path = new CompositionPath(geometry);
			var pathGeometry = compositor.CreatePathGeometry(path);
			var geometricClip = compositor.CreateGeometricClip(pathGeometry);

			visual.Clip = geometricClip;
		}

		public static void UpdateOpacity(this FrameworkElement nativeView, IView view)
		{
			nativeView.Opacity = view.Visibility == Visibility.Hidden ? 0 : view.Opacity;
		}

		public static void UpdateBackground(this FrameworkElement nativeView, IView view)
		{
			if (nativeView is Control control)
				control.UpdateBackground(view.Background);
			else if (nativeView is Border border)
				border.UpdateBackground(view.Background);
			else if (nativeView is Panel panel)
				panel.UpdateBackground(view.Background);
		}

		public static void UpdateBorderBrush(this FrameworkElement nativeView, IView view) 
		{
			if (nativeView is Border wrapperView)
				wrapperView.BorderBrush = view.BorderBrush?.ToNative();
		}

		public static void UpdateBorderWidth(this FrameworkElement nativeView, IView view)
		{
			if (nativeView is Border wrapperView)
				wrapperView.BorderThickness = new UI.Xaml.Thickness(view.BorderWidth);
		}

		public static void UpdateCornerRadius(this FrameworkElement nativeView, IView view)
		{
			if (nativeView is Border wrapperView)
			{
				CornerRadius cornerRadius = view.CornerRadius;
				wrapperView.CornerRadius = WinUIHelpers.CreateCornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomRight);
			}
		}

		public static void UpdateAutomationId(this FrameworkElement nativeView, IView view) =>
			AutomationProperties.SetAutomationId(nativeView, view.AutomationId);

		public static void UpdateSemantics(this FrameworkElement nativeView, IView view)
		{
			var semantics = view.Semantics;
			if (semantics == null)
				return;

			AutomationProperties.SetName(nativeView, semantics.Description);
			AutomationProperties.SetHelpText(nativeView, semantics.Hint);
			AutomationProperties.SetHeadingLevel(nativeView, (UI.Xaml.Automation.Peers.AutomationHeadingLevel)((int)semantics.HeadingLevel));
		}

		internal static void UpdateProperty(this FrameworkElement nativeControl, DependencyProperty property, Color color)
		{
			if (color.IsDefault())
				nativeControl.ClearValue(property);
			else
				nativeControl.SetValue(property, color.ToNative());
		}

		internal static void UpdateProperty(this FrameworkElement nativeControl, DependencyProperty property, object? value)
		{
			if (value == null)
				nativeControl.ClearValue(property);
			else
				nativeControl.SetValue(property, value);
		}

		public static void InvalidateMeasure(this FrameworkElement nativeView, IView view)
		{
			nativeView.InvalidateMeasure();
		}

		public static void UpdateWidth(this FrameworkElement nativeView, IView view)
		{
			// WinUI uses NaN for "unspecified"
			nativeView.Width = view.Width >= 0 ? view.Width : double.NaN;
		}

		public static void UpdateHeight(this FrameworkElement nativeView, IView view)
		{
			// WinUI uses NaN for "unspecified"
			nativeView.Height = view.Height >= 0 ? view.Height : double.NaN;
		}
	}
}