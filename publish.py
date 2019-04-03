import os
import subprocess
import zipfile

from dataclasses import dataclass
from typing import List

rootDirPath = os.path.dirname(os.path.abspath(__file__))

#Zip
def zipdir(path, ziph):
    # ziph is zipfile handle
    for root, dirs, files in os.walk(path):
        for file in files:
            ziph.write(os.path.join(root, file))

def zipFileFromDir(dirPath: str, ouputPath:str):
    zipf = zipfile.ZipFile(ouputPath, 'w', zipfile.ZIP_DEFLATED)
    zipdir(dirPath, zipf)
    zipf.close()

@dataclass
class Info:
    rid: str
    projectName: str

    def projectPath(self)-> str:
        return os.path.join(rootDirPath, f'{self.projectName}/')

    def outputPath(self) -> str:
        return os.path.join(rootDirPath, f'build/{self.projectName}/{self.rid}/')

    def build(self):
        print(f"Building : {self.projectName} {self.rid}")
        subprocess.run(["dotnet", "publish", self.projectPath(), "-c", "release", "-r", self.rid, "-o", self.outputPath()])

    def zip(self):
        zipFileFromDir(self.outputPath(), os.path.join(rootDirPath, "build/", f"{self.projectName}-{self.rid}.zip"))


projects : List[str] = ["PrepareInput", "MosaicGenerator"]
rids : List[str] = ["win-x64", "linux-x64", "osx-x64"]

#List of projects

list : List[Info] = []

for project in projects:
    for rid in rids:
        list.append(Info(rid, project))

for i in list:
    i.build()
    i.zip()
