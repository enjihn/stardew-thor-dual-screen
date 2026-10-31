#nullable enable
using System;
using System.Collections.Generic;

namespace DualScreen;

/// <summary>A quantity picker belongs to the menu and exact item selected, never just its slot.</summary>
internal readonly record struct ActionSelection(object? Menu, object? Item)
{
    public bool TryQuantity(object? menu, object? item, int requested, int available, out int quantity)
    {
        quantity = 0;
        if (Item == null || !ReferenceEquals(Menu, menu) || !ReferenceEquals(Item, item)
            || requested <= 0 || available <= 0) return false;
        quantity = Math.Min(requested, available);
        return true;
    }
}

/// <summary>Chest slot order can change while the menu and player inventory stay unchanged.</summary>
internal sealed class SlotIdentityRevision
{
    readonly List<object?> previous = new();
    int revision;
    public int Observe(IEnumerable<object?> slots)
    {
        bool changed = false;
        int index = 0;
        foreach (var item in slots)
        {
            if (index == previous.Count) { previous.Add(item); changed = true; }
            else if (!ReferenceEquals(previous[index], item)) { previous[index] = item; changed = true; }
            index++;
        }
        if (index < previous.Count) { previous.RemoveRange(index, previous.Count - index); changed = true; }
        if (changed) revision = unchecked(revision + 1);
        return revision;
    }
}

internal enum TouchAction { Down, Move, Up, Cancel }
internal readonly record struct CompanionScope(long Generation, object? Menu, int Layout);
internal readonly record struct CompanionTouch(CompanionScope Scope, TouchAction Action, int X, int Y);

/// <summary>Game publishes context; UI tags input with the scope of the displayed image.</summary>
internal sealed class CompanionState : IDisposable
{
    readonly object sync = new();
    readonly Queue<CompanionTouch> touches = new();
    CompanionScope current;
    bool disposed;
    public CompanionScope SetContext(object? menu, int layout)
    {
        lock (sync)
        {
            if (!ReferenceEquals(current.Menu, menu) || current.Layout != layout)
            {
                current = new(current.Generation + 1, menu, layout);
                touches.Clear();
            }
            return current;
        }
    }
    bool Matches(CompanionScope scope) => !disposed && scope.Generation == current.Generation
        && ReferenceEquals(scope.Menu, current.Menu) && scope.Layout == current.Layout;
    public bool IsCurrent(CompanionScope scope) { lock (sync) return Matches(scope); }
    public void Enqueue(CompanionTouch touch)
    {
        lock (sync)
        {
            if (!Matches(touch.Scope)) return;
            // A stalled game must not later replay an arbitrary backlog of actions.
            if (touches.Count >= 64) { Invalidate(); return; }
            touches.Enqueue(touch);
        }
    }
    public bool TryTakeTouch(out CompanionTouch touch)
    {
        lock (sync)
        {
            while (touches.TryDequeue(out touch)) if (Matches(touch.Scope)) return true;
            touch = default;
            return false;
        }
    }
    public void Invalidate()
    {
        lock (sync) { current = current with { Generation = current.Generation + 1 }; touches.Clear(); }
    }
    public void Dispose() { lock (sync) { disposed = true; Invalidate(); } }
}

/// <summary>Same drawable rectangle used to position the image and map its local touches.</summary>
internal readonly record struct DrawableTransform(double Left, double Top, double Width, double Height,
    int LogicalWidth, int LogicalHeight)
{
    public bool TryMap(double px, double py, out int x, out int y)
    {
        x = y = 0;
        if (Width <= 0 || Height <= 0 || !double.IsFinite(px) || !double.IsFinite(py)
            || px < Left || py < Top || px >= Left + Width || py >= Top + Height) return false;
        x = (int)((px - Left) * LogicalWidth / Width);
        y = (int)((py - Top) * LogicalHeight / Height);
        return true;
    }
    public TouchAction MapAction(TouchAction action, double x, double y) =>
        action == TouchAction.Cancel || !TryMap(x, y, out _, out _) ? TouchAction.Cancel : action;
}

/// <summary>Three fixed buffers: one producer, one newest pending, one UI transfer.</summary>
internal sealed class LatestFrameQueue : IDisposable
{
    internal sealed class Frame
    {
        public readonly byte[] Bytes;
        public long Generation { get; internal set; }
        public CompanionScope Scope { get; set; }
        internal long Epoch;
        internal int State; // 0 free, 1 writing, 2 pending, 3 transferring
        internal Frame(int bytes) => Bytes = new byte[bytes];
    }
    readonly object sync = new();
    readonly Frame[] frames;
    Frame? pending, active;
    long epoch;
    bool disposed, callbackQueued;
    public LatestFrameQueue(int bytes) => frames = new[] { new Frame(bytes), new Frame(bytes), new Frame(bytes) };
    public int PendingCount { get { lock (sync) return pending == null ? 0 : 1; } }
    public Frame? AcquireWriter()
    {
        lock (sync)
        {
            if (disposed) return null;
            foreach (var frame in frames)
                if (frame.State == 0) { frame.State = 1; frame.Epoch = epoch; return frame; }
            // The next produced frame supersedes the unconsumed one, never the UI transfer.
            if (pending is { } replace)
            { pending = null; replace.State = 1; replace.Epoch = epoch; return replace; }
            return null;
        }
    }
    public bool Publish(Frame frame, long generation)
    {
        lock (sync)
        {
            if (frame.State != 1) throw new InvalidOperationException("Frame is not owned by the producer.");
            if (disposed || frame.Epoch != epoch) { frame.State = 0; return false; }
            if (pending != null) pending.State = 0;
            frame.Generation = generation;
            frame.State = 2;
            pending = frame;
            return Schedule();
        }
    }
    bool Schedule()
    {
        if (disposed || pending == null || active != null || callbackQueued) return false;
        callbackQueued = true;
        return true;
    }
    public Frame? TakeLatest()
    {
        lock (sync)
        {
            if (active != null) return null;
            callbackQueued = false;
            if (disposed || pending == null) return null;
            active = pending; pending = null; active.State = 3;
            return active;
        }
    }
    public bool Complete(Frame frame)
    {
        lock (sync)
        {
            if (!ReferenceEquals(active, frame)) throw new InvalidOperationException("Frame is not the active transfer.");
            active = null; frame.State = 0;
            return Schedule();
        }
    }
    public void Abandon(Frame frame)
    {
        lock (sync)
        {
            if (frame.State != 1) throw new InvalidOperationException("Only a producer can abandon its frame.");
            frame.State = 0;
        }
    }
    public void Invalidate()
    {
        lock (sync)
        {
            epoch++;
            if (pending != null) pending.State = 0;
            pending = null;
        }
    }
    public void Dispose() { lock (sync) { disposed = true; Invalidate(); } }
}
