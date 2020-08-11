using System;
using System.Timers;

namespace MyTimer
{
    public class MyTimer
    {
        private volatile Timer _timer = new Timer();
        private volatile bool _requestStop = false;
        private MyElapsedEventHandler _eventHander;
        private MyElapsedEventHandlerWithParam _eventHandlerWithParam;
        private object _param;

        /**public MyTimer(int interval, MyElapsedEventHandler elapsedEventHandler, bool autoReset = false)
        {
            _timer.Interval = interval;
            _timer.Elapsed += ElapsedWrapper;
            _timer.AutoReset = autoReset;

            _eventHander = elapsedEventHandler;

            Start();
        }**/

        public MyTimer(int interval, MyElapsedEventHandlerWithParam elapsedEventHandler, object param, bool autoReset = false)
        {
            _timer.Interval = interval;
            _timer.Elapsed += ElapsedWrapperWithParam;
            _timer.AutoReset = autoReset;

            _eventHandlerWithParam = elapsedEventHandler;
            _param = param;
            Start();
        }

        private void ElapsedWrapper(object sender, ElapsedEventArgs e)
        {
            if (!_requestStop && _eventHander != null)
            {
                _eventHander();
            }
        }

        private void ElapsedWrapperWithParam(object sender, ElapsedEventArgs e)
        {
            if (!_requestStop && _eventHandlerWithParam != null)
            {
                _eventHandlerWithParam(_param);
            }
        }

        public void Stop()
        {
            _requestStop = true;
            _timer.Stop();
        }

        public void Start()
        {
            _requestStop = false;
            _timer.Start();
        }
    }

    public delegate void MyElapsedEventHandlerWithParam(object param);
    public delegate void MyElapsedEventHandler();
}