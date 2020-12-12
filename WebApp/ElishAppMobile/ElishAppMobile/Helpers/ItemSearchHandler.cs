using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ElishAppMobile.Helpers
{
    public delegate void ResultFound(object data);
    public class ItemSearchHandler : SearchHandler
    {

        public IEnumerable<dynamic> Source { get; private set; }

        public event ResultFound OnSearchFound;


        public void SetItemSource<T>(IEnumerable<T> source) where T : class
        {
            Source = source;
        }
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                //ItemsSource = Source.Where(x => x.Nomor.ToString().ToLower().Contains(newValue.ToLower()));

            }
        }

        protected override void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            OnSearchFound?.Invoke(item);
        }







    }
}
