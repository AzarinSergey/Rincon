1. 
 - Установить asp net core 2.2, docker, В VS2017 установщике доустановить поддержку ServiceFabric.
 - Изменить строку подключения к SQL серверу в константе Cmn.Constants.BismarckConsts.SqlServerConnection. На указанном сервере должна
 быть создана БД с именем как в строке подключения. У указанного в строке юзера должен быть доступ к этой базе. При первом запуске в БД будут созданы пустые таблицы очереди, контекста Pump и Clt.

2. загрузить образ с хранилищем
docker pull minio/minio

3. Запуск контейнера с хранилищем на порту 24001
параметру -v передать в значении пути для "устойчивого" хранения файлов хранилища (путь в параметре до ':')
docker run -p 24001:9000 --name minioServer -e "MINIO_ACCESS_KEY=AKIAIOSFODNN7EXAMPLE" -e "MINIO_SECRET_KEY=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY" -v E:\dataVolumes\minio:/data -v E:\dataVolumes\minio\config:/root/.minio minio/minio server /data 

4. запускаемых проекта 2: 
- Arches.Api (внешний сервис - акторы(на каждый вызов) для проведения расчетов. Нельзя запустить расчет с CalculationUuid рассчет по которому не завершен)
- Bismarck (бизнесовое приложение на микросервисах под управлением оркестратора ServiceFabric)

5. Для автономного запуска расчетов Pump (без участия бизнес приложения) 
- во внешнем сервисе в корне хранилища нужно создать папку "pump"
- в папку "pump" пометсить 2 файла с данными для рассчета:  ecn_vr{CalculationUuid}.csv и vx{CalculationUuid}.csv
- запустить проект Arches.Api 
- перейти по урлу http://localhost:5000/swagger
- выполнить запрос "Calc" со значением в параметре CalculationUuid соответствующим именам файлов в хранилище 
(ecn_vr{CalculationUuid}.csv и vx{CalculationUuid}.csv)
- результат рассчетов будет отправлен на указанные в параметрах запроса урлы, в зависимости от успешности
  "ErrorCallbackUrl": "http://localhost:5000/v1/pump/ping",
  "SuccessCallbackUrl": "http://localhost:5000/v1/pump/ping"

6. Если процесс консольного приложения рассчетов повиснет (это возникает когда консолине не удается выполнить код файлов .py)
 - убить процесс PyHyCarSim.exe диспетчером (это завершит работу актора)
 - очистить директорию по названию CalculationUuid от временных файлов Rincon/External/Arches/Arches.Api/ServiceStaticFiles/PumpService/{CalculationUuid}.




PS C:\Users\Sazarin> docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=123456zZ" -p 2402:1433 --name mssqlserver -v sqlvolume:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2019-CTP2.2-ubunt
5ab49e841a0387de35e2a150332148c3c7a309fccd092b1d2b432aaee0eb492a
PS C:\Users\Sazarin> docker container start mssqlserver -a
