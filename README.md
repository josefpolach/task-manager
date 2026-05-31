# Spravce ukolu (Task List Manager)

Konzolova aplikace v .NET 10 pro spravu seznamu ukolu (to-do list)
s ukladanim do CSV souboru.

---

## Funkce programu

Po spusteni se zobrazi menu s temito volbami:

| Klavesa | Akce                                  |
|---------|---------------------------------------|
| `v`     | Vypsat vsechny ukoly                  |
| `p`     | Pridat novy ukol                      |
| `h`     | Oznacit ukol jako splneny             |
| `m`     | Vymazat ukol                          |
| `f`     | Filtrovat ukoly (splnene / nesplnene) |
| `k`     | Ulozit do souboru a ukoncit program   |

Kazdy ukol ma ctyri udaje:

- **id** &mdash; poradove cislo ukolu (cele cislo)
- **popis** &mdash; textovy popis
- **termin** &mdash; datum ve formatu `RR-MM-DD`
- **stav** &mdash; `open` (nesplneny) nebo `done` (splneny)

---

## Datovy soubor

Ukoly jsou ulozeny v souboru `taskList.csv`. Jeden ukol = jeden radek,
udaje oddelene strednikem:

```
1; Nakoupit potraviny; 26-05-30; open
2; Dokoncit projekt z C#; 26-06-15; open
3; Zavolat zubari; 26-05-22; done
```

Pokud soubor neexistuje, program zacne s prazdnym seznamem a soubor
vytvori pri prvnim ulozeni.

---

## Spusteni

Predpoklad: nainstalovany .NET 10 SDK.

```bash
git clone <url-repozitare>
cd programovaniZaverecnyProjekt
dotnet run
```

---

## Jak to funguje

### Atomicke ukladani

Pri ukladani se data nejprve zapisi do docasneho souboru
`taskList.csv.tmp`, a teprve po uspesnem dokonceni se prejmenuji
na finalni `taskList.csv` (`File.Move` s `overwrite: true`). Pripadny
pad programu uprostred zapisu tedy nemuze poskodit puvodni data &mdash;
v nejhorsim pripade zustane orphan `.tmp` soubor, ktery se pri
priste prepise.

### Tolerantni nacitani

Nacitani CSV souboru je odolne proti chybam &mdash; prazdne radky
a poskozene zaznamy (spatny pocet poli oddelenych strednikem, id
ktere neni cislo) se preskoci a nahlasi na konzoli. Pripadna chyba
souboroveho systemu se zachyti pres `IOException`. Pokud soubor
jeste neexistuje (prvni spusteni), program startuje s prazdnym
seznamem.

### Validace vstupu

Vsechny vstupy od uzivatele prochazi pres pomocne funkce, takze
do programu se nikdy nedostane rozbita hodnota:

- `NactiNeprazdnyText()` opakuje dotaz, dokud uzivatel nezada
  neprazdny text.
- `NactiCislo()` pouziva `int.TryParse`, takze pismena ani prazdne
  vstupy program neshodi.
- `NactiDatum()` overuje pres `DateTime.TryParseExact` jak format
  `RR-MM-DD`, tak platnost data (napr. `26-13-40` je odmitnuto).

Volby v menu a ve filtru jsou take osetrene &mdash; neznama klavesa
spadne do vetve `default` s upozornenim.

### Struktura kodu

Hlavni smycka je `do/while` se `switch` na stisknutou klavesu, kazda
volba menu deleguje praci na samostatnou funkci. Datovy model
predstavuje `struct TUkol` se ctyrmi poli (id, popis, terminText,
stav). Funkce jsou rozdelene do logickych skupin (vstupy &mdash; operace
s ukoly &mdash; souborove I/O).

---

## Autor

Josef Polach &mdash; m@josefpolach.cz  
[github.com/josefpolach](https://github.com/josefpolach)