# Distracted Driver gameplay update

## Fly Away collision fix

Swatters and score zones now compare their actual UI rectangles in the playfield's local coordinates using a separating-axis test. Car yaw, pitch, roll, mirrored UI transforms and inherited scale no longer change the collision result. SwatterPairMover is the single collision owner and checks after movement; the old FlyCollisionUI component remains as a harmless compatibility component for existing prefab references. Death sound plays once through the minigame manager.

## Whack-a-Fly

Open the Games app. Both games participate in its existing random selection; NEXT GAME on either game's title screen switches to the other available game.

- Six fly holes, a 45-second round, and three lives.
- Click flies after they emerge. Missing an emerging fly costs a life; scoring gradually increases the pace.
- Successful hits award dopamine and count toward gamer passengers' ride goals.
- Pause stops the game. Leaving the app resets it. Retry starts a fresh round.
- Uses Fly Away's existing backgrounds, pixel font, wavy title, button sprites, fly sprites and sound/music hooks.

### Replace the pipe art

Open `Assets/Prefabs/Games/Minigame_WhackAFly.prefab` in Prefab Mode. On the root's **Whack A Fly Game** component, replace **Pipe Sprite** with your imported Sprite (2D and UI). It updates all six placeholders during play. Individual Image objects are named **Pipe Placeholder - replace sprite**, inside each Pipe Slot under Whack Board. The Fly Opening mask makes the fly rise from behind the pipe rim; resize this opening if your final pipe has a differently sized mouth. The GameScene instance is connected to this prefab.

## Passenger preferences

Each passenger has an editable **Preference** field. Existing passengers are distributed among the three types. The Taxi offers show the goal before accepting, pickup dialogue repeats it, the active ride tracks progress, and drop-off reports the fare and preference tip separately.

- **Speed:** meet the route's displayed time target for a 50% tip. Timing starts at pickup, not acceptance. Target is distance / 12 + 20 seconds, with a 30-second minimum.
- **Smooth:** starts at a 50% tip; each rough-driving event removes 20 percentage points. Hard turns, heavy braking and obstacle impacts count, with a one-second cooldown to prevent one impact counting many times. Road contact does not count.
- **Gamer:** earn five phone points while the passenger is aboard for a 50% tip. Both minigames count. Points before pickup and after delivery do not count.

Tips round to whole dollars, with a $1 minimum when a positive tip is earned. Ride metrics persist with active-ride saves; older saves without these fields still load.

## Upgrade strategies

Three new uncommon shop assets use the existing purchase, owned-upgrade and save systems. Each is stackable up to three times, including Joker copies. The listed downsides are visible in each shop description.

| Upgrade | Benefit per stack | Tradeoff per stack | Base price |
|---|---|---|---|
| Drift Kit | +30% boost power; +25% boost duration | 8% less cruising speed | $8 |
| Dopamine Rig | 35% more dopamine gain; +0.12 maximum dopamine speed multiplier | 20% faster dopamine drain; 8% lower fares | $8 |
| Premium Meter | +35% fares | 8% slower steering; 15% lower dopamine gain | $10 |

Percentage penalties and dopamine gain/drain multipliers compound. Additive boost/fare bonuses sum by stack. Existing daily price scaling still applies. Effects derive from the owned list rather than mutating base stats on each load, so reloading cannot multiply the same upgrade repeatedly. The new upgrade icons temporarily reuse an existing shop icon and can be changed on the three asset files.

## Tutorial button

Main Menu now has five buttons, including TUTORIAL. This starts a temporary practice run with the existing guided purchase/pickup/delivery steps and pauses the day clock. Practice updates remain in memory and never write to the normal save. Esc opens the usual pause menu; returning to Main Menu restores access to the saved run. Tutorial completion displays a practice-complete message.

## Validation

Automated checks are run in an isolated copy of the Unity project, not the open editor. They cover rotated/scaled collisions, preference reward thresholds and persistence, runtime minigame controls and app switching, upgrade tradeoffs and stack limits, and tutorial save protection. Rendered previews are checked before delivery. Playtesting and balancing the new numbers across full runs is still recommended.
