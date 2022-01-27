FROM alpine
ENV PROFILE 'default'
ENV ACCESS_KEY_ID '123'
ENV ACCESS_KEY_SECRET '123'
ENV COMMAND 'download'

RUN mkdir /tmp/s3-uploader/
COPY ./run.sh /tmp/s3-uploader/
COPY ./s3-util.py /tmp/s3-uploader/

RUN apk add --no-cache \
        python3 \
        py3-pip \
    && pip3 install --upgrade pip \
    && pip3 install --no-cache-dir \
        awscli \
    && rm -rf /var/cache/apk/*

RUN pip3 install boto3

RUN aws --version
WORKDIR /tmp/s3-uploader/

CMD export AWS_SECRET_ACCESS_KEY=${ACCESS_KEY_SECRET} && export AWS_ACCESS_KEY_ID=${ACCESS_KEY_ID} && sh run.sh ${COMMAND}
