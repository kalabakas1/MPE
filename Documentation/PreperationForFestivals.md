# Preparation for festivals
Spoiler alert, this is not a text regarding how much beer and how many bags of chips you should buy if you are going to a music festival. This is more a collection of observations and stories regarding the technical preparations that happens to make a musical festival a reality.

## Motivation
So why do I write this? Actually, to be honest, at the current moment I have been drinking a bit too much coffee (provided by my project manager - self roasted), and I can't really sleep. But it have been something I really have been wanting to write. I think that some of these small experiences can be used in the future to avoid having to refactore, and perhaps avoid having to work odd hours to make your deadlines. Along with that I believe that it's a important aspect of the live of a developer to actually be allowed to write his perspective of a situation. 

## Setting the stage
I have been doing a bit of different software development that range from small to rather large projects. All exposed by some sort of web based frontend that. But every year at summer time my professional time have always ended up being used on preparing and perfecting systems for festivals.

There are a lot of different aspects in that venue of software-systems. The ones I had to support were rather limited and included:
* Volunteer data management system
* Guest and performer data registry
* Support platform exposing all possible data that could be used to solve problems.
* Distribution of electronic products to volunteers, guests and performers
* Integration to physical gates
* Internal monitoring of data providers state of service and performance
* Automated beverage system integration

So to make things clear, my role was not big, the systems weren't large like Amazone and Google - but they needed to be online and responding 24/7.

## Getting the ducks in a row
As I stated I am actually only employed as any other developer, and that suits me pretty well. But one of my downsides are that I can take any role that is needed to make the process work. This were sort of needed this year. The reason were that the company I worked for had added a lot of new people to the team I were a part of, both project-managers and developers. The issue lie in the responsibilities of the new project-manager that were added to the project. He hadn't experienced the flow of a season with the festivals, and at that time were already overworked with other customers.

So someone with the experience had to partly be the project-manager - and surprise, the only one that could do that were me. But I didn't know anything about project-management besides my common sense. 

But common sense tends to be enough most of the time. So what I did was to make a plan, like any other bank-robber. Find out the different deliveries from the different data providers, make the timeline and make sure that people stick with it - and make sure that you know who those people are. Make a contact table containing all the needed data so you know who the players are.

By doing that I now had a perfect idea of how and when the systems were ready for production. Who delivers the next thing to act on, and who we should contact if the deadline were passed without delivery of artifacts.

Along with that, strict meetings made sure that all were on the same page. So in the case you had multiple providers of different services that might interact, make sure to have regular meetings with all of them at once. Make it possible to share issues and broken deadlines before it is to late. Make sure the right people talk together.

__Advice:__ Make a plan, stick to it, request statuses at broken deadlines, and make sure that you communicate that it is all for the greater good.

## Distribution tool reworked
So one of this years fun tasks were to integrate the systems to use two new service providers. One for distribution of electronic giftcards and vouchers for guests and volunteers, along with another one that were a provider responsible for access point hardware, so when a guest had to enter the festival area he had to be scanned in a NFC scanner. That way the organization could have an idea of how many actually were on location at a given time.

In my world it's a fun task to do as a greenfield project. No code to be limited by, and nothing to hinder to use the newest software and frameworks. But thats not the way this story goes. This system had a lot of logic regarding e.g. the distribution of vouchers, and usage of giftcards. So we couldn't use the newest frameworks, and had to extend the existing code.

The part about extending existing code can be some what taunting and difficult. But in this case it seemed to be pretty easy. By using old patterns where you program against interfaces and encapsulates your code, I were actually able to exchange the existing code by some code using the same interfaces but integrating to the new service provider. It were so easy and straight forward that it seemed a bit too good to be true.

One of the reasons this were being a straight forward case actually lie in the preparations made before the actual implementation. When you need to change a service provider, you need to know what the existing one does. The old one exposed services both in the form of a SOAP service, but also through REST. So a good mix, hidden behind defined interfaces. But how best to actually define the criteria for a service providers exposed services? I know that people normally just do a requirement description in a word document, but it doesn't seem to be the preferred way to communicate with developers. 

So what do we always not come around doing, and what are always missing? Well normally for most projects, it is documentations. And when it comes to REST APIs, documentation is sort of needed to understand what it does and how it works. So by creating some auto-generated documentation using a mock implementation of the REST API, I could generate a swagger file that made the foundation of communication between me and the service providers developer. We both new the standard, and we new what the endpoints did, no mis-communications at all. And on top of that, we already had produced the documentation that normally would be lacking detail.

__Advice:__ Is it external or have a chance of being modified or replaced, please encapsulate it and make a interface you can use in the future.

__Advice:__ Communicate REST through a standard format e.g. Swagger.

## Support tool reworked
One of the central tools at the festival were the supporting platform. It were the one way into all data needed to solve any situation that a guest could have. And the questions were vivid:

* I have payed my subscription but check-in says otherwise, why?
* I have bought a wristband/ticket from a random dude at the corner - but check-in says it is invalid?
* I haven't got all my vouchers distributed to my wristband, as I were promised?
* All my money is gone, were are they?

And these were just some of the random questions drunk festival guests can come up with. 

The main goal for this tool were to give the support staff a one way visualization of the data so they could give the guest a proper answer or at least create a case for second level support.

### First issue - external providers
The main issue with this tool were the response time. A search for a guest could take 20 seconds or minutes before the response were rendered. It were a test of patience to use it - and when ever you had the list or results, when ever you selected a specific person, you had to wait again. It had to search three systems by calling REST endpoints before it could actually render the result. So see it as a centralized search "engine". But this could not go on. So last year, me and another developer, sat down and analyzed the support tool to get some sort of idea of the "root"-cause. By throwing some tools at the problem we found out that one of the external data providers had a very long response time before we actually got the result. At the time, the data provider could not implement some sort of indexing strategy, because this were during the execution of the event - but we actually found the culprit. Well... one of them.

__Advice:__ Never trust external dependencies. Always debug by looking at the outer-most layer of the application. Not likely to be the kernel that is faulty every time.

### Second issue - flow of execution
But it didn't stop there. A year later I found some time to actually look at the way the code were structured and what happens internally in the support tool. The search logic were pretty straight forward.

1. if volunteer data provider is selected, search in that system and wait for response
2. if guest data provider is selected, search in that system and wait for response
3. if visitors provider is selected, search in that system and wait for response.
4. When all results have been received, merge them, and render it in the frontend.

Fair enough, it's pretty straight forward how to speed this up. Lets just make it run in parallel. Start each of the searches and wait on all of them to finish. The total time spent is equal to the slowest search endpoint. A valid way to "quick"-fix the problem. 

But there is a catch. All the integrations depend on the HTTP context that contains data needed to send to the providers. Some specific session variables value needed to be added to the requests. So not that easy to wrap the search functions in threads, because the lowest layer of code depended on the HTTP context - not cool. 

__Advice:__ Don't make integrations components depend on the web context

__Advice:__ Asyc is sometimes the way to go to make work done multiple places at once.

A bit of work fixed this issue and we were up and running with a new flow of execution that actually gave a big bit of performance. But lets go deeper...

### Third issue - DDOSing yourself
So now the flow were more or less optimized to a better state. But something were still killing the API's. Every time a search were performed, it seemed like one additional request per result-item were executed against the data providers API.

So if the search result were containing 50 items, to render the result the API received 51 requests.

By the way, there were ten computers running this support tool all the time. So if every computer performing a search at the same time, the API should handle 510 requests. And these people were constantly working with the system.

By hooking up the cool Prefix we got an insight into what were actually happening at the execution of a search. To my surprise the results received from one of the providers iterated all of the results and called a method to get the UID of the wristband the person had been assigned. One issue, this method executed a REST request against the API, and it were taking forever. So what to do? Well why not just request all the data at once instead of separate call for each item? That sort of fixed the problem. Expose API endpoint taking a list of IDs, instead of just one.

The initial issue were actually recorded on a graph representing the amount of API calls per minute. After the fix, compared with before, you couldn't see any sort of activity coming from the support tool. Small things, little time spent, a huge difference. 

Total response time for a search came down to max three seconds from at least ten.

__Advice:__ It might be clear, but don't DDOS your own applications.

__Advice:__ Don't call REST in a loop if you can avoid it - limit total calls if possible.

## Monitor, monitor, monitor
Over the last years I have been doing this job having a very little insight into the health of the different systems. It weren't possible to spend a lot of money to buy the state of the art metrics monitoring systems to get a total insight - and what I have learn, that is not the important part. The last couple of years, I spent some time making a small library that can give me that insight. It can give me the insight into the servers metrics, how the RAM, CPU and network is performing - and a lot of other interesting informations. But that is not what is the most important part of monitoring. So what is it that we should monitor? The answer is straight forward. We should concern ourself with the metrics that is important to the organization. In this case, we need to know what is important to the volunteers that drive the festivals. This target-group might concern them-selfs about:

* Is my wristband activated in the gate?
* Do I got the electronic giftcards and vouchers on my wristbands?
* Can I communicate with my team-leader in the messaging system?
* Can I check-in with my checkin-card?

All these questions is centralized around the external service providers we integrate to. So therefore we need to monitor those integrations: Do they respond in time, are there any errors happening, what status do they return etc.? But also, do our check-in sub-system respond in time, are there delays that make the physical check-in flow halter?

These "metrics" are the ones we need to monitor. Those are the things that effect the people building the festival, if these are out of order, a physical line of people will start building up, resulting in bad moods and perhaps even a PR issue.

If we can monitor these metrics, and act fast if anything is throwing errors, then we have a living chance of mitigating the issue before it manifests physically.

Remember, I am not stating that CPU and RAM etc. are not important, but as I read some were:

> If your system performs as normal, why get alerted if the CPU or RAM spikes? If it does not effect the users perception of performance, why wake your self up in the middle of the night for that?

The one thing I learned, though, to monitor are the disk space left on the storage. This is the one server metric I really do monitor and gets alerts on. In the execution of a festival, a lot of files gets generated and will take up space. If the storage runs dry, you might risk that your entire system goes down.

So, one of the really popular things to talk about these days seem to be the duty of on-call developers, dev-ops, SRE and what ever title we tend to come up with. These systems are not big or complex, but they need to be up and running 24/7 for about about two months. The first couple of years we just accepted the call from the organization with an issue where the basic solution were to restart some part of the system. But I really like my beauty sleep, and really don't want to be woken by that horrible ringtone from the on-call phone. The thing I implemented were some basic healt-checks in the system that gets executed every 30 seconds. If they failed, they would test again. Fail three times, and it could be configured to execute a script that could heal the system to a stable state. If it still failed the test, then the text message would be sent. This would also make sure we don't get alerted because of a nightly deploy, unless it actually broke the system totally. This would make sure we only acted on important stuff, not stuff that could be solved by automation.

__Advice:__ Don't trust the users of your own API's - they may break you entire platform if not controlled.

__Advice:__ Know your organizations important performance/satisfactory indicators, and monitor them.

__Advice:__ Communicate when stuff breaks, and stick with it.

__Advice:__ Don't get alerted unless the world can't fix itself

## On-boarding is a bitch
I were once told that if a software system passed the age of five in production it would be considered as being old. So at this moment we have concluded that these systemes should be considered as being old. I guess they have six years experience in production, and have been an idea two years prior of production.

The system started as a prof of concept to make a working example of a CMS running with MVC as a frontend. It seem to be a pretty straight forward project, the issue is that it have never been cleaned up, undergone code-review, have no unit-tests, and documentation of code or processes are non-existing. It have spanned 30 solutions containing three coding projects in each, integrated over time into one solution - so when everything builds it should work. 

How would you ever sell that in a job proposal?

>Open position for developer formerly educated as detective, working best knowing that when ever you fix an issue, you might have introduced three others - you just don't know where.

As mentioned before the team I were a part of had just hired a lot of new developers, so it would be easy to just tell them to work on that/those project(s). Right? I have not yet solved this issue. But I think I got a list of different things I think would be important for the developer to have:

* Curiosity:
  * for understanding how things work
  * for understanding how the festivals work, and what drives volunteers
* Working independently and are self-driven
* Have a high level of professionalism
* Service minded
* Cares for his/hers private-life
* Can talk to people over coffee or a cigarette - get to know the volunteers

We might have that person on the team, but how would you on-board him onto the project? Would you give him a introduction to the overall system first, and then a more deep introduction to the central sub-systems? Would you start giving him small bug-fix cases so he get a feel of different parts of the system?

But by asking these questions I think I know what to do first. The main thing the new guy should be introduced to is the central workings of a festival. He have to be introduced to an actual physical festival. He need to see what the systems we support actually do, and what value they give our customers. He have to know what happens when a system goes down. He need to see what the physical implications are if the check-in sub-system goes offline - how the lines of people just gets longer, how people gets more irritated. 

This will first scare him, but we need to show what it is all about. It is about people and value, which both are scary to deal with. It is important to give him an impression of how important his job actually are to a lot of people once every year to the annual music festival. A few days were your software systems need to run without issues.

By giving him this insight, without looking at code, you can see if he is the person. If he is the developer who get scared at a stressful time, or he will help make the problems go away with all in his power. It is important to show him the goal, and the people that we effect with our work - even though it is not even close to rocket-science.

If it seems like he is the latter developer, he can learn any programming language needed without a problem, just because he knows the overall goal.

__Advice:__ Make sure people knows the goal, have an idea of the effect of the systems, knows what happens if things doesn't work 100%.

# Conclusion
A lot of all these advices given in the text could come from any known famous developer, architect, speaker or software-advocate. But the important thing is that they are true, and proven in the field. I haven't come up with the advices, but I just make the prof that they actually seem to be worth focusing on.

Well, thats the technical aspect of this little text. I think that the software industry need to focus on the soft sides of the cold products we produce. Make sure to get the developers to work for the customers values, and not just to solve cases without having an understanding of what the overall goal of the systems are. You get your developers to understand and care for the customers goals, you will get the best product and most dedicated on-call developers. But to do this you need to invest some time, so you get your developers on-boarded in a orderly fashion. You need to show them what it is all about, and need to introduce them to the customer without them having to produce dead and cold code that goes into production.