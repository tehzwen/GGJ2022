#!/bin/sh

if [[ $1 == "upload" ]]; then
    python3 s3-util.py --upload
else
    python3 s3-util.py --download
fi
