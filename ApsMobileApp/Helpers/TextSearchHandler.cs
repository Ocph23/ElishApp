using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ApsMobileApp.Helpers;

public class TextSearchHandler : SearchHandler
{

    public event ResultFound OnSearchFound;
    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        base.OnQueryChanged(oldValue, newValue);
        OnSearchFound?.Invoke(newValue);
    }
}
