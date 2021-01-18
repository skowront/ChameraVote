ChameraVote
=======
ChameraVote is a solution for online votings.

Intro
-----------
This application was developped to allow dealing with online votings (for example NGO votings). 
Key feature is that it is possible to restrict list of users who can cast their vote, authorize them, but also keep their ballots anonymous to a certain level.
Ballot anonymization is implemented through RSA blind signatures.

Server
-----------
The server side is implemented with Python 3.
Server application requires a few python packages but it should run on any machine that supports Python 3.

Client
-----------
There are 2 client applications available (Web client - .html+.js and Desktop client - c#+WPF ).
Web client was made to run on any browser (tested on chrome and opera). 
Desktop client is made for Windows only.

| Feature         | Web client       | Desktop client  |
|-----------------|:----------------:|----------------:|
| Registration    | +                |+                |
| Logging in      | +                |+                |
| Casting votes   | +                |+                |
| Adding votings  | -                |+                |
| Browsing results| -                |+                |

Application is still under developpment.

Upcoming features:
  * equal features for web and desktop client
  * printing results to a document
  * password change (possibly with configurable smtp to send pwd reset codes, that would also require adding e-mail to the database)
  * localization
  * more admin tools


Below you may find some screenshots from desktop client.
![alt text](https://github.com/skowront/ChameraVote/blob/master/github/images/chameraVoteClient.jpg)


![alt text](https://github.com/skowront/ChameraVote/blob/master/github/images/chameraVoteClientResults.jpg)
