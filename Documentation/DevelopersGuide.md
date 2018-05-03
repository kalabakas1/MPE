# Developers guide for an easy day
A lot of things can be done to make a developers life easier, and make him focus on the one thing he needs to do. Develop software!. This is a short description of some of the things you can do to make your fellow developers life easier on a daily basis.

## Motivation
At one point I got tired of correcting stuff just to make the solution I worked on compile. This included configurations that didn't exist in the configuration files. Some of them didn't have transformations for the different environments, so they got deployed with the development values, which basically crashed the site. At the end of that day I started creating this document to make my life easier and to sum up what could be avoided of small irritating errors that might occur.

## The List!!!
It is gonna be a small list that sums up the different things along with an argument that sort of legalizes it. Not gonna be religious about it, but it's something I have gotten frustrated over, which could be avoided.

### Remember to add configuration transformation items - you create, you transform
When dealing with web applications or normal executables you might need to add something like appSettings in the .config files to differentiate between development, staging and the release environment. I spend around two hours trying to make all the configurations have the correct values, just because the developer that created didn't choose the correct values for the different environments.  You might not know the values, but you should know if it something that is gonna be transformed when building a specific environment.

### Develope in branches - master should always be production ready
There is a lot of hype regarding how to do software development, but in the current state of my work and employment it is still a best practice to create a branch if you have to do anything else then creating markdown files for documentation or something similar trivial. If you fixes a bug in production, creating a new feature - please do create a branch for it, and merge master into your branch on a regular basis.

### Nothing is done before it have been tested on QA/staging/some-other-test-environment-name
It's pretty simple, unless you have pushed it to a testing environment where you can see it works - then it is not done. When you have tested it on a test environment, then you should also have considered configuration transformations for at least that environment. After that you can mark the case as done and move on to the next interesting bit of your day.

### Commit your code before you go home, die, get sick or otherwise leave the office
One of the worst things I have experienced, and properly you all have, is the setback of a coworker being sick. When that happens it should be easy for someone else to take over his work, checkout his branch and work on it as normal. But in the real world, co-workers aren't always thinking in "what if"'s. What if I get the flue tomorrow, can we then finish on time? So in the perfect team I would suggest that every developer work in a separate branch from the master, don't go home before committing all the work, no matter if it compiles or not. That way, if the co-worker turns in sick the next day, you can continue his work by giving him a call and checking out his code.
__Commit before you stop working__

### Don't let your pride use the hours from a case
So we are all human beings, and we all have some sort of pride in what we do. But when it comes to deadlines, and tasks that already have a estimate associated with it, there just ain't room for pride. What pride does to a developer is pretty simple. Imagine that you have gotten a fairly technical task that contains a lot of new stuff for you to learn. But your dog is sick, your parents are mad because you married a farmers daughter etc. That combined with pride results in a big use of the estimated hours, without you asking for help. Wasted hours that could be spend on solving the problem. My experience is that if you get stuck doing a task, just ask someone in your team for help, and get on with it. 

### Don't comment out code, remove it or change the architecture
So why does people comment out code? I have asked my self that over and over again, and got to a lot of good answers. But basically the only one person that knows the reason why some code is commented out, is the developer himself - and thats why you should never remove code by comment, just simply delete it. Comments should be in a normal text form, explaining some critical or complex code, never to remove code. It just brings doubt and confusion to the project.

### Have unique exception messages
```csharp
if (String.IsNullOrEmpty(viewModel.VoucherCode))
    throw new Exception("Voucher is invalid");

if (viewModel.BasketId == Guid.Empty)
    throw new Exception("Basket is invalid");

var basket = _basketRepository.GetBasket(viewModel.BasketId.ToString());
if (basket == null)
    throw new Exception("Basket is invalid");

if (basket.VoucherCodes.Any(c => c.Code == viewModel.VoucherCode))
    throw new Exception("Voucher code is already used on this basket");

var voucher = VoucherCode.GetByCode(viewModel.VoucherCode);

if (voucher == null ||
    voucher.Deleted ||
    voucher.IsUsed ||
    voucher.TotalUsages >= voucher.MaxUsages ||
    voucher.Voucher == null ||
    voucher.Voucher.SiteId != basket.LanguageId ||
    voucher.Voucher.Deleted ||
    !(voucher.Voucher.ValidFrom <= DateTime.Now && voucher.Voucher.ValidTo > DateTime.Now))
        throw new Exception("Voucher is invalid");
```

So during the last few weeks I have seen and debug my fare share of code. Every day not under 10 hours of intensive work. During this period, a small part of the team tried to figure out why we couldn't add vouchers to our system. We constantly got "Voucher is invalid". So we found the code, started to read from the top down, found the place where the exception were thrown. But it didn't make any sense. We added logging to see if the data model were invalid or null - it contained all the data needed for the code to execute without exceptions. After a short break, we instead started to read from the bottom up, realizing that we were looking at the wrong exception message. 

The lesson here is: Don't have duplicate exception messages within the same method. Easier to locate the exact exception if the messages are unique within a specific context.

### You can't work on a case if that case does not contain a description of the task at hand
Imagine that you get a case with a headline limited to about 200 characters - and that's it! How would you solve that it you knew by the header that this is gonna be a huge complex task? After the last couple of weeks, I have concluded that I'm never ever gonna touch that case at all. It is extremely difficult to solve a case if you don't know what the customer expects, and what the criteria of success really are. So please do take the time to describe what a case is about - what it should change, where to do the change, what the customer expects, and how to validate that the case is done.

> Don't create cases that only contain a headline - it only bring frustrations with it.

### You cant replace functionality if you don't know what it does
For about a month ago I was told to replace a basket functionality in a e-commerce platform with some copy of a existing implementation. The agreement with the customer was:

> It just have to work exactly as before

First of all, do never guarantee that! And never ever do it if you do not have a complete overview of the existing functionality you are about to replace. If a system is neatly implemented and you have documentation that visualizes what is gonna happen when you save a entity, e.g. five events will fire, then it might be fine to guarantee this. But if your system is not documented at all, then the developer goes from a small area of focus that only takes a short time to get a understanding of, to having to understand almost the entire system.

> Do map the functionality you should replace and describe the functionality and flows. Then make the customer sign off on it. You now have a consensus regarding what is gonna be the end result after replacing the functionality.

### Keep your customer bussy with testing
PUT TEXT HERE (13 deployments on QA, 10 deployments on production)

### Read the documentation
"How do I do this?", "Where do I find this?", "How does this work?". It is fine to ask questions, but sometime it gets a bit too much. I really admire the university that had a support-teddy-bear as a first line of support. If the people needed help they were required to tell the problem to the bear before they got to talk to a real person. It actually forced the people to reflect on a solution, and normally they didn't needed to talk to an actual support-person, they solved their own problems. Documentation is the same thing, it contains answers already given to the questions you are asking. Read it instead of asking, and try to keep up every time the documentation is updated. If you ask too much, you generally just end up with a answer in the form of:

> Read the documentation!!!

## Conclusion
All teams are different and this small list of stuff can't fit all teams or projects. But the essence of it is the same. Identify the irritating repetitive errors that occurs, find a solution to them, and implement the solution as a concrete deployment rule, or a culture in the team. You can even game-ify it by having a cake/beer rule - for each time a certain error occurs, the developer should bring it for the next team meeting.