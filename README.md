# iot-smarthome

SmartHeating to aplikacja oparta na .NET 9 i PostgreSQL, umożliwiająca monitorowanie temperatury w pomieszczeniu.

## ✨ Funkcje

- **Zbieranie danych** – Rejestrowanie temperatury w czasie rzeczywistym i zapisywanie jej w bazie danych.
- **Automatyczne sterowanie ogrzewaniem** – Włączanie i wyłączanie grzejnika na podstawie zadanych progów temperatury.
- **Analiza danych** – Obliczanie średniej temperatury w określonym przedziale czasowym.

## 🛠 Technologie

- **Backend:** .NET 9 (ASP.NET Core)
- **Baza danych:** PostgreSQL

## User stories
1. Utrzymanie komfortowej temperatury w pomieszczeniu:

JAKO użytkownik
CHCIAŁBYM  móc ustawić docelową temperaturę,
przy której grzejnik włączy się,
PO TO, aby podnieść temperaturę w pomieszczeniu do komfortowego poziomu, 
oraz temperaturę, przy której grzejnik automatycznie się wyłączy, aby nie przegrzewać pomieszczenia. 
Dzięki temu system grzewczy działałby w sposób efektywny, utrzymując stałą, komfortową temperaturę w pomieszczeniu, 
unikając jednocześnie zbędnego zużycia energii. Moje ustawienia powinny być elastyczne i umożliwiać łatwą regulację 
w zależności od pory dnia czy preferencji, zapewniając optymalny komfort cieplny.

2. Monitorowanie temperatury:

JAKO użytkownik
CHCIAŁBYM, aby temperatura była wyświetlana w czasie rzeczywistym,
PO TO, aby w razie potrzeby szybko zareagować i dostosować ustawienia grzejnika 
lub klimatyzatora, zapewniając optymalny komfort cieplny. Dodatkowo,
przydatne byłoby mieć możliwość sprawdzenia historii temperatury, 
aby analizować, jak zmieniała się temperatura w ciągu dnia lub tygodnia.

3. Historia temperatury:

JAKO użytkownik
CHCIAŁBYM Chciałbym mieć możliwość przeglądania historii zmian temperatury w pomieszczeniu,
PO TO, aby móc analizować, jak temperatura zmieniała się w różnych porach dnia i nocy. 
Analiza takich danych ułatwiłaby mi także planowanie, jak najlepiej wykorzystać system grzewczy 
w zależności od zmieniających się warunków w ciągu dnia lub tygodnia.

4. Obliczanie średniej temperatury:

JAKO użytkownik
CHCIAŁBYM móc zobaczyć średnią temperaturę z wybranego okresu
PO TO, ABY ocenić, jak efektywnie działa ogrzewanie

5. Obsługa różnych jednostek temperatury:

JAKO użytkownik
CHCIAŁBYM móc wybrać jednostkę temperatury (°C lub °F)
PO TO, ABY dostosować system do moich preferencji.

6. Oszczędzanie energii:

JAKO użytkownik
CHCIAŁBYM móc aktywować tryb oszczędzania energii
PO TO, ABY zmniejszyć zużycie energii, gdy nie ma mnie w pomieszczeniu.
