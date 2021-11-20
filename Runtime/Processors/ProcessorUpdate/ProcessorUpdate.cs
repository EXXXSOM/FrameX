using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;


[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]

public class ProcessorUpdate
{
    private int _countTicks;
    private int _countTicksFixed;
    private int _countTicksLate;
    private int _countTicksProc;
    private int _countTicksProcFixed;
    private int _countTicksProcLate;

    private readonly List<ITick> ticks = new List<ITick>(128);
    private readonly List<ITickFixed> ticksFixed = new List<ITickFixed>();
    private readonly List<ITickLate> ticksLate = new List<ITickLate>();
    private readonly List<ITick> ticksProc = new List<ITick>(32);
    private readonly List<ITickFixed> ticksFixedProc = new List<ITickFixed>(32);
    private readonly List<ITickLate> ticksLateProc = new List<ITickLate>(32);

    private int GetTicksCount => _countTicks + _countTicksFixed + _countTicksLate
    + _countTicksProcFixed + _countTicksProcLate + _countTicksProc;

    public void AddTick(object updateble)
    {
        ticks.Add(updateble as ITick);
        _countTicks++;
    }

    public void RemoveTick(object updateble)
    {
        if (ticks.Remove(updateble as ITick))
        {
            _countTicks--;
        }
    }

    public void AddTickFixed(object updateble)
    {
        ticksFixed.Add(updateble as ITickFixed);
        _countTicksFixed++;
    }

    public void RemoveTickFixed(object updateble)
    {
        if (ticksFixed.Remove(updateble as ITickFixed))
        {
            _countTicksFixed--;
        }
    }

    public void AddTickLate(object updateble)
    {
        ticksLate.Add(updateble as ITickLate);
        _countTicksLate++;
    }

    public void RemoveTickLate(object updateble)
    {
        if (ticksLate.Remove(updateble as ITickLate))
        {
            _countTicksLate--;
        }
    }

    public void Add(object updateble)
    {
        if (updateble is ITick tickable)
        {
            ticks.Add(tickable);
            _countTicks++;
        }

        if (updateble is ITickFixed tickableFixed)
        {
            ticksFixed.Add(tickableFixed);
            _countTicksFixed++;
        }

        if (updateble is ITickLate tickableLate)
        {
            ticksLate.Add(tickableLate);
            _countTicksLate++;
        }
    }

    public void Remove(object updateble)
    {
        if (ticks.Remove(updateble as ITick))
        {
            _countTicks--;
        }

        if (ticksFixed.Remove(updateble as ITickFixed))
        {
            _countTicksFixed--;
        }

        if (ticksLate.Remove(updateble as ITickLate))
        {
            _countTicksLate--;
        }
    }

    public void Update()
    {
        if (Core.ApplicationIsQuitting) return;

        //layer.Time.Tick();
        //var delta = layer.Time.deltaTime;

        for (var i = 0; i < _countTicks; i++)
        {
            ticks[i].Tick(); //delta
        }

        for (var i = 0; i < _countTicksProc; i++)
        {
            ticksProc[i].Tick(); //delta
        }
    }

    public void FixedUpdate()
    {
        if (Core.ApplicationIsQuitting) return;
        //var delta = layer.Time.deltaTimeFixed;

        for (var i = 0; i < _countTicksFixed; i++)
        {
            ticksFixed[i].TickFixed();
        }

        for (var i = 0; i < _countTicksProcFixed; i++)
        {
            ticksFixedProc[i].TickFixed();
        }
    }

    public void LateUpdate()
    {
        if (Core.ApplicationIsQuitting) return;
        //var delta = layer.Time.deltaTime;

        for (var i = 0; i < _countTicksLate; i++)
        {
            ticksLate[i].TickLate();
        }

        for (var i = 0; i < _countTicksProcLate; i++)
        {
            ticksLateProc[i].TickLate();
        }
    }

    public void Dispose()
    {
        _countTicks = 0;
        _countTicksLate = 0;
        _countTicksFixed = 0;
        _countTicksProc = 0;
        _countTicksProcFixed = 0;
        _countTicksProcLate = 0;

        ticks.Clear();
        ticksFixed.Clear();
        ticksLate.Clear();
        ticksProc.Clear();
        ticksFixedProc.Clear();
        ticksLateProc.Clear();
    }
}
