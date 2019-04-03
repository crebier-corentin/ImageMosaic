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
            ziph.write(os.path.join(root, file), os.path.relpath(os.path.join(root, file), os.path.join(path, '..')))

def zipFileFromDir(dirPath: str, ouputPath:str):
    zipf = zipfile.ZipFile(ouputPath, 'w', zipfile.ZIP_DEFLATED)
    zipdir(dirPath, zipf)
    zipf.close()

@dataclass
class Project:
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


projectNames : List[str] = ["PrepareInput", "MosaicGenerator"]
rids : List[str] = ["win-x64", "linux-x64", "osx-x64"]

#List of projects

projectList : List[Project] = []

for project in projectNames:
    for rid in rids:
        projectList.append(Project(rid, project))

for pro in projectList:
    pro.build()
    pro.zip()
