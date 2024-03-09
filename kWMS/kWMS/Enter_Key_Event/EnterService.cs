using kWMS.Enter_Key_Event;
using Xamarin.Forms;

[assembly: Dependency(typeof(EnterService))]
namespace kWMS.Enter_Key_Event
{
    public class EnterService : IEnterService
    {
        public void PressEnter()
        {

            /*Instrumentation inst = new Instrumentation();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                inst.SendKeyDownUpSync(Keycode.Enter);
            }).Start();*/
        }
    }
}
