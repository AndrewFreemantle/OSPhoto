##How to contribute
First of all, thank you for taking the time to contribute to this project. Our aim is to maintain a stable project and try to fix bugs and add new features continuously. You can help us do more.

###Getting started

####Check out the roadmap

Development is grouped into *milestones*. If there is a feature or bug that is not listed in the **issues** page or there is no one assigned to the issue, please assign yourself to add or fix it! It's always a good idea to discuss the task in the issue to agree the approach to take as your work will often be built upon. 

####Writing some code!

Contributing to a project on Github is pretty straight forward. If this is you're first time, these are the steps you should take.

- Fork this repo.

And that's it! Read the code available and change the part you don't like! Your change should not break the existing code and the tests should pass when you're done.

If you're adding new functionality, start from the branch **master**. It is best practice to create a new branch and work in there:

````shell
(master) $ git pull
(master) $ git checkout -b feat/new-feature 
````

When you're done, please submit a pull request and the project maintainers will review your changes. This may involve a little more work on your part to keep the codebase consistent. 

Please keep the following in mind:
- [SOLID design principles](https://en.wikipedia.org/wiki/SOLID)
- [Design patterns](https://en.wikipedia.org/wiki/Design_Patterns) where appropriate
- Ideomatic language style (i.e. conforming to the typical or accepted conventions of the language)
- [YAGNI](https://www.martinfowler.com/bliki/Yagni.html) (i.e. implement what's needed, nothing more. Similarly, remove what is no longer needed)
- Simplicity, readability and maintainability is preferred over complexity 

####Tests

The project uses a test-first, Test-Driven Design approach. New services must have accompanying unit (or integration) tests and all tests must pass before a PR will be accepted. 

####Documentation

Code comments are encouraged for particularly dense or terse methods, but otherwise you should endevour to write clear, intuative and understable software. 