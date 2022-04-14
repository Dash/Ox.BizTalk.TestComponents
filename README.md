# Ox.BizTalk.TestComponents

This library is meant to be a very basic implementation of BizTalk interfaces that can be used with test frameworks for the unit testing of BizTalk components without having to execute code in the BizTalk engine.

It is not guaranteed to behave in the same was as the BizTalk engine (in fact, I gaurantee it won't!), but to aid in stubbing components.

All methods are marked as virtual so you can override bits as you feel fit.

## Building
It is built against .NET 4.7.2 as this covers BizTalk 2016 which is still in support.

## Usage

This code should only be used in unit tests when the BizTalk runtime is unavailable.  It doesn't fully implement all the required bits, and most certainly doesn't do it in a clever way.

A common approach used in these test classes is to allow you to register event handlers on methods which use the MethodCalledEventArgs class, which simply contains the arguments passed into a method, in signature order.  This allows you to check that a method is being called with the expected values.  For example:

```csharp
// Create a task to track when the callback has been completed.
var eventsComplete = new TaskCompletionSource<bool>();

// Setup some unique test data
var testData = Guid.NewGuid().ToString();

// Create an inline function to handle our event and test the result
void ReceiverShuttingDown(object sender, MethodCalledEventArgs e)
{
    // Test that the inserted data matches our input
    Assert.AreEqual(testData, (string)e.Args[0]);

    // Mark the test complete
    eventsComplete.SetResult(true);
}

// Setup the test class
var proxy = new TestTransportProxy();

// Register our event handler
proxy.OnReceiverShuttingDown += ReceiverShuttingDown;

// Submit some test data
proxy.ReceiverShuttingDown(testData, null);

// Wait for the event handler to be called, or fail after a timeout period
if(!eventsComplete.Task.Wait(TEST_TIMEOUT))
{
    Assert.Fail("Tests methods were not called.");
}
```

Naturally this exmaple is rather dim, because it's checking that the data we pass in is the data that is logged, which will always be the case.  Where this approach is useful when we have classes that call these methods as a dependency and we want to test whether that was called correctly.

Interfaces/abstract classes covered:

### Orchestration

* XLANGPart (TestXLANPart)
* XLANGMessage (TestXLANGMessage)

### Pipelines
* IBaseMessagePart (TestMessagePart)
* IBaseMessageFactory (TestMessageFactory)
* IBaseMessageContext (TestMessageContext)
* IBaseMessage (TestMessage)

### Adapters

* IBTTransportBatch (TestTransportBatch)
* IBTTransportConfig (TestTransportConfig)
* IBTTransportProxy (TestTransportProxy)
* IBTBatchCallBack (TestBatchCallBack)

### General
* IResourceTracker (TestResourceTracker)
* IBasePropertyBag (TestProperyBag)
* IPropertyBag (TestPropertyBag)

## Changes
There are amany methods that aren't implemented, you may need to add some functionality.

Please submit a pull request with changes.