using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace TreehopperControlCenter
{
    public class BindablePicker : Picker
    {
        Boolean _disableNestedCalls = false;

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker), null, propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(Object), typeof(BindablePicker), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);
        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create("SelectedValue", typeof(Object), typeof(BindablePicker), null, BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

        public String DisplayMemberPath { get; set; }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                if (SelectedItem != value)
                {
                    SetValue(SelectedItemProperty, value);
                    InternalUpdateSelectedIndexSelectedValue();
                }
            }
        }

        public Object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
                InternalUpdateSelectedIndexSelectedValue();
            }
        }

        public String SelectedValuePath { get; set; }

        public BindablePicker()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        void InternalUpdateSelectedIndexSelectedItem()
        {
            if (_disableNestedCalls)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(SelectedValuePath))
            {
                return;
            }
            var selectedIndex = -1;
            Object selectedItem = null;
            if (ItemsSource != null)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    if (item != null)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(SelectedValuePath);
                        if (prop.GetValue(item) == SelectedValue)
                        {
                            selectedIndex = index;
                            selectedItem = item;
                            break;
                        }
                    }

                    index++;
                }
            }
            _disableNestedCalls = true;
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        void InternalUpdateSelectedIndexSelectedValue()
        {
            if (_disableNestedCalls)
            {
                return;
            }

            var selectedIndex = -1;
            Object selectedValue = null;
            if (ItemsSource != null)
            {
                var index = 0;
                var hasSelectedValuePath = !String.IsNullOrWhiteSpace(SelectedValuePath);
                foreach (var item in ItemsSource)
                {
                    if (item != null && item.Equals(SelectedItem))
                    {
                        selectedIndex = index;
                        if (hasSelectedValuePath)
                        {
                            var type = item.GetType();
                            var prop = type.GetRuntimeProperty(SelectedValuePath);
                            selectedValue = prop.GetValue(item);
                        }
                        break;
                    }
                    index++;
                }
            }
            _disableNestedCalls = true;
            SelectedValue = selectedValue;
            SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        static void OnItemsSourceChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var picker = (BindablePicker)bindable;

            if (Equals(newValue, null) && !Equals(oldValue, null))
            {
                return;
            }

            picker.Items.Clear();

            if (!Equals(newValue, null))
            {
                var hasDisplayMemberPath = !String.IsNullOrWhiteSpace(picker.DisplayMemberPath);

                foreach (var item in (IEnumerable)newValue)
                {
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(picker.DisplayMemberPath);
                        picker.Items.Add(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        picker.Items.Add(item.ToString());
                    }
                }
            }

            picker.InternalUpdateSelectedIndexSelectedValue();
        }

        void OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (SelectedIndex < 0 || ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext())
            {
                SelectedItem = null;
                SelectedValue = null;
                return;
            }

            var index = 0;
            var hasSelectedValuePath = !String.IsNullOrWhiteSpace(SelectedValuePath);
            foreach (var item in ItemsSource)
            {
                if (index == SelectedIndex)
                {
                    SelectedItem = item;
                    if (hasSelectedValuePath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(SelectedValuePath);
                        SelectedValue = prop.GetValue(item);
                    }

                    break;
                }
                index++;
            }
        }

        static void OnSelectedItemChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var boundPicker = (BindablePicker)bindable;
            if (boundPicker.ItemSelected != null)
            {
                boundPicker.ItemSelected.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
                boundPicker.InternalUpdateSelectedIndexSelectedValue();
            }
        }

        static void OnSelectedValueChanged(BindableObject bindable, Object oldvalue, Object newvalue)
        {
            var boundPicker = (BindablePicker)bindable;
            boundPicker.InternalUpdateSelectedIndexSelectedItem();
        }
    }
}
