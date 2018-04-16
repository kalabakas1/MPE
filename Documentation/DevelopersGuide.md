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

## Conclusion
All teams are different and this small list of stuff can't fit all teams or projects. But the essence of it is the same. Identify the irritating repetitive errors that occurs, find a solution to them, and implement the solution as a concrete deployment rule, or a culture in the team. You can even game-ify it by having a cake/beer rule - for each time a certain error occurs, the developer should bring it for the next team meeting.