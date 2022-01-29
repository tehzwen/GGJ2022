import boto3
import argparse
import os


class AssetHandler:
    def __init__(self, bucket_name, profile_name, cli=False):
        self._bucket_name = bucket_name

        if (cli):
            self.client = boto3.client('s3')
        else:
            self.session = boto3.Session(profile_name=profile_name)
            self.client = self.session.client('s3')
        self.download_folder = "DownloadedAssets"
        self.upload_folder = "UploadAssets"

    def download_contents(self):
        objects = self.client.list_objects(
            Bucket=self._bucket_name, Prefix="Assets/")
        print(objects['Contents'])

        for object in objects['Contents']:
            if object['Key'] == 'Assets/':
                continue
            else:
                filename = object['Key'].split('/')[-1]
                self.client.download_file(
                    self._bucket_name, object['Key'], self.download_folder + "/" + filename)

    def upload_contents(self):
        arr = os.listdir("./UploadAssets")
        cwd = os.getcwd()

        for file in arr:
            if (file != ".gitkeep"):
                response = self.client.upload_file(
                    cwd + "/UploadAssets/" + file, self._bucket_name, "Assets/" + file)


def main():
    BUCKET_NAME = "macs-global-game-jam-2022"
    parser = argparse.ArgumentParser(description='S3 Uploader for gamejam purposes')
    parser.add_argument("-u", "--upload", action="store_true", help="option for uploading via cli")
    parser.add_argument("-d", "--download", action="store_true", help="option for downloading via cli")
    args = parser.parse_args()

    if (args.upload):
        handler = AssetHandler(BUCKET_NAME, 'default', True)
        print("Uploading as cli...")
        handler.upload_contents()

    elif (args.download):
        handler = AssetHandler(BUCKET_NAME, 'default', True)
        print("Downloading as cli...")
        handler.download_contents()

    else:
        handler = AssetHandler(BUCKET_NAME, 'default')
        choice = int(input("Press 1 to upload, 2 to download\n"))

        if choice == 1:
            handler.upload_contents()
        elif choice == 2:
            handler.download_contents()
        else:
            print("are you stupid?")


main()
