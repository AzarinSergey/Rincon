1. Установить asp net core 2.2, docker, 
В VS2017 установщике доустановить поддержку ServiceFabric

2. загрузить образ с хранилищем
docker pull minio/minio

3. Запуск контейнера с хранилищем на порту 24001
параметру -v передать в значении пути для "устойчивого" хранения файлов хранилища (путь в параметре до ':')
docker run -p 24001:9000 --name minioServer -e "MINIO_ACCESS_KEY=AKIAIOSFODNN7EXAMPLE" -e "MINIO_SECRET_KEY=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY" -v E:\dataVolumes\minio:/data -v E:\dataVolumes\minio\config:/root/.minio minio/minio server /data 

4. запускаемых проекта 2: 
- Arches.Api (внешний сервис - акторы(на каждый вызов) для проведения расчетов. Нельзя запустить расчет с CalculationUuid рассчет по которому не завершен)
- Bismarck (бизнесовое приложение на микросервисах под управлением оркестратора ServiceFabric)


