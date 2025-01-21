# Átesős platform (fallthrough / open floor)

Hogy rakd össze:
 1. csináld meg magát a platformot, tegyél rá Box Collider 2D komponenst
 2. tag-ként kattintsd be az `openFloor`-t
 3. "átengedő collider" létrehozása: hozz létre benne egy üres GameObject-et, és rendelj hozzá egy Box Collider 2D-t; ez lesz az a collider, ami megengedi hogy alulról keresztülugorj a platformon

Ha felülről érkezel, az átengedő collider tetejéig fogsz esni, amikor a láb hitboxod eléri, visszakapcsolódik a platform objektumon (amit `openFloor`-ral jelöltél) a collision, és vissza fogsz snap-pelni a platform tetejére.

Ha az átengedő collider nincs elég mélyen, akkor esélyes hogy nem kapsz a platformról jump-ot.