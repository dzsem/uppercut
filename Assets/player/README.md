# Player

### Rendering
A játékos a default layer-en rajzolódik, az orderben a 10. (hogy a platformok előtt jelenjen meg)

### Hitboxok
A player 5 collider-ből áll:
 - a rigidbody collidere (Friction physics material-lal) [default layer]
 - az átesős platformok érzékelésére szolgáló hitbox (másik hitbox a Player objektumon) [default layer]
 - a két ütésre szolgáló box (felfelé- és jobbra) [hitbox layer]
 - az a hitbox, amin keresztül a damage-t kapjuk (`"AttackableHitbox"`) [hitbox layer]

A vízszintes hitbox invertálódik inputtól függően (balra-jobbra néz); a függőleges hasonlóan (alapból fel, punchdown-nál le).

Az ütésre szolgáló hitboxok `attackbox` tag-esek, a megüthetőek pedig `attackable`. 

Mindig csak a jelenlegi támadás hitboxainak gameObject-jei aktívak.

> [!WARNING]
> Minden cucc, amit "Default" layerre raksz és trigger, be fog kavarni a ground detection-be.
> Amit oda teszel és nem trigger, az a fizikába.
>
> **TODO: valahogy egységesen kezelni**

### Movement (`PlayerMover`)

Az alap movement ability-k:

|Ability|Bind + követelmények|Cooldown?|
|:------|:-------------------|:--------|
|Uppercut|`SPACE` + földön|nincs|
|Dash|(`←→AD`) + `X` + nincs cooldown-on + két földetérés között csak egyszer|van|
|Smash/Punchdown|`↓S` + `X` + levegőben + két földetérés között csak egyszer|nincs|

#### Interface
|Függvény/Field|Absztrakt|
|:-------------|:--------|
|`bool disableInput`| Kikapcsolja a player inputot, ha igaz. Cutscenekhez / pause-hoz hasznos lehet. |
|`void RefreshUppercut()`| - (alapból csak akkor hívódik meg, ha földet ér a játékos) |
|`void RefreshDash()`| - (alapból csak akkor hívódik meg, ha földet ér a játékos) |
|`void RefreshMovementAbilities()`| Refresheli az uppercutot, dash-t, és punchdown-t. Automatikusan meghívódik amikor a játékos földet ér. |
|`bool IsAttacking()`| - |
|`PlayerMover.EAttackType GetAttackType()`| Visszaadja, hogy milyen támadást végez jelenleg a player |

#### Tippek

Ellenségek implementálásakor:
 - a sebző (trigger) colliderek (`PunchBox`) a Hitbox layer-en vannak, és csak Hitbox layerrel tudnak érintkezni (projektbeállítás), valamint `"attackbox"` tag van rajtuk
 - a player-en `"Player"` tag van
 - jelenlegi támadási state lekérése: `gameObject.GetComponent<PlayerMover>().IsAttacking()` és `.GetAttackType()`

### HP és knockback (`PlayerHealth`)

#### Interface
|Függvény/Field|Absztrakt|
|:-------------|:--------|
|`int Health`| ezen keresztül tudod állítani a HP-t, és ez kiadja a megfelelő `onDamage` vagy `onDeath` eventet |
|`int maxHealth`| - |
|`bool forceInvulnerability`| biztosítja, hogy a játékos a damage system-en keresztül ne lehessen hátralökhető / sebezhető |
|`float knockbackResistanceStrength`| ellenállás knockback-re. Ez a nehézség scalingjére használható. Knockback kiszámításáért lásd a [Knockback](#Knockback) részt. |
|`UnityEvent onDeath`| - |
|`UnityEvent<int> onDamage`| akkor hívódik, amikor a HP csökken. Paramétere a sebzés mértéke (pozitív). |
|`void KnockbackInDirection(Vector2 dir, float strength)`| Knockback kiszámításáért lásd a [Knockback](#Knockback) részt. |

#### Knockback
Knockback kiszámításakor meg kell adni az ellökés irányát (ált. player.pos - enemy.pos normalizálva), és az erősségét, ami az enemy tulajdonsága.

A hátralökés erejének kiszámítása:
$\overline F = \overline{\text{normalizált irány}} \cdot \text{knockback ereje} \cdot \max(\text{minimál knockback faktor},\;\text{enemy knockback} - \text{knockback resistance})$, ahol:
 - a knockback ereje és a minimál knockback factor a PlayerHealth tulajdonsága, kb globális
 - az enemy knockback az érintkező enemy tulajdonsága (KnockbackInDirection `strength` param.)
 - az irány az ellökés iránya normalizálva (ált. $\hat d = norm(\overline{\text{pos}}_\text{player} - \overline{\text{pos}}_\text{enemy})$)

Tehát minél nagyobb a resistance, annál kevésbé lökődik vissza a player ugyanolyan enemytől.

#### Invulnerability
Amikor a player belesétál egy enemybe, garantáltan kap `invulnerabilityDuration`-nyi időt, amég nem lökődhet el, és nem sebződhet.
Ezt lehet ismételni, tehát ha rátapadsz egy enemy-re és kellően nagy az invulnerability duration, akkor gyakorlatilag végtelen invuln-t kapsz.
Ezért fontos a knockback és az invuln idő rendes beállítása.