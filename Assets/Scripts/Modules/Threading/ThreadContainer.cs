using System.Threading;

namespace Threading
{
    public class ThreadContainer
    {
        Thread _t;
        bool _gotoDestroy = false;
        bool _isDestroy = false;

        bool _isRunedAwake = false;
        bool _isRunedStart = false;

        bool _isEnable = false;
        bool _needEnable = true;

        public ThreadContainer() : this(true)
        {
        }

        public ThreadContainer(bool Active)
        {
            SetActive(Active);
            _t = new Thread(Main);
            _t.Start();
        }

        public void SetActive(bool active)
        {
            _needEnable = active;
        }

        public void Destroy()
        {
            _gotoDestroy = true;
        }

        public bool IsActive { get => _isEnable; }
        public bool IsDestroy { get => _isDestroy; }

        void Main()
        {
            while (!_gotoDestroy)
            {
                if (_needEnable)
                {
                    if (!_isRunedAwake)
                    {
                        _isRunedAwake = true;
                        Awake();
                        continue;
                    }

                    if (!_isEnable)
                    {
                        _isEnable = true;
                        OnEnable();
                    }

                    if (!_isRunedStart)
                    {
                        _isRunedStart = true;
                        Start();
                    }
                    Update();
                }

                if (!_needEnable || _gotoDestroy)
                {
                    if (_isEnable)
                    {
                        _isEnable = false;
                        OnDisable();
                    }
                }
            }
            OnDestroy();

            _isDestroy = true;
            _t.Abort();
            _t = null;
        }


        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void Update() { }
        protected virtual void OnDisable() { }
        protected virtual void OnDestroy() { }
    }
}