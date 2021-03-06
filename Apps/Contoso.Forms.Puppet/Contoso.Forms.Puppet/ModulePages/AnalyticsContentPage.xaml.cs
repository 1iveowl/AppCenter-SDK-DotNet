﻿using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public class Property
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Property(string propertyName, string propertyValue)
        {
            Name = propertyName;
            Value = propertyValue;
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AnalyticsContentPage : ContentPage
    {
        List<Property> EventProperties;

        public AnalyticsContentPage()
        {
            InitializeComponent();
            EventProperties = new List<Property>();
            NumPropertiesLabel.Text = EventProperties.Count.ToString();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "lightning.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            EnabledSwitchCell.On = await Analytics.IsEnabledAsync();
            EnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();
        }

        async void AddProperty(object sender, EventArgs e)
        {
            var addPage = new AddPropertyContentPage();
            addPage.PropertyAdded += (Property property) =>
            {
                EventProperties.Add(property);
                RefreshPropCount();
            };
            await Navigation.PushModalAsync(addPage);
        }

        async void PropertiesCellTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PropertiesContentPage(EventProperties));
        }

        void TrackEvent(object sender, EventArgs e)
        {
            var properties = new Dictionary<string, string>();
            foreach (Property property in EventProperties)
            {
                properties.Add(property.Name, property.Value);
            }

            if (EventProperties.Count == 0)
            {
                Analytics.TrackEvent(EventNameCell.Text);
                return;
            }

            EventProperties.Clear();
            RefreshPropCount();
            Analytics.TrackEvent(EventNameCell.Text, properties);

        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await Analytics.SetEnabledAsync(e.Value);
        }

        void RefreshPropCount()
        {
            NumPropertiesLabel.Text = EventProperties.Count.ToString();
        }
    }
}
