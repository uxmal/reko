## Reko GUI event bus

###Problem statement
Given:
* a user interface with controls running on the UI thread. 
* an engine running on a separate thread.

The user interface is interested in receiving notifications from the engine, but these notifications must 
arrive on the UI thread. Therefore there needs to be an intermediary which receives the events -- from any thread
-- and then dispatches them to the UI thread using a Post-to-queue mechanism.
This allows the engine to fire and forget events.

To make this work, the UI thread calls methods on the IEventBusService to register interest in events from the engine.
A registration for such an event could look like this:

```
var busSvc = services.GetService<IEventBusService>();
var subscription = busSvc.Subscribe<Eventargs>(
    decompiler.CompilationComplete,
    this.decompiler_Complete,
    EventStyle.Automatic);
```
where the first argument is the event in the free-threaded engine we wish to listen to,
the second argument is the event handler, which always must run on the UI thread
and the final argument specifies how to handle throttling of multiple events.

The simplest scenario is when events happen rarely. Here we want them to behave just 
like they do in synchronous, single thread programs.
```
void decompiler_Complete(object sender, EventArgs e) {
    // We don't expect these to queue up, so we use EventStyle.Automatic.
}
```
However, in some cases the events are arriving at such a speed that the post-to-queue will fill up with events.
If these events carry with them no data, they really are quite redundant
In these cases, the UI will want to tell the event bus: "I'll be busy handling that event you sent me,
so don't send me any more events until I tell you to. If an event does happen while I'm busy, remember it and
call me back later". This allows the UI thread to dispatch other messages in its event queue and avoid the UI
thread locking up:
```
this.subscription = busSvc.Subscribe<Eventargs>(
    imageMap.ItemAdded,
    this.imageMap_ItemAdded,
    EventStyle.ManualRestart);

void imageMap_ItemAdded(object sender, EventArgs e)
{
    // We won't get any more events on this subscription until we tell it to resume.
    // do something slow.

    subscription.Restart(); // start sending events again, but not until the UI message thread has had a chance.
}
```
In other cases, we care about the data passed with events, but the UI thread is too slow to handle them.
In that case, the subscription could collect the events and queue them up. The subscriber can then collect
the queued events at once and consume them: 
```
this.subscription = busSvc.Subscribe<ProcedureEventArgs>(
    this.program.ProcedureAdded,
    this.program_ProcedureAdded,
    EventStyle.EnqueueEvents);

void program_ProcedureAdded(object sender, ProcedureEventArgs e)
{
    // if 5 ProcedureAdded events were received, program_ProcedureAdded will be called
    // 5 times in quick succession.
    DoSomething(e.Procedure);
}
```
However, that last protocol may suffer from race conditions. Perhaps it is better for the receiver to do the queueing itself:
```
void program_ProcedureAdded(object sender, ProcedureEventArgs e)
{
    this.myqueuedprocedures.Add(e.Procedure);
    SynchronizationContext.Current.Post(()=> { DoStuffWith(myqueuedprocedures)});
}
```
