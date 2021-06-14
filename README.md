# mux205project
Rendu de projet MUX205 1ere année

## Description du projet
Projet Unity 3D
Création d'un jeu 3D de mining

## Principe du jeu
Un personnage 3D évolue sur un terrain généré de manière procédural. Il peut marcher et courir sur ce terrain en escaladant les obstacles.
Les ressources de minage sont générés aléatoirement sur le terrain et le personnage peut les récupérer en s'approchant.

Une interface graphique 2D du personnage affiche en temps réel ses niveaux :
- Santé : Si le niveau de soif est à zéro, la santé diminue. Si la santé est à zéro, le jeu est terminé.
- Soif : plus le temps passe, plus le niveau de soif diminue.
- Endurance : Plus le personnage fait des efforts plus l'endurance diminue. Quand l'endurance est à zéro, le personnage ralentit. Il doit s'arrêter pour récupérer de l'endurance.

- Nombre de ressources collectées

## Modélisation
Le personnage est modélisé et animé sous Blender avec un algorithme de cinématique inverse.
3 animations sont disponibles :
- idle : respiration du personnage
- walk : marche
- run : course

La ressource de minage (cristal) est modélisé sous Blender également. Un export 2D permet d'afficher une image dans l'interface graphique du personnage.

Le terrain désertique est généré de manière procédurale en utilisant un bruit de Perlin. L'algorithme et la modélisation s'appuient sur le travail de Sébastien Lague : https://github.com/SebLague/Procedural-Landmass-Generation