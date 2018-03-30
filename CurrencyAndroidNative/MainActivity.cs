using Android.App;
using Android.Widget;
using Android.OS;

namespace CurrencyAndroidNative
{
    [Activity(Label = "CurrencyAndroidNative", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView _textView1;
        EditText _editText1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //var cos = _textView1.Text;
            _textView1 = (TextView)FindViewById(Resource.Id.textView1);
            _editText1 = (EditText)FindViewById(Resource.Id.editText1);
            
            _editText1.TextChanged += _editText1_TextChanged;
        }

        private void _editText1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            _textView1.Text = _editText1.Text;
        }
    }
}

