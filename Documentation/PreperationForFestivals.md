# Preparation for festivals
Spoiler alert, this is not a text regarding how much beer and how many bags of chips you should buy if you are going to a music festival. This is more a collection of observations and stories regarding the technical preparations that happends to make a musical festival a reality.

## Motivation
So why do I write this? Actually, to be honest, at the current moment I have been drinking a bit too much coffee (provided by my project manager - self rosted), and I can't really sleep. Byt it have been something I really have been wanting to write. I think that some of these small exeperiences can be used in the future to avoid having to refactore, and perhaps avoid having to work odd hours to make your deadlines. Along with that I belive that it's a important aspect of the live of a developer to actually be allowed to write his perspective of a situation. 

## Setting the stage
I have been doing a bit of different software development that range from small to rather large projects. All exposed by some sort of web based frontend that. But every year at summer time my professional time have always ended up being used on preparing and perfecting systems for festivals.

There are a lot of different aspects in that venue of softwaresystems. The ones I had to support were rather limited and included:
* Volunteer data management system
* Guest and performer data registry
* Support platform exposing all possible data that could be used to solve problems.
* Distribution of electronic products to volunteers, guests and performers
* Integration to physical gates
* Internal monitoring of data providers state of service and performance
* Automated beverage system integration

So to make things clear, my role was not big, the systems were'nt large like Amazone and Google - but they needed to be online and responding 24/7.

## Getting the ducks in a row
As I stated I am actually only employeed as any other developer, and that suits me pretty well. But one of my downsides are that I can take any role that is needed to make the process work. This were sort of needed this year. The reason were that the company I worked for had added a lot of new people to the team I were a part of, both projectmanagers and developers. The issue lie in the responsibilities of the new projectmanager that were added to the project. He had'nt experienced the flow of a season with the festivals, and at that time were already overworked with other customers.

So someone with the experience had to partly be the projectmanager - and surprise, the only one that could do that were me. But I did'nt know anything about projectmanagement besides my common sense. 

But common sense tends to be enough most of the time. So what I did was to make a plan, like any other bank-robber. Find out the different deliveries from the different data providers, make the timeline and make sure that people stick with it - and make sure that you know who those people are. Make a contact table containing all the needed data so you know who the players are.

By doing that I now had a perfect idea of how and when the systems were ready for production. Who delivers the next thing to act on, and who should we contact if the deadline were passed.

Along with that, strict meetings made sure that all were on the same page. So in the case you had multiple providers of different services that might interact, make sure to have regular meetings with all of them at once. Make it possible to share issues and broken deadlines before it is to late. Make sure the right people talk together.

__Advice:__ Make a plan, stick to it, and request statuses at broken deadlines, and make sure that you communicate that it is all for the greater good.

## Distribution tool reworked

__Advice:__ Communicate REST through a standard format e.g. Swagger.

## Support tool reworked
One of the central tools at the festival were the supporting platform. It were the one way into all data needed to solve any situation that a guest could have. And the questions were vivid:

* I have payed my subscription but checkin says otherwise, why?
* I have bouhght a wristband/ticket from a random dude at the corner - but checkin says it is invalid?
* I havn't got all my vouchers distributed to my wristband, as I were promised?
* All my money is gone, were are they?

And these were just some of the random questions drunk festival guests can come up with. 

The main goal for this tool were to give the support staf a one way visualization of the data so they could give the guest a proper answer or at least create a case for second level support.

### First issue - external providers
The main issue with this tool were the response time. A search for a guest could take 20 seconds or minutes before the response were rendered. It were a test of pation to use it - and when ever you had the list or results, when ever you selected a specific person, you had to wait again. It had to search three systems by calling REST endpoints before it could actually render the result. So see it as a centralized search "engine". But this could not go on. So last year, me and another developer, sat down and analyzed the support tool to get some sort of idea of the "root"-cause. By throwing some tools at the problem we found out that one of the external data providers had a very long response time before we actually got the result. At the time, the data provider could not implement some sort of indexing strategy, because this were during the execution of the event - but we actually found the culprit. Well... one of them.

__Advice:__ Never trust external dependencies. Always debug by looking at the outer-most layer of the application. Not likely to be the kernal that is faulty every time.

### Second issue - flow of execution
But it didn't stop there. A year later I found some time to actually look at the way the code were structured and what happend internally in the support tool. The search logic were pretty straight forward. 

1. if volunteer data provider is selected, search in that system and wait for response
2. if guest data provider is selected, search in that system and wait for resonse
3. if visitors provider is selected, search in that system and wait for response.
4. When all results have been recieved, merge them, and render it in the frontend.

Fair enough, it's pretty straight forward how to speed this up. Lets just make it run in parallel. Start each of the searches and wait on all of them to finish. The total time spent is equal the slowes search endpoint. A valid way to "quick"-fix the problem. 

But there is a catch. All the integrations depend on the HTTP context that contains data needed to send to the providers. Some specific session variables value needed to be added to the requests. So not that easy to wrap the search functions in threads, because the lowest layer of code depended on the HTTP context - not cool. 

__Advice:__ Don't make integrations components dependt on the web context

__Advice:__ Asyc is sometimes the way to go to make work done multiple places at once.

A bit of work fixed this issue and we were up and running with a new flow of execution that actually gave a big bit of performance. But lets go deeper...

### Third issue - DDOSing yourself
So now the flow were more or less optimized to a better state. But something were still killing the API's. Every time a search were performed, it seemed like one aditional request per result-item were executed against the data providers API.

So if the search result were containing 50 items, to render the result the API recieved 51 requests.

By the way, there were ten computers running this support tool all the time. So if every computer performing a search at the same time, the API should handle 510 requests. And these people were constantly working with the system.

By hooking up the cool Prefix we got an insight into what were actually happening at the execution of a search. To my surprise the results received from one of the providers iterated all of the results and called a method to get the UID of the wristband the person had been assigned. One issue, this method executed a REST request against the API, and it were taking forever. So what to do? Well why not just request all the data at once instead of seperate call for each item? That sort of fixed the problem. Expose API endpoint taking a list of IDs, instead of just one.

The initial issue were actually recorded on a graph representing the amount of API calls per minut. After the fix, compared with before, you could'nt see any sort of activity comming from the support tool. Small things, little time spent, a hurge difference. 

Total response time for a search came down to max three seconds from at least ten.

__Advice:__ It might be clear, but dont DDOS your own applications.

__Advice:__ Don't call REST in a loop if you can avoid it - limit total calls if possible.

## Monitor, monitor, monitor

__Advice:__ Don't trust the users of your own API's - they may break you entire platform if not controlled.

__Advice:__ Know your organizations importent performance/satisfactory indicators, and monitor them.

__Advice:__ Communicate when stuff breaks, and stick with it.

__Advice:__ Don't get alerted unless the world can't fix itself

## Onboarding is a bitch
I were once told that if a software system passed the age of five in production it would be considered as being old. So at this moment we have concluded that these systemes should be considered as being old. I guess they have six years experience in production, and have been an idea two years prior of production.

The system started as a proff of concept to make a working example of a CMS running with MVC as a frontend. It seem to be a pretty straight forward project, the issue is that it have never been cleaned up, undergone code-review, have no unit-tests, and documentation of code or processes are non-existing. It have spanded 30 solutions containing three coding projects in each, integrated over time into one solution - so when everything builds it should work. 

How would you ever sell that in a job proposal?

>Open position for developer formerly educated as detective, working best knowing that when ever you fix an issue, you might have introduced three others - you just don't know where.

As meantioned before the team I were a part of had just hirred a lot of new developers, so it would be easy to just tell them to work on that/those project(s). Right? I have not yet solved this issue. But I think I got a list of different things I think would be important for the developer to have:

* Curiousity:
  * for understanding how things work
  * for understanding how the festivals work, and what drives volunteers
* Working independently and are selfdriven
* Have a high level of professionalism
* Service minded
* Cares for his/hers privatelife

We might have that person on the team, but how would you onboard him onto the project? Would you give him a introduction to the overall system first, and then a more deep introduction to the central sub-systems? Whould you start giving him small bug-fix cases so he get a feel of different parts of the system?

But by asking these questions I think I know what to do first. The main thing the new guy should be introduced to is the central workings of a festival. He have to be introduced to an actual physical festival. He need to see what the systems we support actually do, and what value they give our customers. He have to know what happens when a system goes down. He need to see what the physical implications are if the checkin sub-system goes offline - how the lines of people just gets longer, how people gets more irritated. 

This will first scare him, but we need to show what it is all about. It is about people and value, which both are scary to deal with. It is important to give him an impression of how important his job actually are to a lot of people once every year to the annual music festival. A few days were your software systems need to run without issues.

By giving him this insight, without looking at code, you can see if he is the person. If he is the developer who get scared at a stressfull time, or he will help make the problems go away with all in his power. It is important to show him the goal, and the people that we effect with our work - even though it is not even close to rocket-science.

If it seems like he is the latter developer, he can learn any programming language needed without a problem, just because he knows the overall goal.

__Advice:__ Make sure people knows the goal, have an idea of the effect of the systems, knows what happens if things does'nt work 100%.


# Conclusion