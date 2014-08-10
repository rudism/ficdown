# [Cloak of Darkness](/foyer)

A basic IF demonstration.

Hurrying through the rainswept November night, you're glad to see the bright lights of the Opera House. It's surprising that there aren't more people about but, hey, what do you expect in a cheap demo game...?

## [Foyer]("Foyer of the Opera House")

[You](#examined-self) are standing in a spacious hall, splendidly decorated in red and gold, with glittering chandeliers overhead. The entrance from the street is to the [north](#tried-to-leave), and there are doorways [south](/bar) and [west](/cloakroom).

### Tried to Leave

[You've](#examined-self) only just arrived, and besides, the weather outside seems to be getting worse.

### Examined Self

[You aren't carrying anything.|You are wearing a handsome cloak, of velvet trimmed with satin, and slightly splattered with raindrops. Its blackness is so deep that it almost seems to suck light from the room.](?lost-cloak)

## Cloakroom

The walls of this small room were clearly once lined with hooks, though now only one remains. The exit is a door to the [east](/foyer).

[Your cloak is on the floor here.](?dropped-cloak)
[Your cloak is hanging on the hook.](?hung-cloak)

- [Examine the hook.](#examined-hook)
- [Hang your cloak on the hook.](?examined-self&!lost-cloak#lost-cloak+hung-cloak)
- [Drop your cloak on the floor.](?examined-self&!lost-cloak#lost-cloak+dropped-cloak)

### Examined Hook

It's just a small brass hook, [with your cloak hanging on it|screwed to the wall](?hung-cloak).

## [Bar]("Foyer Bar")

You walk to the bar, but it's so dark here you can't really make anything out. The foyer is back to the [north](/foyer).

- [Feel around for a light switch.](?!scuffled1#scuffled1+not-in-the-dark)
- [Sit on a bar stool.](?!scuffled2#scuffled2+not-in-the-dark)

### Not in the Dark

In the dark? You could easily disturb something.

## [Bar](?lost-cloak "Foyer Bar")

The bar, much rougher than you'd have guessed after the opulence of the foyer to the north, is completely empty. There seems to be some sort of message scrawled in the sawdust on the floor. The foyer is back to the [north](/foyer).

[Examine the message.](/message)

## [Message]("Foyer Bar")

The message, neatly marked in the sawdust, reads...

**You have won!**

## [Message](?scuffled1&scuffled2 "Foyer Bar")

The message has been carelessly trampled, making it difficult to read. You can just distinguish the words...

**You have lost.**
