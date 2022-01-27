#!/bin/bash

COMMAND="download"
ACCESS_KEY_ID=""
ACCESS_KEY_SECRET=""

docker container run \
--mount type=bind,source="$(pwd)"/DownloadedAssets,target=/tmp/s3-uploader/DownloadedAssets \
--mount type=bind,source="$(pwd)"/UploadAssets,target=/tmp/s3-uploader/UploadAssets \
-e ACCESS_KEY_ID=${ACCESS_KEY_ID} \
-e ACCESS_KEY_SECRET=${ACCESS_KEY_SECRET} \
-e COMMAND=${COMMAND} \
--rm gamejamuploader:latest
