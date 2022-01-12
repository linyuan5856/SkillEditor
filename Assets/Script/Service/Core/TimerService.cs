using System;
using System.Collections.Generic;
using System.Diagnostics;
using BluePro.FrameWork;
using UnityEngine;

public class TimerService : BaseService
{
    public const float INFINITY = -9999;
    private bool openLog = false;

    public class Timer
    {
        private int identifyId;
        private float interval;
        private float life;
        private object[] param;
        private Action<bool, object[]> callBack;
        private bool isStop;
        private bool isDelete;

        private float startTime;
        private float lastUpdateTime;

        public float StartTime
        {
            get => startTime;
            set => startTime = value;
        }

        public bool IsStop
        {
            get => isStop;
            set => isStop = value;
        }

        public bool IsDelete
        {
            get => isDelete;
            set => isDelete = value;
        }

        public float Life
        {
            get => life;
            set => life = value;
        }

        public int IdentifyId => identifyId;

        public void SetTimer(int id, Action<bool, object[]> callBack, float interval, float life, object[] param = null)
        {
            this.identifyId = id;
            this.interval = interval;
            this.life = life;
            this.callBack = callBack;
            this.param = param;
        }


        public bool Tick(float time)
        {
            if (time - lastUpdateTime >= interval)
            {
                callBack(false, param);
                lastUpdateTime = time;
            }

            if (Math.Abs(life - INFINITY) > 1)
            {
                if (time - startTime >= life)
                    isDelete = true;
            }

            return isDelete;
        }

        public void Reset()
        {
            this.callBack(true, param);
            this.startTime = 0;
            this.lastUpdateTime = 0;
            this.life = INFINITY;
            this.callBack = null;
            this.param = null;
            this.isStop = false;
            this.isDelete = false;
        }
    }

    private int identifyId;
    public float Interval { get; set; } = 0.1f;
    private float unscaleCurrentTime;


    private MapList<int, Timer> timeMap = new MapList<int, Timer>();
    private Queue<Timer> pool = new Queue<Timer>();

    public int CreateTimer(Action<bool, object[]> callBack, float interval = 1.0f, float life = INFINITY,
        object[] param = null)
    {
        Timer timer = null;
        this.identifyId++;

        if (pool.Count > 0)
        {
            timer = pool.Dequeue();
            DebugTime(LogType.Log, string.Format("[{0}]  Create Timer From Pool-> {1} Life->{2}",
                this.GetType().Name, this.identifyId, life));
        }
        else
        {
            timer = new Timer();
            DebugTime(LogType.Log, string.Format("[{0}]  Create Timer Use NEW-> {1} Life->{2}",
                this.GetType().Name, this.identifyId, life));
        }

        timer.SetTimer(this.identifyId, callBack, interval, life, param);
        timer.StartTime = GetTimeNow();
        this.timeMap.Add(identifyId, timer);
        return identifyId;
    }

    public void RefreshTimerLife(int id, float life)
    {
        var timer = GetTimer(id);
        if (timer != null)
        {
            timer.StartTime = GetTimeNow();
            timer.Life = life;
            DebugTime(LogType.Log,
                string.Format("[{0}]  Refresh Timer ID->{1} Life->{2}", this.GetType().Name, id, life));
        }
    }

    public void StopTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsStop = true;
        DebugTime(LogType.Log, string.Format("[{0}]  Stop Timer ID->{1}", this.GetType().Name, id));
    }

    public void ResumeTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsStop = false;
        DebugTime(LogType.Log, string.Format("[{0}]  Resume Timer ID->{1}", this.GetType().Name, id));
    }


    public void DeleteTimer(int id)
    {
        var timer = GetTimer(id);
        if (timer != null)
            timer.IsDelete = true;
        DebugTime(LogType.Log, string.Format("[{0}]  Delete Timer ID->{1}", this.GetType().Name, id));
    }

    private float GetTimeNow()
    {
        return unscaleCurrentTime;
    }

    private Timer GetTimer(int id)
    {
        if (timeMap.ContainsKey(id))
        {
            var time = timeMap[id];
            return time;
        }

        DebugTime(LogType.Error, string.Format("[{0}] ID->{1} is not Exist", this.GetType().Name, id));
        return null;
    }

    private float tempRecordUnScaled;
    private readonly List<int> willDeleteTimerList = new List<int>();

    public override void DoUpdate(float deltaTime)
    {
        base.DoUpdate(deltaTime);
        unscaleCurrentTime += Time.unscaledDeltaTime;
        tempRecordUnScaled += Time.unscaledDeltaTime;
        if (tempRecordUnScaled >= Interval)
        {
            willDeleteTimerList.Clear();
            tempRecordUnScaled -= Interval;
            var list = this.timeMap.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                var timer = list[i];
                if (timer == null)
                {
                    DebugTime(LogType.Error, string.Format("[{0}] List Index->{1}  Timer is null",
                        this.GetType().Name, i));
                    continue;
                }

                //timer.Tick(float time) return true,this timer will be deleted（life is over）
                if (timer.IsDelete ||
                    !timer.IsStop && !timer.IsDelete && timer.Tick(GetTimeNow()))
                    willDeleteTimerList.Add(timer.IdentifyId);
            }

            for (int i = 0; i < willDeleteTimerList.Count; i++)
            {
                int id = willDeleteTimerList[i];
                var timer = this.GetTimer(id);
                timer.Reset();
                this.timeMap.Remove(id);
                this.pool.Enqueue(timer);
                DebugTime(LogType.Log, string.Format("[{0}]  Timer Life Is End ID->{1}", this.GetType().Name, id));
            }
        }
    }


    [Conditional("UNITY_EDITOR")]
    private void DebugTime(LogType type, object obj)
    {
        if (!openLog)
            return;

        switch (type)
        {
            case LogType.Error:
                UnityEngine.Debug.LogError(obj);
                break;
            case LogType.Warning:
                UnityEngine.Debug.LogWarning(obj);
                break;
            case LogType.Log:
                UnityEngine.Debug.Log(obj);
                break;
        }
    }
}