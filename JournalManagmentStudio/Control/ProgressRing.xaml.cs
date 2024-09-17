using System.Windows;

namespace JournalManagmentStudio.Journal.Control
{
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class ProgressRing : System.Windows.Controls.Control
    {
        /// <summary>
        /// Identifies the EllipseDiameterTemplateSetting dependency property.
        /// </summary>
        public static readonly DependencyProperty EllipseDiameterTemplateSettingProperty =
            DependencyProperty.Register("EllipseDiameterTemplateSetting", typeof(double), typeof(ProgressRing), new PropertyMetadata(default(double)));

        /// <summary>
        /// Identifies the EllipseOffsetTemplateSetting dependency property.
        /// </summary>
        public static readonly DependencyProperty EllipseOffsetTemplateSettingProperty =
          DependencyProperty.Register("EllipseOffsetTemplateSetting", typeof(Thickness), typeof(ProgressRing), new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Identifies the IsActive dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
           DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new PropertyMetadata(false, IsActiveChanged));

        /// <summary>
        /// Identifies the MaxSideLengthTemplateSetting dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxSideLengthTemplateSettingProperty =
             DependencyProperty.Register("MaxSideLengthTemplateSetting", typeof(double), typeof(ProgressRing), new PropertyMetadata(default(double)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressRing"/> class.
        /// </summary>
        public ProgressRing()
        {
            this.SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Gets the template-defined diameter of the "Ellipse" element that is animated in a templated ProgressRing.
        /// </summary>
        public double EllipseDiameterTemplateSetting
        {
            get { return (double)GetValue(EllipseDiameterTemplateSettingProperty); }
            private set { SetValue(EllipseDiameterTemplateSettingProperty, value); }
        }

        /// <summary>
        /// Gets the template-defined offset position of the "Ellipse" element that is animated in a templated ProgressRing.
        /// </summary>
        public Thickness EllipseOffsetTemplateSetting
        {
            get { return (Thickness)GetValue(EllipseOffsetTemplateSettingProperty); }
            private set { SetValue(EllipseOffsetTemplateSettingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the ProgressRing is showing progress.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Gets the maximum bounding size of the progress ring as rendered.
        /// </summary>
        public double MaxSideLengthTemplateSetting
        {
            get { return (double)GetValue(MaxSideLengthTemplateSettingProperty); }
            private set { SetValue(MaxSideLengthTemplateSettingProperty, value); }
        }

        /// <summary>
        /// Is called when the template of the ProgressRing control is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateActiveState();
        }

        /// <summary>
        /// Is called when the IsActive dependency property is changed.
        /// </summary>
        /// <param name="d">The object that it's property is changed.</param>
        /// <param name="e">Event args of the property changed event.</param>
        private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressRing = d as ProgressRing;
            if (progressRing != null)
            {
                progressRing.UpdateActiveState();
            }
        }

        /// <summary>
        /// Is called when the size of the ProgressRing control is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateMaxSideLength();
            this.UpdateEllipseDiameter();
            this.UpdateEllipseOffset();
        }

        /// <summary>
        /// Update the visual state of a ProgressRing control.
        /// </summary>
        private void UpdateActiveState()
        {
            if (this.IsActive)
            {
                VisualStateManager.GoToState(this, "Active", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Inactive", true);
            }
        }

        /// <summary>
        /// Update the value of EllipseDiameter property.
        /// </summary>
        private void UpdateEllipseDiameter()
        {
            if (this.ActualWidth <= 25)
            {
                this.EllipseDiameterTemplateSetting = 3;
            }
            else
            {
                this.EllipseDiameterTemplateSetting = (this.ActualWidth / 10) + 0.5;
            }
        }

        /// <summary>
        /// Update the value of EllipseOffset property.
        /// </summary>
        private void UpdateEllipseOffset()
        {
            if (this.ActualWidth <= 25)
            {
                this.EllipseOffsetTemplateSetting = new Thickness(0, 7, 0, 0);
                return;
            }
            else if (this.ActualWidth <= 30)
            {
                var top = this.ActualWidth * (9 / 20) - (9 / 2);
                this.EllipseOffsetTemplateSetting = new Thickness(0, top, 0, 0);
                return;
            }
            else
            {
                var top = this.ActualWidth * (2 / 5) - 2;
                this.EllipseOffsetTemplateSetting = new Thickness(0, top, 0, 0);
            }
        }

        /// <summary>
        /// Update the value of MaxSideLength property.
        /// </summary>
        private void UpdateMaxSideLength()
        {
            if (this.ActualWidth <= 25)
            {
                this.MaxSideLengthTemplateSetting = 20;
            }
            else
            {
                this.MaxSideLengthTemplateSetting = this.ActualWidth - 5;
            }
        }
    }
}