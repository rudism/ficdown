# [The Robot King](/robot-cave)

An experiment in markdown-based interactive fiction by Rudis Muiznieks.

Release r2014-06-29

## Robot Cave

> You wake up and emit a great robot yawn as you stretch your metallic arms. Today is the big day! You have to start your new job working for the Robot King at the Robot Palace!

You're in your cave and you can see [a peg by the door where you usually hang your raincoat|your raincoat hanging by the door](?raincoat).

Your cave only has one tiny window, and through it you can see [the sun shining through the clouds|that it's raining heavily outside](?stopped-raining).

**What do you want to do?**

- [Go outside and start walking to the palace.](/outside)
- [Wait for it to stop raining.](#stopped-raining)
- [Put on your raincoat.](#raincoat)

### Raincoat

You take your raincoat and put it on. It fits perfectly!

### Stopped Raining

It feels like hours, but it finally stops raining. You hope you won't be late for your new job!

## [Outside](?!raincoat&!stopped-raining)

> You step through the door and feel the water flowing over your metal body. Brrr! That's cold! You start to think that maybe getting your raincoat would be a good idea. This is just the kind of rain that might turn you into a rusty robot statue if you stay in it too long.

You're standing on your front porch in the pouring rain. You need to get to the palace for your new job, but you don't want to rust!

**What will you do?**

- [Continue walking to the palace.](/rusted)
- [Go back into your cave.](/robot-cave)

## Outside

You step through the door and feel the early afternoon sun warming your metal body. It feels good, but you were supposed to start your new job early in the morning!

You run as fast as you can all the way to the Robot Palace, but it's already too late.

"You were supposed to be here first thing in the morning," says the palace guard. "We can't have sleepy-head robots working at the Robot Palace! Try finding a different job instead."

**You've been fired!**

## Rusted!

You start walking toward the Robot Palace in the rain. Who needs a raincoat anyway? As you move down the path, rust starts forming on your legs and knees so you have to walk slower. Eventually the rust gets so bad that you can't move anymore at all!

As your whole body rusts over, you wonder what you could have been thinking. Only a crazy robot would ever go out into the rain without a raincoat!

You will have a long time to think about your mistake while you wait for another robot to come and help scrape off all the rust so you can move again. Since you never made it to the palace for your new job, you'll probably be fired.

**You have turned into a rusty robot statue!**

## [Outside](?raincoat&!stopped-raining)

You head out the door and into the rain. It's a good thing you put on your raincoat, because it's just the kind of rain that would probably turn you into a rusty robot statue if you stayed in it for too long.

You follow the road by your house all the way through town until you reach the door to the Robot Palace.

The palace guard looks you up and down. "What do you want?" he asks.

**What will you tell him?**

- ["I'm the new janitor-bot!"](/palace-entrance#new-job)
- ["I'd like a tour of the palace!"](/palace-entrance)

## Palace Entrance

The robot guard looks at you and [nods|frowns](?new-job). "[Oh yeah, they told me to expect you. You're supposed to be starting today right?|We don't do tours on weekdays. Hey, aren't you the new janitor-bot who's starting today?](?new-job)"

**How will you answer?**

- ["Yup!"](/palace-entrance)
- ["Nope!"](/back-to-bed)

## Back to Bed

The robot guard looks at you with a confused expression on his face, then stops paying attention to you.

I guess you decided that you don't want a new job today after all. You turn around and walk all the way back home, where you hop back into bed for a quick nap.

Without a job, you fail to earn any money and you can no longer afford fuel to power yourself.

**You run out of fuel and shut down forever!**

## Palace Entrance

> The robot guard nods and ushers you into the palace through the large front doors.

> "You'll want to report to the Master Janitor Robot downstairs. He'll give you your uniform and get you started," the guard says, then quickly leaves and shuts the doors behind him.

The palace entrance is one of the biggest rooms you've ever seen! There are statues of knight-robots and pictures of all of the old Robot Kings going back for centuries lining the walls. The picture closest to you is a picture of the current Robot King. He looks a lot like you!

There is a grand double staircase leading up to the throne room, a hallway straight ahead that leads to the living quarters, and a door to your left that says "Stairs."

**Where do you want to go?**

- [Go upstairs to the throne room.](/throne-room)
- [Go through the hall to the living quarters.](/living-quarters)
- [Go downstairs to see the Master Janitor Robot.](/palace-basement)

## Living Quarters

You walk into the hall that leads to the living quarters, and find a gate blocking your way. There is a robot scanner installed on the gate. I guess it only opens for robots who live or work here. Maybe the Master Janitor Robot will have a way for you to get through.

- [Go back to the palace entrance.](/palace-entrance#tried-gate)

## Palace Basement

> You walk down three flights of stairs until you reach the basement. The staircase is dark, but the basement is even darker. It's a little scary! You hope you can get the information you need from the Master Janitor Robot and get out of here as quickly as possible.

You're standing in the basement where new employees can pick up their uniforms and learn what their jobs are for the day.

[The Master Janitor Robot is pacing back and forth here, muttering to himself.|There is a funny looking robot here pacing back and forth, muttering to himself. That must be the Master Janitor Robot. When he notices you, he stops muttering and stares at you with crazy eyes.](#talked-to-master)

**What will you do?**

- [Go back upstairs.](/palace-entrance)
- [Ask the Master Janitor Robot what he's muttering about.](#talked-to-master+muttering)
- [Ask the Master Janitor Robot about your uniform.](#talked-to-master+uniform)
- [Ask the Master Janitor Robot about the gate upstairs.](?tried-gate#talked-to-master+about-the-gate)
- [Ask the Master Janitor Robot about your job.](?uniform#started-job)

### Muttering

"Muttering?" says the Master Janitor Robot. "Was I muttering? I hadn't noticed."

The Master Janitor Robot pauses thoughtfully for a moment, then resumes muttering.

### Uniform

The Master Janitor Robot's eyes light up a pleasant shade of blue. "Ahh, you must be the new janitor-bot starting today!" he says.

He walks to a box in the corner and pulls out a blue janitor's uniform, then hands it to you. You put it on.

### About the Gate

"Ahh, yes, the gate," says the Master Janitor Robot. "Quite a clever contraption. There's a scanner attached that looks for a special device that's sewn into the [uniform I gave you|uniform that employees here wear](?uniform).[ As I said, you'll want to head up there now to start cleaning room 13.](?started-job)"

### Started Job

"Ready to get going?" says the Master Janitor Robot. He continues before you have a chance to answer. "Good, good. Your first job will be to clean room 13 in the living quarters. That's where the Robot King keeps all of his spare robes and crowns. There's a janitor's closet right next to that room where you can get a mop to clean the floors, and a duster to dust off the crowns."

The Master Janitor Robot scratches his chin for a moment, then resumes pacing back and forth and muttering to himself.

## [Living Quarters](?)

You head into the hallway that leads to the living quarters and come to a large gate. A scanner attached to the gate lights up and beeps a few times. After a moment, you hear a click and a soft hiss as the gate opens to let you pass. Once you walk through, the gate hisses and clicks shut behind you.

You notice with some alarm that there's no scanner on the inside of the gate. You don't know how to get back out!

## [Living Quarters](?uniform&!job-started)

That's when you realize that you never asked the Master Janitor Bot what your job here was. You just took your uniform and left!

**You have failed to perform your new job because you never found out what it was.**

## [Living Quarters](?uniform&job-started)

That's no problem though, because you already know what your job is. You continue down the hall, looking at and passing all of the doors until you come to the one marked with a "13." Right next to it is another door labeled "Janitor's Closet."

You open the closet and grab the mop and duster. You're so excited! Your first day as a janitor working for a Robot King that looks just like you, and you are about to enter a room containing all of his spare robes and crowns. What fun!

**You have reached the end of the intro to The Robot King.**
