# ImageMosaic
Un générateur d'image mosaïque

# PrepareInput

`PrepareInput chemin/vers/le/dossier/contenant/les/images`  

```
-l Limite le nombres d'image prise dans le dossier
-s Taille des images après redimensionnement (20 recommandé) 
-o Fichier de sortie
```
##### Exemple :
`PrepareInput images/ -l 500 -s 10 -o images.zip`

# MosaicGenerator
```
-i Archive zip créer par PrepareInput
-t Image modèle
-o Fichier de sortie
```
##### Exemple :
`MosaicGenerator -i images.zip -t voiture.png -o mosaic-voiture.png`
