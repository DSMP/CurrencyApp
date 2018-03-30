using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;

namespace CurrencyAndroidNative
{
    [Activity(Label = "CurrencyAndroidNative", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView _textView1;
        RecyclerView _mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //var cos = _textView1.Text;
            _textView1 = (TextView)FindViewById(Resource.Id.textView1);
            _mRecyclerView = FindViewById<RecyclerView>(Resource.Id.DatesRecyclerView);
        }
    }
}

