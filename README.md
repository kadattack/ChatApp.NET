# ChatApp.NET
This is a web chat application created in .NET 6 with Angular and PostgreSQL. The app is based on a Django tutorial StudyBud:
https://www.youtube.com/watch?v=PtQiiknWUcI, where I took the HTML/CSS from the Django tutorial and rebuild it from scratch with the Angular and .NET Framework. 


The .NET chat app includes live chat implemented with SignalR.
Account management is implemented with IdentityUser and roles are implemented with IndentityRole.

## TODO
- [ ] Implement role management. Roles [Admin, Moderator, Member] exist, however there are no privileges for each role yet. Currently everyone is considered as a Member.
- [ ] Add string length checks. Currently there is no length limit input on all forms.
- [ ] Add identication of online presence. People can currently only see in the console log if someone comes online.
- [ ] Fix timestamp strings so they turn from time to dates once 24h pass.


## Release
This project is only used as a portoflio.
Implementation of this project can be found at: 
https://chatappdotnet.herokuapp.com/
