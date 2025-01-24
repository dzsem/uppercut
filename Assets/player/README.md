# Player

### Rendering
A játékos a default layer-en rajzolódik, az orderben a 10. (hogy a platformok előtt jelenjen meg)

### Hitboxok
A player 4 collider-ből áll:
 1. a rigidbody collidere (Friction physics material-lal) [default layer]
 2. az átesős platformok érzékelésére szolgáló hitbox (másik hitbox a Player objektumon) [default layer]
 3. a két ütésre szolgáló box (felfelé- és jobbra) [hitbox layer]

A vízszintes hitbox invertálódik inputtól függően (balra-jobbra néz); a függőleges hasonlóan (alapból fel, punchdown-nál le).

### Movement

Az alap movement ability-k:

|Ability|Bind + követelmények|Cooldown?|
|:------|:-------------------|:--------|
|Uppercut|`SPACE` + földön|nincs|
|Dash|(`←→AD`) + `X` + nincs cooldown-on + két földetérés között csak egyszer|van|
|Smash/Punchdown|`↓S` + `X` + levegőben + két földetérés között csak egyszer|nincs|

### Interface
|Függvény/Field|Absztrakt|
|:-------------|:--------|
|`bool disableInput`| Kikapcsolja a player inputot, ha igaz. Cutscenekhez / pause-hoz hasznos lehet. |
|`void RefreshUppercut()`| - (alapból csak akkor hívódik meg, ha földet ér a játékos) |
|`void RefreshDash()`| - (alapból csak akkor hívódik meg, ha földet ér a játékos) |
|`void RefreshMovementAbilities()`| Refresheli az uppercutot, dash-t, és punchdown-t. Automatikusan meghívódik amikor a játékos földet ér. |
|`bool IsAttacking()`| - |
|`PlayerMover.EAttackType GetAttackType()`| Visszaadja, hogy milyen támadást végez jelenleg a player |

### Tippek

Ellenségek implementálásakor:
 - a sebző (trigger) colliderek (`PunchBox`) a Hitbox layer-en vannak, és csak Hitbox layerrel tudnak érintkezni (projektbeállítás), valamint `"attackbox"` tag van rajtuk
 - a player-en `"Player"` tag van
 - jelenlegi támadási state lekérése: `gameObject.GetComponent<PlayerMover>().IsAttacking()` és `.GetAttackType()`