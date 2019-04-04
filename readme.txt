//загрузить образ с хранилищем
docker pull minio/minio

//Запуск контейнера с хранилищем на порту 24001
//параметру -v передать в значении пути для "устойчивого" хранения файлов хранилища (путь в параметре до ':')
docker run -p 24001:9000 --name minioServer -e "MINIO_ACCESS_KEY=AKIAIOSFODNN7EXAMPLE" -e "MINIO_SECRET_KEY=wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY" -v E:\dataVolumes\minio:/data -v E:\dataVolumes\minio\config:/root/.minio minio/minio server /data 


