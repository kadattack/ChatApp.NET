# ChatApp.NET
A .NET Framework 6 with Angular and PostgreSQL implementation of a Django tutorial StudyBud:
https://www.youtube.com/watch?v=PtQiiknWUcI


The only thing taken from the Django tutorial above, is the HTML and CSS. The rest was made from scratch.


The app includes live chat implemented with SignalR.
Account management is implemented with IdentityUser and roles are implemented with IndentityRole.

## TODO
- [ ] Implement role management. Roles [Admin, Moderator, Member] exist, however there are no privileges for each role yet. Currently everyone is considered as a Member.
- [ ] Add string length checks. Currently there is no length limit input on all forms.
- [ ] Add identication of online presence. People can currently see in the console log if someone comes online.
- [ ] Fix timestamp stings so they turn from time to dates once 24h pass.


## Release
This procjet is only used for portoflio.
Implementation of this project can be found here: 
https://chatappdotnet.herokuapp.com/
