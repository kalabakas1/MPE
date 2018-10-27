# Dealing with slow data-providers
I have never said that I were good at making titles, and this one is no different - it is not sexy at all. But never the less I think it could be interesting to write about the experiences I had when I developed a small part of a self service portal, that included integration towards a slow data provider.

## Summary
So basically we had a single page application that had to display some data that we had to get from the data-provider. The issue here were that the data provider had response times up to six seconds to start with - this got optimized over time. As all knows, your response time is bound to the response time from your data provider. If you can't get it faster than six seconds, that is how fast your frontend can receive their data for display.

## The task
We had a user flow where they had to login to a SSO site that then validated and redirect the user back to the self service portal. The page they were presented to were a dashboard displaying your products, latest invoice, and manuals for the products they had - each in a small widget on the dashboard.

Somewhere in that flow we had to gather data that took about six seconds total to acquire from the data provider. And on top of that the main architect had told the customer that we had a performance indicator stating that we could load all data within two seconds per component.  

Basically we had to do some sort of fictive budget for time consumed after the user clicks the login button on the SSO site, each stage can max take two seconds.

1. Validation of user (500ms)
2. Redirect to portal (1000ms)
3. Load each widget on the dashboard (2000ms)

Basically it would take roughly 3500ms before we were over the performance indicator limit - at least that was our current budget.

Next to that we had a other issue in the form of the customers key performance indicator that statet we should be able to load a 1000 users data within 10 minutes.

## The provider
We had a bit of data to acquire before the user could get a proper experience of the dashboard. It had to get the following data:

* User data (firstname, lastname, email etc.) (70ms)
* Main product data (700ms)
  * Sub product data for each of the main products (1800ms)
* Invoice data (620ms)

The ms next to each of the data is roughly the average response time for each of the calls to the data-provider. The issue here were that a big part of the users would have multiple main products, that meaning multiple calls to the sub product endpoint. 

But lets start by looking at the math here for a user having e.g. two main products:
* 70ms for user data
* 700ms Main products
* 1.800ms for each of the two main products
* 620ms for the invoice data

A total on __4.990ms__

Comparing those two calculations we could conclude that this were not gonna be possible and still being able to be within the performance indicator.

So what could we do.

## The naive approach
In school we were always told to do the most simple implementation as possible to start with - it might be the best one anyway. So that was what I did. 

When the user clicks the login button and get validated it would result in a item being pushed to a queue. The worker would pick up that item and start to acquire the data from the data-provider.

I started to implement the worker by doing a synchronous implementation, just getting the data needed one after each other. I knew that it would result in a total of 3.990ms per user before the data were loaded and locally stored. But at least I would have a base for an implementation and tweaking it later on. 

## The better approach
The main architecture including the queue were still the center of the approach. But this time I had a look at what actually could be done in parallel. So for a starters I needed the users data before actually being able to start acquiring other data. For what if the user didn't exist in the data-providers store? Then there wasn't a reason to try to get all the other data, and I would just be wasting bandwidth and time that I didn't have.

So the "algorithm" would look like this:
1. Get the users data
2. Create threads for each of the other data sources needed. e.g. products and invoices (and communications)
3. Start the threads in parallel and wait for them to complete

So you can primitively conclude that the overall time used for one user would be the time used to get the users main data along with the slowest of the parallel executed threads ergo:

70ms + 700ms + 1.800ms = __2.570ms__ (you need the 700ms to get the main products to be able to get the sub-products). But hey, we made it about 2.420ms faster, in theory. And keep in mind this is only the time used on getting the data, the processing time, event though it is not that much is not considered a part of this calculation.

## The limitations
At the current point in time we could get it to run faster because of the limitation in form of the data-providers response time. Now we ran what we could in parallel if possible to get as much external work done at the same time. All these calculations are based on tests done towards the data-providers test environment, that is a shared environment with the possibility of being slow if other customers were using it. But it were the only possibility we had. 

The future includes a new test against their dedicated production environment that might include a proxy and multiple dedicated servers to just handle our requests. This approach might enhance our response time quite a bit. But it is speculations at the current point.

## The forgotten part
So now we actually had a data flow pattern that retrieves the needed data as fast at it were possible based on the current data providers server architecture. But what about the frontend? How did that fit into the current data flow? Well. Lets say that our local storage had expired your data because you had been inactive for too long. The frontend would do its thing as always, by calling a API to gets it data. It might be the Customer endpoint that should return the main data for the current user. 

If it didn't have the data in the storage, it would generate a queue item that the worker would pick up and act on. While the worker did its data-retrieval thing it would ask the storage for the generate item for each 500ms for a total of 20 times before it times out and return a "Not Found" message to the frontend.

At this time the user had waited for ten seconds to just get a "We don't have your data... Fool" message thrown back in the face. The user had just waisted ten seconds that they never get back. This could result in the user loosing its confidence to the system and wont visit the site again. 

So lets try a second time to get this right. 

At this time I changed the worker a bit. If it at any point in time could not find the data and store it in our temporary storage it should instead create a entry that defines that the data wasn't there. That way the API could ask for the data, if the data wasn't there it could look for the error record. If that were present it could terminates its retry pattern and return a not found message to the frontend within one second. 

That way the user would get a fast failure message and carry on with whatever they needed to do on the site.

## Conclusion
So what have I learned in this process? Well, as always, do the most simple integration as possible to get started. From there on you can enhance your design to optimize your overall performance. The important thing is also to understand who is gonna be using you integration. If they are user faced you might want to consider how you are gonna let them know if things fail fast. That way you enhance the user experience and lets the user think the site is acting fast and not waisting time.