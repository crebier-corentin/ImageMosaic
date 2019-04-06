# ImageMosaic
Un générateur d'image mosaïque

![Mosaic](berries-mosaic.png)
###### Irina Edilbaeva "Strawberries Topped Cake in Plate" https://www.pexels.com/photo/strawberries-topped-cake-in-plate-2072476/
###### "Large-scale CelebFaces Attributes (CelebA) Dataset" http://mmlab.ie.cuhk.edu.hk/projects/CelebA.html

# PrepareInput

`PrepareInput chemin/vers/le/dossier/contenant/les/images`  

```
-l Limite le nombres d'image prise dans le dossier
-s Taille des images après redimensionnement (20 recommandé) 
-r Recherche les images dans les sous-dossiers
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
