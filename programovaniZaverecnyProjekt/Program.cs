using System.Globalization;

// pro lepsi orientaci, pouzijeme metodu GetFullPath, aby uzivatel programu vedel, kde je ulozen soubor s ukoly
string taskList = Path.GetFullPath("/home/josef/deployment/czu/programovani-c-sh/programovaniZaverecnyProjekt/programovaniZaverecnyProjekt/taskList.csv");

// Pri startu programu rovnou nacteme ukoly ze souboru.
List<TUkol> ukoly = Nacti();

char odpoved;
do
{
    Console.Clear();
    Console.WriteLine("Spravce ukolu - MENU");
    Console.WriteLine("------------------------");
    Console.WriteLine("Vypsat vsechny ukoly [v]");
    Console.WriteLine("Pridat ukol          [p]");
    Console.WriteLine("Oznacit jako hotovy  [h]");
    Console.WriteLine("Vymazat ukol         [m]");
    Console.WriteLine("Filtrovat ukoly      [f]");
    Console.WriteLine("Ulozit a ukoncit     [k]");
    Console.Write("Zadejte akci: ");
    odpoved = char.ToLower(Console.ReadKey().KeyChar);

    switch (odpoved)
    {
        case 'v':
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Vypis ukolu");
            Console.WriteLine("------------------------");
            VypisVsechny(ukoly);
            Console.WriteLine("\nStisknete klavesu pro navrat do menu...");
            Console.ReadKey();
            break;

        case 'p':
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Pridani ukolu");
            Console.WriteLine("------------------------");
            ukoly.Add(Pridat(DalsiId(ukoly)));
            Console.WriteLine("Ukol byl pridan.");
            Console.ReadKey();
            break;

        case 'h':
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Oznaceni splneneho ukolu");
            Console.WriteLine("------------------------");
            VypisVsechny(ukoly);
            OznacJakoHotovy(ukoly, NactiCislo("Zadejte id ukolu: "));
            Console.ReadKey();
            break;

        case 'm':
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Vymazani ukolu");
            Console.WriteLine("------------------------");
            VypisVsechny(ukoly);
            Vymazat(ukoly, NactiCislo("Zadejte id ukolu: "));
            Console.ReadKey();
            break;

        case 'f':
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Filtrovani ukolu");
            Console.WriteLine("------------------------");
            Console.WriteLine("Zobrazit nesplnene [o] nebo splnene [d]?");
            Console.Write("Zadejte volbu: ");
            char volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();
            // Osetrime, ze uzivatel zadal jen ocekavane volby.
            if (volba == 'o')
                Filtruj(ukoly, "open");
            else if (volba == 'd')
                Filtruj(ukoly, "done");
            else
                Console.WriteLine("Neznama volba filtru.");
            Console.ReadKey();
            break;

        case 'k':
            // Pri ukonceni ulozime aktualni stav do souboru.
            Console.Clear();
            Console.WriteLine("Spravce ukolu - Ukladani a ukonceni");
            Console.WriteLine("------------------------");
            Uloz(ukoly);
            Console.WriteLine("Program se ukoncuje.");
            Console.ReadKey();
            break;

        case 'e':
            Console.Clear();
            Console.WriteLine("  *    .  .       .        *    .");
            Console.WriteLine("     .      *   .    *  .      .");
            Console.WriteLine("  .    *  .   .    .   *    .    *");
            Console.WriteLine();
            Console.WriteLine("         TAJNY EASTER EGG!");
            Console.WriteLine();
            Console.WriteLine("  Gratulujeme! Nasli jste skryty pozdrav.");
            Console.WriteLine("  Dekuji za pozitivni hodnoceni a preji Vam krasny den!");
            Console.WriteLine();
            Console.WriteLine("  *    .  .       .        *    .");
            Console.WriteLine("     .      *   .    *  .      .");
            Console.WriteLine("  .    *  .   .    .   *    .    *");
            Console.WriteLine();
            Console.WriteLine("Stisknete klavesu pro navrat do menu...");
            Console.ReadKey();
            break;

        default:
            Console.WriteLine("\nSpatna volba!");
            Console.ReadKey();
            break;
    }
} while (odpoved != 'k');



// POMOCNE FUNKCE PRO BEZPECNE NACITANI VSTUPU OD UZIVATELE

// Osetreny vstup c.1: nacteni cele radky textu, ktera nesmi byt prazdna.
string NactiNeprazdnyText(string vyzva)
{
    string? text;
    do
    {
        Console.Write(vyzva);
        text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text))
            Console.WriteLine("Vstup nesmi byt prazdny, zkuste to znovu.");
    } while (string.IsNullOrWhiteSpace(text));
    return text.Trim();
}

// Osetreny vstup c.2: nacteni cele cisla.
// int.TryParse nespadne pri nesmyslnem vstupu, jen vrati false.
int NactiCislo(string vyzva)
{
    int cislo;
    while (true)
    {
        Console.Write(vyzva);
        if (int.TryParse(Console.ReadLine(), out cislo))
            return cislo;
        Console.WriteLine("Zadejte prosim platne cele cislo.");
    }
}

// Osetreny vstup c.3: nacteni data ve formatu RR-MM-DD.
// Kontrolujeme presny format i platnost data (napr. neexistuje 26-13-40).
string NactiDatum(string vyzva)
{
    DateTime datum;
    while (true)
    {
        Console.Write(vyzva);
        if (DateTime.TryParseExact(Console.ReadLine(), "yy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out datum))
            return datum.ToString("yy-MM-dd");
        Console.WriteLine("Zadejte datum ve formatu RR-MM-DD (napr. 26-06-15).");
    }
}


// ===================================================================
// FUNKCE PRO PRACI S UKOLY
// ===================================================================

// Vytvori novy ukol podle vstupu od uzivatele a vrati ho.
TUkol Pridat(int noveId)
{
    TUkol ukol;
    ukol.id = noveId;
    ukol.popis = NactiNeprazdnyText("Zadejte popis ukolu: ");
    ukol.terminText = NactiDatum("Zadejte termin (RR-MM-DD): ");
    ukol.stav = "open"; // novy ukol je vzdy nesplneny
    return ukol;
}

// Vypise jeden ukol na jeden radek.
void VypisUkol(TUkol ukol)
{
    Console.WriteLine("{0}\t {1}\t {2}\t {3}",
        ukol.id, ukol.popis, ukol.terminText, ukol.stav);
}

// Vypise hlavicku tabulky a vsechny ukoly ze seznamu.
void VypisVsechny(List<TUkol> seznam)
{
    if (seznam.Count == 0)
    {
        Console.WriteLine("Seznam ukolu je prazdny.");
        return;
    }
    Console.WriteLine("ID\t Popis\t\t Termin\t\t Stav");
    Console.WriteLine("------------------------------------------------");
    foreach (TUkol ukol in seznam)
        VypisUkol(ukol);
}

// Najde index ukolu se zadanym id. Vraci -1, kdyz ukol neexistuje.
int NajdiIndexPodleId(List<TUkol> seznam, int id)
{
    for (int i = 0; i < seznam.Count; i++)
        if (seznam[i].id == id)
            return i;
    return -1;
}

// Oznaci ukol se zadanym id jako splneny.
// Struct je hodnotovy typ, proto si ho vytahneme, zmenime a vlozime zpet.
void OznacJakoHotovy(List<TUkol> seznam, int id)
{
    int index = NajdiIndexPodleId(seznam, id);
    if (index == -1)
    {
        Console.WriteLine("Ukol s timto id neexistuje.");
        return;
    }
    TUkol ukol = seznam[index];
    ukol.stav = "done";
    seznam[index] = ukol;
    Console.WriteLine("Ukol byl oznacen jako splneny.");
}

// Smaze ukol se zadanym id ze seznamu.
void Vymazat(List<TUkol> seznam, int id)
{
    int index = NajdiIndexPodleId(seznam, id);
    if (index == -1)
    {
        Console.WriteLine("Ukol s timto id neexistuje.");
        return;
    }
    seznam.RemoveAt(index);
    Console.WriteLine("Ukol byl smazan.");
}

// Filtruje ukoly podle stavu a vypise jen ty odpovidajici.
void Filtruj(List<TUkol> seznam, string stav)
{
    // LINQ Where vybere jen ukoly se zadanym stavem.
    List<TUkol> vybrane = seznam.Where(u => u.stav == stav).ToList();
    if (vybrane.Count == 0)
    {
        Console.WriteLine("Zadne ukoly s timto stavem.");
        return;
    }
    VypisVsechny(vybrane);
}

// Vrati nove id = nejvetsi pouzite id + 1 (nebo 1, kdyz je seznam prazdny).
int DalsiId(List<TUkol> seznam)
{
    int max = 0;
    foreach (TUkol ukol in seznam)
        if (ukol.id > max)
            max = ukol.id;
    return max + 1;
}



// NACITANI A UKLADANI SOUBORU (bezchybne)

// Nacte ukoly ze souboru. Kdyz soubor neexistuje, vrati prazdny seznam.
// Poskozene radky preskoci a nahlasi, aby program nespadl.
List<TUkol> Nacti()
{
    List<TUkol> seznam = new List<TUkol>();

    // Kdyz soubor jeste neexistuje, zacneme s prazdnym seznamem.
    if (!File.Exists(taskList))
        return seznam;

    try
    {
        string[] radky = File.ReadAllLines(taskList);
        int cisloRadku = 0;
        foreach (string radek in radky)
        {
            cisloRadku++;

            // Prazdne radky proste preskocime.
            if (string.IsNullOrWhiteSpace(radek))
                continue;

            // Radek rozdelime podle strednika - format: id; popis; termin; stav
            string[] casti = radek.Split(';');
            if (casti.Length != 4)
            {
                Console.WriteLine("Preskocen poskozeny radek c.{0} (spatny pocet poli).", cisloRadku);
                continue;
            }

            // id musi byt cele cislo, jinak radek preskocime.
            if (!int.TryParse(casti[0].Trim(), out int id))
            {
                Console.WriteLine("Preskocen poskozeny radek c.{0} (id neni cislo).", cisloRadku);
                continue;
            }

            TUkol ukol;
            ukol.id = id;
            ukol.popis = casti[1].Trim();
            ukol.terminText = casti[2].Trim();
            ukol.stav = casti[3].Trim();
            seznam.Add(ukol);
        }
    }
    catch (IOException e)
    {
        // Kdyby se soubor nepodarilo precist, nahlasime a vratime, co mame.
        Console.WriteLine("Chyba pri cteni souboru: " + e.Message);
    }

    return seznam;
}

// Ulozi vsechny ukoly do souboru.
// Nejprve zapiseme do docasneho souboru a teprve potom ho prejmenujeme,
// aby se pri pripadne chybe behem zapisu neposkodila puvodni data.
void Uloz(List<TUkol> seznam)
{
    string docasny = taskList + ".tmp";
    try
    {
        List<string> radky = new List<string>();
        foreach (TUkol ukol in seznam)
            // Stejny format jako pri nacitani: id; popis; termin; stav
            radky.Add($"{ukol.id}; {ukol.popis}; {ukol.terminText}; {ukol.stav}");

        File.WriteAllLines(docasny, radky);

        // Az kdyz zapis probehl v poradku, nahradime puvodni soubor.
        File.Move(docasny, taskList, overwrite: true);

        Console.WriteLine("Ulozeno do souboru " + taskList);
    }
    catch (IOException e)
    {
        Console.WriteLine("Chyba pri ukladani souboru: " + e.Message);
    }
}


// DATOVA STRUKTURA
// Struktura jednoho ukolu - drzi vsechna data o jednom ukolu.
struct TUkol
{
    public int id;             // poradove cislo ukolu
    public string popis;       // textovy popis ukolu
    public string terminText;  // termin ve formatu RR-MM-DD (jako text)
    public string stav;        // "open" nebo "done"
}