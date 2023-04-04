using System;
using System.Collections.Generic;
using System.Text;


namespace ApsMobileApp.Helpers;

public class CustomStepper : Grid
{
    readonly Button PlusBtn;
    readonly Button MinusBtn;
    readonly Entry Entry;

    public static readonly BindableProperty TextProperty =  BindableProperty.Create(propertyName: "Text", returnType: typeof(double),  
        declaringType: typeof(CustomStepper), defaultValue: (double)0,  defaultBindingMode: BindingMode.TwoWay);

    public double Text
    {
        get { return (double)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty IncrementProperty = BindableProperty.Create(propertyName: "Increment", returnType: typeof(double),
       declaringType: typeof(CustomStepper), defaultValue: (double)1, defaultBindingMode: BindingMode.TwoWay);

    public double Increment
    {
        get { return (double)GetValue(IncrementProperty); }
        set { SetValue(IncrementProperty, value); }
    }

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(propertyName: "Minimum", returnType: typeof(double),
     declaringType: typeof(CustomStepper), defaultValue: (double)0, defaultBindingMode: BindingMode.TwoWay);

    public double Minimum
    {
        get { return (double)GetValue(MinimumProperty); }
        set { SetValue(MinimumProperty, value); }
    }

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(propertyName: "Maximum", returnType: typeof(double),
     declaringType: typeof(CustomStepper), defaultValue: (double)0, defaultBindingMode: BindingMode.TwoWay);

    public double Maximum
    {
        get { return (double)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }


    public static readonly BindableProperty ShowEntryProperty = BindableProperty.Create(propertyName: "ShowEntry", returnType: typeof(bool),
    declaringType: typeof(CustomStepper), defaultValue: true, defaultBindingMode: BindingMode.TwoWay);

    public bool ShowEntry
    {
        get { return (bool)GetValue(ShowEntryProperty); }
        set { SetValue(ShowEntryProperty, value); }
    }



    public CustomStepper()
    {
        PlusBtn = new Button {  Text = "+",  TextColor=Colors.Black,  FontAttributes = FontAttributes.Bold, FontSize = 15, Padding=0 , Margin=0 };
        MinusBtn = new Button { Text = "-",  TextColor = Colors.Black, FontAttributes = FontAttributes.Bold, FontSize = 15, Padding=0, Margin=0 };
        switch (Device.RuntimePlatform)
        {

            case Device.UWP:
            case Device.Android:
                {
                    PlusBtn.BackgroundColor = Colors.LightGray;
                    MinusBtn.BackgroundColor = Colors.LightGray;
                    break;
                }
            case Device.iOS:
                {
                    PlusBtn.BackgroundColor = Colors.DarkGray;
                    MinusBtn.BackgroundColor = Colors.DarkGray;
                    break;
                }
        }

        PlusBtn.Clicked += PlusBtn_Clicked;
        MinusBtn.Clicked += MinusBtn_Clicked;
        Entry = new Entry { PlaceholderColor = Colors.Gray, FontSize=14,  Keyboard = Keyboard.Numeric, VerticalTextAlignment= TextAlignment.Center, 
            IsVisible=ShowEntry, 
            HorizontalTextAlignment=TextAlignment.Center, BackgroundColor = Color.FromHex("#3FFF") };
        Entry.Behaviors.Add(new DecimalNumberValidationBehavior());
        Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        Entry.TextChanged += Entry_TextChanged;


        this.RowDefinitions.Add(new RowDefinition());
        this.RowDefinitions.Add(new RowDefinition() { Height= GridLength.Auto} );
        this.RowDefinitions.Add(new RowDefinition() );

        this.ColumnDefinitions.Add(new ColumnDefinition());
        this.ColumnDefinitions.Add(new ColumnDefinition());
        this.ColumnDefinitions.Add(new ColumnDefinition());

        Grid.SetRow(MinusBtn, 0);   Grid.SetRowSpan(MinusBtn, 3);
        Grid.SetRow(Entry, 0); Grid.SetRowSpan(Entry, 3);
        Grid.SetRow(PlusBtn, 0); Grid.SetRowSpan(PlusBtn, 3);

        Grid.SetColumn(MinusBtn, 0);
        Grid.SetColumn(Entry, 1);
        Grid.SetColumn(PlusBtn, 2);

        Children.Add(MinusBtn);
        Children.Add(Entry);
        Children.Add(PlusBtn);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue))
            this.Text = Double.Parse(e.NewTextValue);

    }



    private void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text - Minimum < Minimum)
            Text = Minimum;
        else 
            Text -= Increment;
    }

    private void PlusBtn_Clicked(object sender, EventArgs e)
    {
        
        if ((Text+Increment) > Maximum)
            Text = Maximum;
        else
            Text += Increment;
    }
}

internal class DecimalNumberValidationBehavior : Behavior<View>
{
    protected override void OnAttachedTo(BindableObject bindable)
    {
        base.OnAttachedTo(bindable);
    }

}
