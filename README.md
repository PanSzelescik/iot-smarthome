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
chciałbym móc wybrać jednostkę temperatury (°C, °F lub K)
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

### Azure Functions
1. Zaloguj się do portalu Azure i uruchom Cloud Shell.
2. Ustaw zmienne
```sh
let "randomIdentifier=$RANDOM*$RANDOM"
location="polandcentral"
resourceGroup="func-rg-$randomIdentifier"
storage="funcstorage$randomIdentifier"
plan="func-plan-$randomIdentifier"
functionApp="func-app-$randomIdentifier"
```
3. Utwórz grupę zasobów
```sh
az group create --name $resourceGroup --location $location
```
4. Utwórz Storage Account
```sh
az storage account create --name $storage --location $location --resource-group $resourceGroup --sku Standard_LRS
```
5. Utwórz plan App Service (Consumption Plan, Linux)
```sh
az functionapp plan create \
  --resource-group $resourceGroup \
  --name $plan \
  --location $location \
  --number-of-workers 1 \
  --sku Y1 \
  --is-linux
```
6. Utwórz Function App z obsługą .NET 9 (isolated)
```sh
az functionapp create \
   --resource-group $resourceGroup \
   --plan $plan \
   --runtime dotnet-isolated \
   --functions-version 4 \
   --name $functionApp \
   --storage-account $storage \
   --os-type Linux
```
7. Dodaj zmienne środowiskowe
```sh
az functionapp config appsettings set \
  --name $functionApp \
  --resource-group $resourceGroup \
  --settings "HOME_ASSISTANT_TOKEN=value" "IOTHUB_DEVICE_CONNECTION_STRING_POKOJ_SZYMONA=value" "IOTHUB_DEVICE_CONNECTION_STRING_SALON=value"
```
Gdzie `HOME_ASSISTANT_TOKEN` to token do Home Assistant (https://ha.panszelescik.pl/), a `IOTHUB_DEVICE_CONNECTION_STRING_POKOJ_SZYMONA` i `IOTHUB_DEVICE_CONNECTION_STRING_SALON` to Connection Stringi odpowiednio dla urządzeń o Device ID `pokoj_szymona` i `salon` z Azure IoT Hub.
8. Kod symulatora znajduje się w katalogu `simulator`, otwórz solucję za pomocą Ridera (upewnij się, że masz zainstalowany .NET 9 i plugin Azure Toolkit for Rider)
9. Kliknij prawym na solucję i wybierz Publish, a następnie Azure
![rider64_uKdt1Xg8JZ](https://github.com/user-attachments/assets/fc68e2ed-4b38-4d14-8136-f46c02d19fa1)
![rider64_dw6V8IMdtg](https://github.com/user-attachments/assets/86c585c0-e882-4274-8774-8383d39bf5cc)
10. Wybierz utworzony wcześniej Function App (w razie potrzeby zaloguj się) i kliknij Next aby zbudować solucję i ją wysłać do Azure
![rider64_CERAjBeFbD](https://github.com/user-attachments/assets/3e026bc8-35fc-46fb-b140-bc45301a1c19)
11. Symulator co 15 minut będzie pobierał prawdziwą temperaturę z Home Assistant i przesyłał je jako `pokoj_szymona` i `salon`. W razie potrzeby testowania możesz użyć ręcznych funkcji: https://iotsmarthomesimulator.azurewebsites.net/api/pokoj_szymona?temperature=23.48&code=value gdzie:
- pokoj_szymona to Device ID urządzenia
- temperature to temperatura w stopniach Celsjusza (pamiętaj o kropce, nie przecinku)
- code to klucz zabezpieczający funkcję, który znajdziesz w Functions > Keys > _master
![brave_uotyTGmp3N](https://github.com/user-attachments/assets/9551eeae-e243-4648-a4e3-ab9bab9b875e)

### Azure App Service
1. Zaloguj się do portalu Azure i uruchom Cloud Shell.
2. Ustaw zmienne
```sh
let "randomIdentifier=$RANDOM*$RANDOM"
location="polandcentral"
resourceGroup="appservice-rg-$randomIdentifier"
appServicePlan="appservice-plan-$randomIdentifier"
webAppName="appservice-webapp-$randomIdentifier"
```
3. Utwórz grupę zasobów
```sh
az group create --name $resourceGroup --location $location
```
4. Utwórz plan App Service
```sh
az appservice plan create \
  --name $appServicePlan \
  --resource-group $resourceGroup \
  --location $location \
  --sku B1 \
  --is-linux
```
5. Utwórz Web App
```sh
az webapp create \
  --resource-group $resourceGroup \
  --plan $appServicePlan \
  --name $webAppName \
  --runtime "DOTNETCORE|9.0" \
  --deployment-local-git
```
6. Skonfiguruj zmienne środowiskowe
```sh
az webapp config appsettings set \
  --resource-group $resourceGroup \
  --name $webAppName \
  --settings "ConnectionStrings__Postgres=value" "ConnectionStrings__EventHubName=value" "ConnectionStrings__EventHubCompatibleConnectionString=value" "ConnectionStrings__IoTHubConnectionString=value"
```
Gdzie ConnectionString `Postgres` to Connection String do bazy danych PostgreSQL, `EventHubName` to Event Hub-compatible name z Azure IoT Hub, `EventHubCompatibleConnectionString` to Event Hub-compatible endpoint z Azure IoT Hub, aby móc odbierać dane, a `IoTHubConnectionString` to Connection String do IoT Hub, aby móc wysyłać dane.
7. Kod wdrożysz identycznie jak w przypadku Azure Functions, czyli otwierając solucję w Riderze znajdującą się w folderze api i publikując ją do Azure. Wybierz utworzony wcześniej Web App i kliknij Next aby zbudować solucję i ją wysłać do Azure
