# TwitterBot
This code is designed to repost tweets containing a pscp.tv url from a given list of twitter accounts.

Developed in C# .NET Core 3.1

### Supported Arguments:

**/appkey=** The application key of the twitter app you will run the bot on.

**/appsecret=** The application key of the twitter app you will run the bot on.

**/authkey=** User's Authentication Key (from OAuth) goes here 

**/secret=** User's Authentication Secret (from OAuth) goes here


### Optional parameters:

**/follow=** A comma separated list of twitter ids to follow. numbers only, use https://tweeterid.com/ to convert a handle

**/delete=** A comma separated list of tweet ids to delete

**/tweet=** A message you would like to tweet out once when the application runs

