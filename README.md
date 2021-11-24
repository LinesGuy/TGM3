# TGM3
A Tetris clone written in C# + MonoGame

The goal is to emulate Tetris: the Grand Master 3 Terror Instinct as closely as possible, and might include customisation options to closely emulate other Tetris games

# Features
[ ] = Todo

[~] = WIP

[x] = Done

if something isn't listed here I probably forgor :skull: to add it, ping me on discord if you want it added

Basics:

[X] Piece movement

[X] CW / CCW rotation

[X] 180 degree rotation

[X] Hold piece

[X] Next piece previews

[~] Standard piece colours (SRS done, ARS not done)

[~] Gravity (Done except piece will not auto lock when it hits the ground)

[~] Soft drop (Same status as above)

[X] Hard drop

[X] Line clear detection and clearing

[X] Game over detection



More advanced Tetris stuff:

[X] Ghost piece

[X] SRS spawn location and orientation

[X] SRS Wall/floor kicks

[X] SRS rotation

[ ] ARS spawn location and orientation

[ ] ARS Wall/floor kicks

[ ] ARS rotation

[ ] [Infinity](https://tetris.fandom.com/wiki/Infinity)

[X] IHS (Initial Hold System)

[X] IRS (Initial Rotation System)

[X] ARR (Auto Repeat Rate)

[X] ARE (Entry delay)

[X] Line ARE (Entry delay after clearing a line)

[X] DAS (Delayed auto shift)

[ ] Piece Lock (Delay between piece hitting ground and 'locking')

[ ] Line clear (Delay when clearing line)

[X] Bag randomizer (AKA 7 piece randomizer)

[X] 20G

[ ] 20G SRS exceptions (scroll down to 20G section in SRS page on tetris wiki)

[ ] Synchro move support

[ ] Zangi move support

General TGM3 specific stuff:

[X] Line clear bonus (Clearing 3 or 4 lines gives +4 or +6 levels respectively)

[ ] "Tetris COOL": If player makes certain number of tetris in a section (see wiki for values)

[ ] "Special COOL": If player does 3 tetris in a row, 2 t spins in a row, or t spin triple (world only)

[ ] "Hole REGRET": Unknown exactly how this is triggered but generally when the player leaves holes in the playfield

[ ] T-SPIN! detection, when the player clears a line with a T-Spin

[ ] Big mode code: LLRRLRLR

[ ] Game results screen

[ ] Current level display

[ ] Badges/awards (I have no idea how these work yet)

[ ] Game time display



Master mode:

[~] Section COOL/REGRET (Detection is complete but is otherwise unused so far)

[ ] Grade recognition system

[ ] Internal grade system

[ ] Combo multiplier

[ ] Level multipler

[ ] Staff roll grade system

[ ] Level 500 early end

[ ] Score

[ ] Half-invisible + Full invisible credits roll



Easy mode:

[ ] Easy mode piece guide

[ ] Easy mode [hanabi calculations](https://docs.google.com/document/d/11J4BAxrkzxI5VYa1dt7oLuWEhI0yhv8OnNpgfbdKS7U/edit)

[ ] Easy mode fireworks

[ ] Easy mode ending at level 200

[ ] Easy mode 20G credits roll of fixed length



Sakura mode

[ ] the mode (probably will never get implemented because I never played it, DM me if you want to help with it though, you don't *need* to know how to code, just have good 
knowledge in Sakura and Tetris in general so I can add it)

[ ] Random piece sequence: A+B+C+D




Shirase:

[ ] End at level 500 if slower than 2:28:00 (classic) or 3:03:00 (world), award grade S5 minus section REGRETs

[ ] Between levels 500-1000, spawn garbage lines at the bottom

[ ] End at level 1000 if slower than 4:56:00 (classic) or 6:06:00 (world), award grade S10 minus section REGRETs

[ ] Between levels 1000-1300, new tetrominos spawn with monochrome [] blocks (and disable line clear effects for these blocks)

[ ] Shirsae grading system (math.floor(level) minus section REGRETs)

[ ] Section REGRET detetion (if section takes more than one minute)

[ ] Big tetromino challenge credit roll

[ ] Shirase ARE / Line ARE / DAS, Lock, Line clear delay table

[ ] Rising garbage quota table

[ ] Score system



User accounts etc:

[ ] User accounts (enter username and passcode)

[ ] Examinations

[ ] Demotions

[ ] Highscores

[ ] Number of plays

[ ] Master/Shirase/Easy/Sakura grade displays

[ ] Decorations



Graphics stuff:

[X] Playfield grid

[ ] Fancy border

[ ] Background that changes depending on level

[ ] Playfield opacity

[ ] Particle effects upon clearing a line

[ ] Flash piece when locking

[ ] In ARS, dim piece during piece lock



Audio stuff:

[ ] Background music that changes depending on level

[ ] Piece lock sounds

[ ] Next piece sounds

[ ] Line clear sounds



Menus and GUI:

[ ] Selection for Easy/Sakure/Master/Shirase mode

[ ] Selection for SRS (world) or ARS (classic)

[ ] Title screen

[ ] Settings (controls, volume, other game customization)

[ ] READY GO before game starts



Multiplayer:

[ ] Two boards with their own controls

[ ] VS mode

[ ] Press start to disable multiplayer

[ ] PLEASE INSERT COIN indicator (or something similar)



Other:

[ ] Marathon mode

[ ] 40L race (and other line counts)



# muh discord

Lines#9260
