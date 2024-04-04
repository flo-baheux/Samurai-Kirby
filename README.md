# Samurai-Kirby

Sources for my game Samurai Kirby, a demo for a reflex game using a dedicated server.

[To test the game please visit the itch.io page!](https://laywelin.itch.io/samurai-kirby?password=Laywelin-42)

[You can find the server source here](https://github.com/flo-baheux/Samurai-Kirby-Server)

## My goals through this project

There is a bunch of specifics things I either wanted to learn, test or focus on.

- Proper networking on a separate thread
- Serialization
- At least decent separation of gameplay and networking
- Accessibility
- Playable with controllers or keyboard, menu navigation included
- As few assets as possible (which means reusing as much as possible the initial spritesheet)
- Learn how to manage assets, fonts, sounds, animations, i18n, UI vs gameplay, coroutines 

## Caveats I'am aware of

### No loading or error display
I did not spend time working on that, and I regret that it's not implemented!

### Polymorphism and casts (as used in serialization) is highly unefficient.
I did it to play with C# to get used to it, and with this game scope it's a non issue. However that could be highly improved!

### The network thread is only used to receive, not to send
I am spawning a network thread to receive messages, but using Tasks to send. 
It would have been better for instance to have a queue of messages and use the network thread to consume it.

### Not leveraging Data Oriented Design
I did not learn about that paradigm until recently - my next project will probably dip into it because I'm very curious about it!
However from my current understanding this would not change much on this project considering its size.

### It's not customizable enough - not suited for a team
I did not leverage Prefabs enough. It shows that it's a solo project, and my next project will focus a bit more on setting things up
as if I was in a multidisciplinary team.