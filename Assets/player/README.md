# Player

### Hitboxok
A player 4 collider-ből áll:
 1. a rigidbody collidere (Friction physics material-lal) [default layer]
 2. az átesős platformok érzékelésére szolgáló hitbox (másik hitbox a Player objektumon) [default layer]
 3. a két ütésre szolgáló box (felfelé- és jobbra) [hitbox layer]

A vízszintes hitbox invertálódik inputtól függően (balra-jobbra néz); a függőleges még nem - TODO.
