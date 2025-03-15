# iot-smarthome

SmartHeating to aplikacja oparta na .NET 9 i PostgreSQL, umożliwiająca monitorowanie temperatury w pomieszczeniu.

## ✨ Funkcje

- **Zbieranie danych** – Rejestrowanie temperatury w czasie rzeczywistym i zapisywanie jej w bazie danych.
- **Automatyczne sterowanie ogrzewaniem** – Włączanie i wyłączanie grzejnika na podstawie zadanych progów temperatury.
- **Analiza danych** – Obliczanie średniej temperatury w określonym przedziale czasowym.

## 🛠 Technologie

- **Backend:** .NET 9 (ASP.NET Core)
- **Baza danych:** PostgreSQL

## 🙋‍♂️ User stories
1. Utrzymanie komfortowej temperatury w pomieszczeniu:

Jako użytkownik
chciałbym móc ustawić docelową temperaturę,
przy której grzejnik włączy się,
po to, aby podnieść temperaturę w pomieszczeniu do komfortowego poziomu, 
oraz temperaturę, przy której grzejnik automatycznie się wyłączy, aby nie przegrzewać pomieszczenia. 
Dzięki temu system grzewczy działałby w sposób efektywny, utrzymując stałą, komfortową temperaturę w pomieszczeniu, 
unikając jednocześnie zbędnego zużycia energii. Moje ustawienia powinny być elastyczne i umożliwiać łatwą regulację 
w zależności od pory dnia czy preferencji, zapewniając optymalny komfort cieplny.

2. Monitorowanie temperatury:

Jako użytkownik
chciałbym, aby temperatura była wyświetlana w czasie rzeczywistym,
po to, aby w razie potrzeby szybko zareagować i dostosować ustawienia grzejnika 
lub klimatyzatora, zapewniając optymalny komfort cieplny. Dodatkowo,
przydatne byłoby mieć możliwość sprawdzenia historii temperatury, 
aby analizować, jak zmieniała się temperatura w ciągu dnia lub tygodnia.

3. Historia temperatury:

Jako użytkownik
chciałbym Chciałbym mieć możliwość przeglądania historii zmian temperatury w pomieszczeniu,
po to, aby móc analizować, jak temperatura zmieniała się w różnych porach dnia i nocy. 
Analiza takich danych ułatwiłaby mi także planowanie, jak najlepiej wykorzystać system grzewczy 
w zależności od zmieniających się warunków w ciągu dnia lub tygodnia.

4. Obliczanie średniej temperatury:

Jako użytkownik
chciałbym móc zobaczyć średnią temperaturę z wybranego okresu
po to, aby ocenić, jak efektywnie działa ogrzewanie

5. Obsługa różnych jednostek temperatury:

Jako użytkownik
chciałbym móc wybrać jednostkę temperatury (°C lub °F)
po to, aby dostosować system do moich preferencji.

6. Oszczędzanie energii:

Jako użytkownik
chciałbym móc aktywować tryb oszczędzania energii
po to, aby zmniejszyć zużycie energii, gdy nie ma mnie w pomieszczeniu.

## 💰 Cost Calculator
![Azure_Cost_Calculator](https://github.com/PanSzelescik/iot-smarthome/blob/main/Azure_Cost_Calculator.png)


