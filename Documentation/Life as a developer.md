# Life as a developer
This is a very small article about my life (oh yeah, this is about my life) as a developer during the last couple of years, through different titles I have possessed and projects I have been a part of up till now. But do not be fooled, it will not be a diary but rather a recap on what great things I have experienced that have affected me during that time. The result of this article should be answers to questions like: what is a developer, what can help a developer stay on track, what elements in a company is important for a developer, what should you avoid and what cant you avoid.

## Disclaimer
I will come to write about things that can be opinionated a lot, so keep in mind, this is how I perceive the world as a developer and it will be my own opinion about things. Likewise it will contain subjects that you properly have read elsewhere - but that might confirm the rightness or importance of the subjects. I still have a lot to learn, if given the time - but this is written from my current experience in time. I will be stepping on toes and make a lot of people question my intension for this text, but take a chill pill and don't take it personal.

## Motivation for this crazy text
Like anything else, I have had some motivation for writing this text. The last couple of years I have witnessed the development of a company, from when it wasn't much more than a start-up. It was a 40 man big company with a lot of culture, Friday bar meet-ups, not that much focus on time-tracking, good benefits during the summer, kick-off trips outside of the country and a lot more. It is still a great place to work (don't misunderstand me), but its about four-five times that big at the moment and a lot have changed since then. During that time I have had the chance of being a student employee, a normal developer, something that might be considered a system architect, a software-janitor and a lot of those small, not officially defined, titles that all developers have had. All of this have given me a platform so I'm now able to define the environment I consider to be the right one for a developer - and this is what this small article will be about.

## Responsibility, faith and trust - The beginning
One of the things I remember the best, was the interview I attended to get the job. I basically wasn't educated yet, had only my big interest in development using Java (just another fancy programming language) as my platform. So when I got the question from the CEO (fancy title for boss):

> "We are seeking a good C# (programming language that isn't Java) - Is that gonna be you?" 

The only thing I could think of, that might give me a chance of actually getting the job was:

> "I really cant do anything of what you are asking - but I'm a fast learner"

This was apparently enough for the CEO to actually give me a chance as a student employee, and later on give me a permanent position as a developer. As a young man I was mainly just happy to get a job, but in retrospective this is an example of one of the cornerstones that are important to a developer: faith and trust. Basically the CEO could ditch me before the interview, just by looking at my papers - I had, at that moment, nothing to my name except my word. 

By trusting a developer, you will motivate that person to achieve whatever goal is defined. I know that this was a driving force for me to learn this new programming language. I did it because I did not want to disappoint the two persons in that interview room - because they depended on me (well not really but that was what I told my self at that time to be able to make the hours).

During the period of my student employment I did a lot of different tasks. That might be maintenance tasks on existing systems, small new development in our internal time tracking system to changing colors on buttons on websites. All of these small, seamlessly, meaningless tasks was something I expected when being a student employee. They wasn't big or advanced, but I was doing a bit of development. But all of the sudden, the director of tech entered our little student-office (well, it was a room in the basement, under the basement next to the seasonly over-flooded server-room) and gave us a new task. We had to overtake a relative complex project regarding merging different data-sources and making reasonable relations across of these. First of all, WTF (slang for: are you sure about that). Secondly, I was overwhelmed of the trust he bestowed upon us. We could basically just F everything up and crash the project, or we could make it work - I guess he knew what he wanted to achieve, but he really didn't know anything about the outcome. 

> A leader trusts the employee, guides him and delegate the reponsibility, even though the future is bleak and dosn't have a clear outcome

At that point in time this was the biggest project in our little student department, and it was a great feeling to be a part of it. The motivation was high because my closest boss actually trusted that we would succeed - at least he faked it. He didn't micromanaged the process but only defined the goal at hand in the form of use-cases and guided in the use of different technologies. It was truly a great time in my employment! The affect of being trusted by an authority is not something you should ignore.

Event though the project started to work out fine, it later on got ditched because of lacking resources. In this case the lacking resource was me, because it was more profitable to make me work on a customer project as a permanent developer - but lets get going.

## The darkness - The light
When I got my position as a developer, I was handed a permanent project to manage. It was a system to manage different resources when the users should plan a event of somewhat a larger scale. The hand-off from the former developer could be described as a Proof of Concept (POC) project that combined two large technologies - and as we all know, POC-projects are never in a neat state. It contained a lot of bad architecture, a lot of out-commented code, code never being used, and code not making sense. It was sort of chaos. This would be my every day for the next four years. 

This project would be a great example on a lot of things - and we will not cover them all in this small text. First of all, lets take a look at the concept of technical debt. For those of you who haven't heard of the concept, it's basically if you F up in the middle of the process by creating a hack, and you don't fix it at that time, it might be expensive when you start the next step in the development plan. So basically, make sure to pay your dept as fast as possible - that way, it won't get a pain to pay later. 

So why mention technical debt? Well, who will pay for this endeavor? The ideology of my workplace is that, if the customer won't pay it will not get done (it is the standard answer anyway with exceptions). My take at this approach is a bit different. I see this as a safe way to kill your customer and slowly stop the development speed of the project. If we are not allowed to fix our mistakes when we find them, because of the financial part, then we will just end up with systems containing a lot of errors, hacks, duck-tape, black holes and other magical stuff. That making it impossible to implement new features, because you have no idea of the ramifications on the existing system. We had a great example of a project manager that came to me and saying:

> Why is it, that everytime we implement a new thing, five other things we havn't touched breaks? Why does everything break when deploying the core or a plugin? Why... Why... Why...!

And I get it, we have to take our customers money to stay in business. But what if we see it as an investment? What if we handled the technical debt in the moment we discover it? That way we invest in our customers, we are making sure that features are easier to implement, taking less time, and making them easier for the sales staff to actually sell - without lowering the hourly rates or faking less hours that it takes to do. I am not educated in finances, but am educated in the art of logic - and this approach makes sense. A long time investment in a customer could result in a lasting relationship - and perhaps be a great pitch to new and existing customers.

### Cases and estimation
One of the awesome things about being a developer is that we know what we have to do because we always get a well defined assignment with a pre-defined number of hours attached to it. Make it within this estimate and everyone are happy. Well, we all know that this is not true - not in a long shot. In my employment, I have experienced a lot of different levels of estimation and case handling, this spanning from a title without any estimate to being a long essay on what the customer want, and the customer had actually estimated the case (stating the obvious, don't trust a estimate defined by the customer). But lets talk about utopia. A case should have a title and a description that defines the goal for why we are doing this, along with who it benefits - call it SCRUM or whatever, I really don't care. By defining the value to the developer, you instantly motivate the developer to get a business insight. Yes, there might be cases that just needs to be resolved because it's maintenance - but try to define the WHY - it might not even be necessary to do the case because there might not be a why.

> Why should I spend my time with this case? Who does this benefit?

> Cases not containing title, description and estimate should not exist or are not fully matured cases yet - therefore we can not start working on them yet.

---

So lets get back to the story about the event management system. At a point in time the technical debt and lack of proper architecture in the solution sort of destroyed the mood and motivation in the development group. We knew that we couldn't release anything without breaking something somewhere - and if we released a new version it could result in days of clean up - and by the way the release to one customers environment took around 1-2 hours. The effect was obviously less deployment, less feature released to the customers and a very slow development cycle (defined as the time it takes to make a change in the system to it reaching the customers production environment). We were considering quiting the project and finding other projects in the company to work on. It was a crisis. 

At that point we managed to convince the company to pay some hours to re-factor the project into a proper maintainable state. We, the developers, defined the goal to be:

> Make the project deployable (putting code onto production) without destroying the universe at the same time.  

> Create documentation!!! And make sure developers understand and follows the values and procedures defined in it.

After the first week, we could for the first time download a fresh copy of the source code and build it without any errors - this eliminating the 7 to 14 hours used to setup a new development computer. This saving money for whoever paying those hours, may it be the company or the customer. 

Before this major restructuring of the project, I was a part of a different development team doing a large re-creation of a customers existing website. Here we tried to use different tools to shorten the development cycle, and it was here I learned about Continuous Integration and Delivery. So I went to my closest boss at that time and said that this was the approach we were gonna take on this project - me at the time didn't have any authority at all, but it seemed to work. One week later we had a functional build server (computer doing all the heavy work) and we had our first deployment pipeline to our internal testing site.

It's pretty cool (yep, like a little boy getting a new toy) to be able to develop a new feature on a system. Merge it into a testing branch. Take a coffee or start something else. Come back and know that the building server have deployed your new feature to the testing site.

But why stop here? The next week or so we managed to get this done on our seven test sites. Translated to English, when we merge our new feature to the testing branch, the build server will push to seven test sites within an hour (our build server is not that fast - admittedly). So before we needed to manually keep track on which customer had what version on their testing site, till now, all the customers have the same version of the system on their site, ready for testing.

At this point, we made a copy of our deployment pipelines and adjusted it to be able to deploy to production. My project-manager said to me:

> From now on we will close all release cases and not register time for releases unless something specific needs installed or configured.

Let me recap. We had a system that took 1-2 hours for a release for one customers staging site. We could now deploy to all seven test sites within an hour. We actually also at one point planed a scheduled release at 7:00 in the morning were non of the developers could be on-site - this was done without any problems. We could ensure that all components: background programs, websites, API's etc had the same version throughout the system and was compatible with each-other instead of manually copy-pasting files between local computer to remote server. From a HR perspective we took some developers without motivation, and made them work with a smile by listening to them and making them feel that they have a saying.

> Listen to the developers - they should be the proffessionals

From a financial point of view, this was stupid. We had just closed all the release cases, potentially removing profit from the company. But instead of using time on deployment, we now used time on development - this leading to the next few sections.

### Boring tasks
I could mansion a lot of boring tasks that have annoyed me countless times over the years. One of the would be to register how many quarters of hours I spend on each case so that the sales staff could send out a bill to the customer - and I get that I need to register my spend hours, but at one point it got to much. The company made the big mistake to implement a Slack-bot that posted a message in a public chat-channel if you were missing to register some hours. First of all, the time tracking system was ancient with no descent UI at all, with less to none reasonable possibilities to interact with it - and now we had to register hours before 10am or else it would be posted in the channel.

That was the motivation to remove this boring task from my focus. So, by investing some spare time, I managed to do a integration between my Outlook calender and this ancient dinosaur of a time tracking system. So now, I actually don't care about time tracking anymore, because its a boring task that gets done for me (I still have to put it into Outlook - but the UI is a way better).

My point of this small example is:

> Don't spend time on boring tasks that should be automated. A developer shouldn't focus on boring non-productive tasks when it can be automated.

So if you have a process, or a problem that occurs often, try to eliminate it. Just like when it comes to deployment of code to testing and production environments. Why should the developer have focus on building it and deploying it when it can be done for him (and adding value to the process as a side effect). When it comes to the time tracking example, the side effect of me doing this integration, was that I now actually writes comments on what I spend my time on, and not just registering my time on a case number. In the case of automating builds and deployments, it is done the same way every time, you know when something F's up and you actually have a easy and quick way to revert back to a prior version - oh yeah, you get versioning for free with automating deployment, along with automated backup of target databases.

One of the things that can make a big difference is the collection of error messages. In the event management system we had five workers and two websites, each with the possibility of generating errors affecting the other components that could generate even more errors. Instead of having to look in each of the components local error logs, I really missed some central service I could access and get a total overview of what was failing were. This adding a big value for the operations team when having to debug a error on the production environment. 

### Infrastructure
I am currently having a battle with some management regarding the benefits of automation of different parts of the development of new systems. The main thing is that the company have to invest in some serious infrastructure in the form of a decent build server visualization environment or hard-core iron (physical computer) in the server room. That's one part of it, the other is the software. Should it be something you pay for? What are the vision of doing this automation? Should we invest time in using a new deployment software no one have experience with, just because its free and with no cost?

A lot of these questions are basically political questions - but as I see it, you cant as a leader answer these questions because you are not a developer! Its a bit rough to state that, because off-cause you can be a developer even though you are a leader, my point is therefore:

> If you dont work with code, you can't answer questions regarding what is best for the code and its developers - ask the developers!

Something I hate (hate is a powerful word, but in this case true) is when decisions are made that affects you, taken by people, who can't be regarded professional in that particular area of expertise. So if you somehow have a leadership responsibility, please use the resources at your disposal. Use the professionals, the developers, the hosting people, architects - but please do not take a decision without talking to the people it affects. If you do so, be prepared for riot, objections and backstabbing.

## The end?
So I have talked a bit about a system that have come a long way from being a POC-project that basically was a big ball of hurt, to actually being a project we develop on daily. And when I say develop, I actually mean making the project better with more features, not cleaning up after failed releases all the time. We can fix a problem for a custom and have the code on the production environment within an hour (normally faster). But what should we draw as a conclusion? Should we just stop registering our spend hours and use countless amount of money on automation? Well no. But we have to let our company, what ever that is, aware of the benefits it comes with by doing things a bit more automated and at some point this might be the new standard for how to do development.

As a final example: Today I even had a case with top priority which I fixed fairly easy and I got a response back from the customer thanking me for a quick response time - the only thing I did was to fix it, merge it into production and press release in the deployment system. The fix was so small, but the value for that customer was almost indescribable.

## Conclusion on the trip
As a leader, make sure you have the proper infrastructure set up, so that the people you call developers (and you have properly employed them as the same thing as well), actually do development and not maintenance. There will always be maintenance cases, but they should be limited to the absolutely necessary. Listen to your developers, listen to what they need to make their job easier, better and faster - and then make it happen. And for the love of god, be ready to defend the opinions those developers have. If they don't believe you'll fight for them, they'll not fight for you when you need it.