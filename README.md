# iot-smarthome

SmartHeating to aplikacja oparta na .NET 9 i PostgreSQL, umożliwiająca monitorowanie temperatury w pomieszczeniu.

https://iotsmarthomeapi.azurewebsites.net/

OpenAPI/Postman collection: https://iotsmarthomeapi.azurewebsites.net/swagger/v1/swagger.json

## ✨ Funkcje

- **Zbieranie danych** – Rejestrowanie temperatury w czasie rzeczywistym i zapisywanie jej w bazie danych.
- **Automatyczne sterowanie ogrzewaniem** – Włączanie i wyłączanie grzejnika czy klimy na podstawie zadanych progów temperatury.
- **Analiza danych** – Obliczanie średniej temperatury w określonym przedziale czasowym, maksymalnej, minimalnej, przeliczanie na różne jednostki.

## 🛠 Technologie

- **Backend:** .NET 9 (ASP.NET Core)
- **Baza danych:** PostgreSQL

## 🙋‍♂️ User stories
1. Utrzymanie komfortowej temperatury w pomieszczeniu:

Jako użytkownik
chciałbym móc ustawić docelową temperaturę,
przy której grzejnik włączy się,
po to, aby podnieść temperaturę w pomieszczeniu do komfortowego poziomu (lub klimę, aby obliżyć), 
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
chciałbym mieć możliwość przeglądania historii zmian temperatury w pomieszczeniu,
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

## 📊 Diagram architektury - C4 model

![image](https://github.com/user-attachments/assets/f20d7679-d5e1-4fd7-8d2a-66174d13a9cd)

![image](https://github.com/user-attachments/assets/844c5a89-de03-4431-ad48-e0a62d190b5e)

## 💰 Cost Calculator
![Azure_Cost_Calculator](https://github.com/PanSzelescik/iot-smarthome/blob/main/Azure_Cost_Calculator.png)

## 🚨 Instalacja

### Azure Database for PostgreSQL
1. Zaloguj się do portalu Azure i uruchom Cloud Shell.
2. Ustaw zmienne
```sh
let "randomIdentifier=$RANDOM*$RANDOM"
location="polandcentral"
resourceGroup="pg-rg-$randomIdentifier"
server="pg-server-$randomIdentifier"
sku="Standard_D2ds_v4"
login="azureuser"
password="Pa$$w0rD-$randomIdentifier"
startIp=0.0.0.0
endIp=0.0.0.0
```
3. Utwórz grupę zasobów
```sh
az group create --name $resourceGroup --location $location
```
4. Utwórz serwer PostgreSQL Flexible Server
```sh
az postgres flexible-server create \
  --name $server \
  --resource-group $resourceGroup \
  --location $location \
  --admin-user $login \
  --admin-password $password \
  --sku-name $sku
```
5. Skonfiguruj regułę zapory, aby zezwolić na dostęp z dowolnego adresu IP (zrób to tylko dla serwera testowego, w produkcji należy ograniczyć dostęp do określonych adresów IP)
```sh
az postgres flexible-server firewall-rule create \
  --resource-group $resourceGroup \
  --name AllowAll \
  --server-name $server \
  --start-ip-address $startIp \
  --end-ip-address $endIp
```
6. Pobierz Connection Stringa wybierając serwer bazy danych na Azure > Settings > Connect. Wybierz odpowiedni database, Authentication Method jako PostgreSQL. Następnie rozwiń Connect from your app i skopiuj ADO.NET
![image](https://github.com/user-attachments/assets/cd5f564d-8f77-4bd2-b91b-f3439f020308)

### Azure IoT Hub
1. Zaloguj się do portalu Azure i uruchom Cloud Shell.
2. Zainstaluj rozszerzenie IoT dla Azure CLI
```sh
az extension add --upgrade --name azure-iot
```
3. Ustaw zmienne
```sh
location="polandcentral"
resourceGroup="iot-rg"
iotHubName="my-iothub-$(openssl rand -hex 3)"
```
4. Utwórz grupę zasobów
```sh
az group create --name $resourceGroup --location $location
```
5. Utwórz IoT Hub
```sh
az iot hub create --resource-group $resourceGroup --name $iotHubName --location $location --sku S1
```
6. Pobierz Connection Stringa, aby API mogło wysyłać dane do urządzeń:
```sh
az iot hub show-connection-string --name iotsmarthomeiothub --resource-group ProjektIotHub --output table
```
7. Przejdź do IoT Hub w portalu Azure > Devices i kliknij Add Device aby dodać nowe urządzenie
![brave_NUq6FX3zQV](https://github.com/user-attachments/assets/f33ee14d-0cb7-4984-a6d7-26216c776a4b)
8. Ustaw tylko Device ID i kliknij Save
![brave_2pgrBNp3br](https://github.com/user-attachments/assets/bba2e1d9-0413-4ddb-8a2d-986e9f3223cc)
9. Następnie kliknij na nowo utworzone urządzenie i skopiuj Connection Stringa, który będzie potrzebny do konfiguracji urządzenia
![brave_tIA8AyZOyw](https://github.com/user-attachments/assets/9a127946-47e8-4c4e-b501-be725c977831)
![brave_kNCwXz8YPU](https://github.com/user-attachments/assets/20c2618a-e573-4aae-9b5c-3282dfb9aab3)
10. Przejdź do zakładki Hub settings > Built-in endpoints i skopiuj Event Hub-compatible name oraz Event Hub-compatible endpoint
![image](https://github.com/user-attachments/assets/943e8d80-c6d7-4c2a-815b-a95649d143d2)
![image](https://github.com/user-attachments/assets/cee824f2-843d-47dc-9f00-aa8a179eea5b)
