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
CHCIAŁBYM ustawić temperaturę dla włączenia i wyłączenia grzejnika
PO TO, ABY utrzymać komfortową temperaturę w pomieszczeniu.

2. Monitorowanie temperatury:

JAKO użytkownik
CHCIAŁBYM móc zobaczyć aktualną temperaturę w pomieszczeniu
PO TO, ABY kontrolować temperaturę w pomieszczeniu

3. Historia temperatury:

JAKO użytkownik
CHCIAŁBYM móc przeglądać historię zmian temperatury
PO TO, ABY analizować trendy i dostosować ustawienia grzejnika.

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
